using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace FGOProtoDecoder
{
    internal class Decoder
    {
        private ModuleDefinition assembly;
        Dictionary<string, ObjectDescription> items = null;
        public Decoder(string assemblyPath)
        {
            assembly = ModuleDefinition.ReadModule(assemblyPath);
            items = new Dictionary<string, ObjectDescription>();
        }
        public Dictionary<string, ObjectDescription> GetItems() { return items; }
        public void Parse()
        {
            Console.WriteLine("Begin parse");
            foreach (var type in GetProtobufTypes())
            {
                ObjectDescription od = (ObjectDescription)parseClass(type);
                items.Add(type.FullName, od);
                Console.WriteLine($"Processed {type.FullName}");
            }
        }
        public ClassDescription parseClass(TypeDefinition t)
        {
            ClassDescription cd = new ClassDescription(t.FullName);
            //Console.WriteLine(t.BaseType.FullName);
            //seems no nested Types,no Enum,no OneOf?
            var oneof_dict = new Dictionary<int, OneofDescription>();
            //get all fields and properties
            var protobufIndices = t.Fields.Where(f => f.Name.EndsWith("FieldNumber"));
            var properties = new Dictionary<string, PropertyDefinition>();
            foreach (var prop in t.Properties) properties.Add(prop.Name, prop);
            foreach(var field in protobufIndices)
            {
                var field_name = field.Name.Remove(field.Name.Length - "FieldNumber".Length);
                var field_tag = (int)field.Constant;
                var field_property = properties[field_name];
                var field_type_name = field_property.PropertyType.FullName;
                if (field_type_name.StartsWith("Google.Protobuf.Collections.Repeated"))
                {      
                    var private_field_name = string.Format("_repeated_{0}_codec", field_name.ReformatString());
                    var private_field = t.Fields.FirstOrDefault(f => f.Name == private_field_name);
                    if (private_field != null)
                    {
                        var stIndex = private_field.FieldType.FullName.IndexOf("<");
                        var edIndex = private_field.FieldType.FullName.IndexOf(">");
                        var element_type = private_field.FieldType.FullName.Substring(stIndex + 1, edIndex - stIndex - 1);
                        field_type_name = string.Format("Google.Protobuf.Collections.RepeatedField<{0}>", element_type);
                    }
                    else
                    {
                        Console.WriteLine(private_field_name);
                        throw new Exception($"Cant find {private_field_name} in Fields");
                    }
                }
                FieldDescription fd = new FieldDescription(field_name, field_tag, field_type_name);
                // process OneOf.but seems no?
                if (oneof_dict.ContainsKey(field_tag))
                    oneof_dict[field_tag].Fields.Add(field_name, fd);
                else
                    cd.Fields.Add(field_name, fd);
            }
            return cd;
        }
        private TypeDefinition[] GetProtobufTypes()
        {
            return assembly.GetTypes().Where(t => t.Namespace == "ProtoData").ToArray();
        }   
    }
    public static partial class Extension
    {
        public static string ReformatString(this string s)
        {
            var rs = "";
            foreach(var str in s.Split('_'))
            {
                var temp = char.ToUpper(str[0])+str.Substring(1);
                rs += temp;
            }
            rs= char.ToLower(rs[0]) + rs.Substring(1);
            return rs;
        }
    }
}

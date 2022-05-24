using System.Collections.Generic;

namespace FGOProtoDecoder
{
	public class FieldDescription : ObjectDescription
	{
		public int Tag = -1;
		public string Type = null;
		private static Dictionary<string, string> pbTypeNames = new Dictionary<string, string>() {
			{typeof(System.UInt32).FullName, "uint32"},
			{typeof(System.UInt64).FullName, "uint64"},
			{typeof(System.Int32).FullName, "int32"},
			{typeof(System.Int64).FullName, "int64"},
			{typeof(System.Boolean).FullName, "bool"},
			{typeof(System.String).FullName, "string"},
			{typeof(float).FullName, "float"},
			{typeof(double).FullName, "double"},
			{"Google.Protobuf.ByteString", "bytes"},
		};
		public FieldDescription(string name, int tag, string type) : base(name)
		{
			Tag = tag;
			Type = type;
		}
		public override string[] ToPBLines()
		{
			return new string[] { MapCsTypeToPb(Type) + " " + Name.ToSnakeCase() + " = " + Tag };
		}
		public static string MapCsTypeToPb(string typename, bool annotate_enums = false, bool add_repeated = true)
		{
			if (pbTypeNames.ContainsKey(typename)) return pbTypeNames[typename];
			if (typename.StartsWith("Google.Protobuf.Collections.Repeated"))
			{
				var element_type = typename.Split('<')[1];
				element_type = element_type.Substring(0, element_type.Length - 1);
				var type_name = MapCsTypeToPb(element_type, annotate_enums);
				if (add_repeated) return "repeated " + type_name;
				return type_name;
			}
			var name = typename.CutAfterPlusAndDot();
			return name;
		}
	}
}

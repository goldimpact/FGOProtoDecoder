using System.Linq;
using System.Collections.Generic;

namespace FGOProtoDecoder
{
	public class ClassDescription : ObjectDescription
	{
		public Dictionary<string, FieldDescription> Fields = null;
		public Dictionary<string, ClassDescription> Classes = null;
		public ClassDescription(string name) : base(name)
		{
			Fields = new Dictionary<string, FieldDescription>();
			Classes = new Dictionary<string, ClassDescription>();
		}
		public string[] GetExternalTypes(bool include_google_types = false)
		{
			var used_types = GetUsedTypes();
			var provided_types = GetProvidedTypes();
			var difference = used_types.Except(provided_types);
			var types = new List<string>();
			foreach (var t in difference)
			{
				if (t.StartsWith("System.")) continue;
				if (t.StartsWith("Google.") && !include_google_types) continue;
				types.Add(t);
			}
			return types.ToArray();
		}
		public string[] GetUsedTypes()
		{
			var types = new List<string>();
			foreach (var f in Fields)
			{
				var od = f.Value as OneofDescription;
				if (od != null)
				{
					foreach (var ff in od.Fields) types.Add(ff.Value.Type);
				}
				else types.Add(f.Value.Type);
			}
			foreach (var c in Classes) types.AddRange(c.Value.GetUsedTypes());
			var extra_types = new List<string>();
			foreach (var t in types)
			{
				if (t.StartsWith("Google."))
				{
					int pos = t.IndexOf('<');
					var types_string = t.Substring(pos + 1, t.Length - pos - 2);
					var types_arr = types_string.Split(',');
					extra_types.AddRange(types_arr);
				}
			}
			types.AddRange(extra_types);
			return types.Distinct().ToArray();
		}
		public string[] GetProvidedTypes()
		{
			var types = new List<string>();
			foreach (var c in Classes)
			{
				types.Add(c.Value.Name);
				types.AddRange(c.Value.GetProvidedTypes());
			}
			return types.ToArray();
		}
		public override string[] ToPBLines()
		{
			var lines = new List<string>();
			var name = Name.CutAfterPlusAndDot();
			/*
			if (name.Contains('_'))
            {
				var temp = name.ReformatString();
				name = char.ToUpper(temp[0])+temp.Substring(1);
            }
			*/
			lines.Add("message " + name + " {");
			if (Classes.Count > 0)
			{
				foreach (var item in Classes) lines.AddRange(item.Value.ToPBLines().PadStrings());
				lines.Add("");
			}
			if (Fields.Count > 0)
			{
				foreach (var item in Fields) lines.AddRange(item.Value.ToPBLines().PadStrings("\t", (item.Value is OneofDescription) ? "" : ";"));
			}
			lines.Add("}");
			return lines.ToArray();
		}
	}
}

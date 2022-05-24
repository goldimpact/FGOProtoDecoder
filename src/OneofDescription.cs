using System.Collections.Generic;

namespace FGOProtoDecoder
{
	public class OneofDescription : FieldDescription
	{
		public Dictionary<string, FieldDescription> Fields = null;
		public OneofDescription(string name) : base(name, -1, "")
		{
			Fields = new Dictionary<string, FieldDescription>();
		}
		public override string[] ToPBLines()
		{
			var lines = new List<string>();
			lines.Add("oneof " + Name.CutAfterPlusAndDot() + " {");
			foreach (var item in Fields) lines.AddRange(item.Value.ToPBLines().PadStrings("\t", ";"));
			lines.Add("}");
			return lines.ToArray();
		}
	}
}

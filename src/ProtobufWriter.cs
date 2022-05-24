using System;
using System.Collections.Generic;
using System.IO;

namespace FGOProtoDecoder
{
	public class ProtobufWriter : DescriptionWriter
	{
		public ProtobufWriter(Dictionary<string, ObjectDescription> items) : base(items)
		{
		}
		public override void DumpToDirectory(string directory)
		{
			foreach (var item in Items)
			{
				var filename = Path.Combine(directory, item.Key.CutAfterPlusAndDot() + ".proto");
				var w = new StreamWriter(filename);
				w.WriteLine("syntax = \"proto3\";");
				w.WriteLine();
				DumpItem(w, item.Value);
				w.Close();
			}
		}
		protected void DumpItem(StreamWriter w, ObjectDescription item)
		{
			var lines = item.ToPBLines();
			var c = item as ClassDescription;
			if (c != null)
			{
				// Imports
				foreach (var t in c.GetExternalTypes())
				{
					var cut = t.CutAfterPlusAndDot();
					if (cut.Length == 0) throw new Exception("PooPee: " + t);
					w.WriteLine("import \"{0}.proto\";", cut);
				}
				w.WriteLine();
			}
			foreach (var line in lines) w.WriteLine(line);
		}
	}
}

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
		public override void DumpToFile(string path)
		{
			var filename = Path.Combine(path, "Protos.proto");
			//Using statement calls Dispose which will flush the buffer to the FileStream and also closes the file stream. So you don't need to close it manually.
			using (StreamWriter w = new StreamWriter(filename))
			{
				w.WriteLine("syntax = \"proto3\";");
				foreach (var item in Items) DumpItem(w, item.Value, true);
				w.WriteLine();
			}
			Console.WriteLine($"One file dumped to {path}\n");
		}
		public override void DumpToDirectory(string directory)
		{
			foreach (var item in Items)
			{
				var filename = Path.Combine(directory, item.Key.CutAfterPlusAndDot() + ".proto");
				using (StreamWriter w = new StreamWriter(filename))
				{
					w.WriteLine("syntax = \"proto3\";");
					w.WriteLine();
					DumpItem(w, item.Value, false);
					//w.Close();
				}	
			}
			Console.WriteLine($"Separated files dumped to {directory}\n");
		}
		protected void DumpItem(StreamWriter w, ObjectDescription item, bool isToFile)
		{
			var lines = item.ToPBLines();
			var c = item as ClassDescription;
			if (c != null)
			{
				// Imports
				foreach (var t in c.GetExternalTypes())
				{
					if (!isToFile)
					{
						var cut = t.CutAfterPlusAndDot();
						if (cut.Length == 0) throw new Exception("import file name length is 0" + t);
						w.WriteLine("import \"{0}.proto\";", cut);
					}
				}
				w.WriteLine();
			}
			foreach (var line in lines) w.WriteLine(line);
		}
	}
}

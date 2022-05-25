using System.Collections.Generic;

namespace FGOProtoDecoder
{
	public abstract class DescriptionWriter
	{
		protected Dictionary<string, ObjectDescription> Items = null;
		protected bool isToFile = false;
		public DescriptionWriter(Dictionary<string, ObjectDescription> items)
		{
			Items = items;
		}
		public abstract void DumpToFile(string filename);
		public abstract void DumpToDirectory(string directory);
	}
}

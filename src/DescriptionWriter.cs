using System.Collections.Generic;

namespace FGOProtoDecoder
{
	public abstract class DescriptionWriter
	{
		protected Dictionary<string, ObjectDescription> Items = null;
		public DescriptionWriter(Dictionary<string, ObjectDescription> items)
		{
			Items = items;
		}
		public abstract void DumpToDirectory(string directory);
	}
}

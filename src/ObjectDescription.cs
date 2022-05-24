
namespace FGOProtoDecoder
{
	public abstract class ObjectDescription
	{
		public string Name;
		public ObjectDescription(string name)
		{
			Name = name;
		}
		public abstract string[] ToPBLines();
	}
}

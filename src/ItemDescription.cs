
namespace FGOProtoDecoder
{
	public class ItemDescription : ObjectDescription
	{
		public int Value = -1;
		public ItemDescription(string name, int value) : base(name)
		{
			Value = value;
		}
		public override string[] ToPBLines()
		{
			return new string[] { Name.ToSnakeCase().ToUpper() + " = " + Value.ToString() };
		}
	}
}

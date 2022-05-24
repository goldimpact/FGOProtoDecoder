using System;
using System.IO;
using System.Windows.Forms;

namespace FGOProtoDecoder
{
    public class Program
    {
		public static string WhereFile(string type)
		{
			var fPath = string.Empty;
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = "./";
				openFileDialog.Filter = $"{type} files (*.{type})|*.{type}|All files (*.*)|*.*";
				openFileDialog.FilterIndex = 1;
				openFileDialog.RestoreDirectory = true;
				if (openFileDialog.ShowDialog() == DialogResult.OK) fPath = openFileDialog.FileName;
			}
			return fPath;
		}
		[STAThread]
		static void Main(string[] args)
        {
			var assembly_path = WhereFile("dll");
			var out_path = Path.Combine(Path.GetDirectoryName(assembly_path), "DumpResult");
			if (!Directory.Exists(out_path)) Directory.CreateDirectory(out_path);
			var decoder = new Decoder(assembly_path);
			decoder.Parse();
			var descriptionWriter = new ProtobufWriter(decoder.GetItems());
			descriptionWriter.DumpToDirectory(out_path);
			Console.WriteLine("Done");
			Console.ReadKey();
		}
    }
}

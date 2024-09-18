using System;
using System.Threading.Tasks;
using Opal;

namespace SyntheticProjects
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Missing arguments.");
				return;
			}

			var directory = Path.Parse(args[0]);
			var fanOut = 3;
			var depth = 4;
			var fileCount = 100;
			var generator = new Soup.CppProjectGenerator(fanOut, depth, fileCount);
			await generator.GenerateProjectsAsync(
				directory);
		}
	}
}

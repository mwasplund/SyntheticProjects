using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Opal;

namespace SyntheticProjects
{
	public class CppProjectGenerator
	{
		private static Path RecipeFileName => new Path("Recipe.sml");
		private static string RecipeFileFormat => @"Name: ""{0}""
Language: ""C++|0.1""
Version: ""1.0.0""
Type: ""StaticLibrary""

Source: [
{1}]

Dependencies: {{
	Runtime: [
{2}	]
}}
";

		private static string ClassFileNameFormat => "{0}.cpp";
		private static string ClassFileFormat => @"
namespace {0}
{{
	class {1}
	{{
	public:
		static void DoStuff()
		{{
		}}
	}};
}}
";

		private int uniqueId = 1;
		private int fanOut;
		private int maxDepth;
		private int fileCount;

		public CppProjectGenerator(int fanOut, int maxDepth, int fileCount)
		{
			this.fanOut = fanOut;
			this.maxDepth = maxDepth;
			this.fileCount = fileCount;
		}

		public async Task GenerateProjectsAsync(Path root)
		{
			var references = await GenerateProjectsRecursiveAsync(root, 0);

			// Create the root project
			var projectName = $"RootProject";
			var projectDirectory = root + new Path(projectName);
			await GenerateProjectAsync(projectDirectory, projectName, references);
		}

		public async Task<List<Path>> GenerateProjectsRecursiveAsync(Path root, int depth)
		{
			var resultReferences = new List<Path>();
			for (var i = 1; i <= fanOut; i++)
			{
				var references = new List<Path>();
				if (depth < maxDepth)
					references = await GenerateProjectsRecursiveAsync(root, depth + 1);

				var projectName = $"Project{uniqueId++}";
				var projectDirectory = root + new Path(projectName);
				await GenerateProjectAsync(projectDirectory, projectName, references);

				resultReferences.Add(projectDirectory);
			}

			return resultReferences;
		}

		private async Task GenerateProjectAsync(Path directory, string name, IList<Path> references)
		{
			Console.WriteLine($"Generate Project: {name}");

			// Ensure the directory exists
			if (!System.IO.Directory.Exists(directory.ToString()))
			{
				System.IO.Directory.CreateDirectory(directory.ToString());
			}

			var files = new List<Path>();
			for (var i = 1; i <= this.fileCount; i++)
			{
				var className = $"Class{i}";
				var filePath = await WriteClassFileAsync(directory, name, className);
				files.Add(filePath);
			}

			await WriteRecipeFileAsync(directory, name, references, files);
		}

		private static async Task<Path> WriteClassFileAsync(Path directory, string projectName, string className)
		{
			var fileContent = string.Format(ClassFileFormat, projectName, className);
			var file = new Path(string.Format(ClassFileNameFormat, className));
			var filePath = directory + file;

			using (var fileStream = System.IO.File.Open(filePath.ToString(), System.IO.FileMode.Create, System.IO.FileAccess.Write))
			using (var stringWriter = new System.IO.StreamWriter(fileStream))
			{
				await stringWriter.WriteAsync(fileContent);
			}

			return file;
		}

		private static async Task WriteRecipeFileAsync(
			Path directory,
			string name,
			IList<Path> references,
			IList<Path> source)
		{
			var sourceList = string.Concat(source.Select(value => $"\t\"{value}\"\n"));
			var referencesList = string.Concat(
				references
				.Select(value => value.GetRelativeTo(directory))
				.Select(value => $"\t\t\"{value}\"\n"));

			var fileContent = string.Format(
				RecipeFileFormat,
				name,
				sourceList,
				referencesList);
			var filePath = directory + RecipeFileName;

			using (var fileStream = System.IO.File.Open(filePath.ToString(), System.IO.FileMode.Create, System.IO.FileAccess.Write))
			using (var stringWriter = new System.IO.StreamWriter(fileStream))
			{
				await stringWriter.WriteAsync(fileContent);
			}
		}
	}
}

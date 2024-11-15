using FileCreator;
using Microsoft.Extensions.Configuration;

var fileSizeMb = 10240;
if (args.Length > 0)
	fileSizeMb = int.Parse(args[0]);

var basePath = System.IO.Directory.GetCurrentDirectory();
var configuration = new ConfigurationBuilder()
	.SetBasePath(basePath)
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

var wordsFileName = configuration["WordsFileName"];
var outputFolder = configuration["OutputPath"];
if (!Directory.Exists(outputFolder))
{
	Console.WriteLine($"Output folder {outputFolder} does not exists. Create the folder or change output folder in appsettings.json");
	return;
}

var wordsPath = Path.Combine(basePath, wordsFileName);
var words = File.ReadLines(wordsPath);
var stringGenerator = new RandomWordStringGenerator(words.ToList(), 10);
var fileCreator = new FileCreator.FileCreator(stringGenerator, fileSizeMb);

var fileName = Path.Combine(outputFolder, $"source_{fileSizeMb}Mb.txt");

Console.WriteLine($"Start creating file {fileName}");
fileCreator.CreateFile(fileName);

Console.WriteLine($"File {fileName} Created");

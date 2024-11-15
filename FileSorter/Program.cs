using FileSorter;

if (args.Length == 0)
{
	Console.WriteLine("Specify the full file path");
	return;
}

string filePath = "";
if (args.Length > 0)
	filePath = args[0];

if (!File.Exists(filePath))
{
	Console.WriteLine($"The file {filePath} does not exist.");
	return;
}

string folder = Path.GetDirectoryName(filePath);
string fileName = Path.GetFileName(filePath);

int chunkSize = 500_000;
if (args.Length > 0)
	filePath = args[0];

if (args.Length > 1)
	chunkSize = int.Parse(args[1]);

string outputFile = Path.Combine(folder, Path.GetFileNameWithoutExtension(fileName) + "_sorted.txt");

Console.WriteLine($"File {filePath}, chunk {chunkSize}");
var fileSorter = new ExternalMergeSort(filePath, outputFile, chunkSize);
fileSorter.Sort();
Console.WriteLine("Sorted");
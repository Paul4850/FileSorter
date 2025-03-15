using FileCreator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var fileSizeMb = 10000;//10240;
int maxFileSizeMb = 1000000;

if (!Helpers.TryParseFileSize(args, fileSizeMb, maxFileSizeMb, out fileSizeMb))
    return;

var basePath = System.IO.Directory.GetCurrentDirectory();
var configuration = new ConfigurationBuilder()
	.SetBasePath(basePath)
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false).Build();

var wordsFileName = "";
var outputFolder = "";
var wordsPath = "";
var meanWordsInLine = 10;

if (!Helpers.TryGetConfiguration(configuration, out wordsFileName, out meanWordsInLine, out outputFolder))
    return;

if (!Helpers.ValidateFiles(basePath, wordsFileName, out wordsPath))
    return;

var words = File.ReadLines(wordsPath);

var stringGenerator = new RandomWordStringGenerator(words,  meanWordsInLine: 20);
var fileName = Path.Combine(outputFolder, $"source_{fileSizeMb}Mb.txt");

Console.WriteLine($"Start creating file {fileName}");


using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("FileCreator");

TimeLogger.MeasureExecutionTime(
    () => FileCreator.FileCreator.Create(fileName, fileSizeMb, stringGenerator.Generate),
        nameof(FileCreator.FileCreator.Create),
        logger
    );

Console.WriteLine($"File {fileName} Created");
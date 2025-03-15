using FileCreator;
using FileSorter;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace FileSorterTests
{
    public class FileSorterTests
    {
        private string baseTestDir;
        private string outputFolder;
        private string dataFolder;
        private IConfiguration _configuration;
        private string[] words = "brown fox jumps over the lazy dog".Split(' ');
        private int fileSizeMb = 3;
        private const int megaByteSize = 1024 * 1024;

        [SetUp]
        public void Setup()
        {
            baseTestDir = TestContext.CurrentContext.WorkDirectory;

            _configuration = new ConfigurationBuilder()
                .SetBasePath(baseTestDir)
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false)
                .Build();

            var relativeOutputFolder = _configuration["TestOutputFolder"]!;
            outputFolder = Path.Combine(baseTestDir, relativeOutputFolder);

            var relativeDataFolder = _configuration["TestDataFolder"]!;
            dataFolder = Path.Combine(baseTestDir, relativeDataFolder);
        }


        [TearDown]
        public void TearDown()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(outputFolder);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        [Test]
        public void TestRandomWordStringGenerator()
        {
            var stringGenerator = new RandomWordStringGenerator(words, meanWordsInLine: 10);
            var result = stringGenerator.Generate();
            
            Assert.IsNotEmpty(result);

            var resultWords = result.Split(' ');
            resultWords.ToList().ForEach(w =>
                Assert.IsTrue( w.All(char.IsLetter))
            );
        }

        [Test]
        public void TestFileCreator()
        {
            string fileName = Path.Combine(outputFolder, $"Unsorted_{fileSizeMb}.txt");
            var stringGenerator = new RandomWordStringGenerator(words, meanWordsInLine: 10);
            FileCreator.FileCreator.Create(fileName, fileSizeMb, stringGenerator.Generate);

            Assert.IsTrue(File.Exists(fileName));
            
            double length = new System.IO.FileInfo(fileName).Length / megaByteSize;

            Assert.IsTrue(Math.Abs(length - fileSizeMb) <= 1);
        }
        
        [TestCase("source_1Mb.txt", "source_1Mb_sorted.txt")]
        public void TestFileSorter(string sourceFileName, string targetFileName)
        {
            string sourceFilePath = Path.Combine(dataFolder, sourceFileName);
            string targetFilePath = Path.Combine(dataFolder, targetFileName);

            string outputFilePath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(sourceFileName) + "_sorted.txt");

            var sorter = new ExternalMergeSort(sourceFilePath, outputFilePath, linesInChunk: 10000, outputFolder);
            sorter.Sort();
            
            Assert.IsTrue(File.Exists(outputFilePath));

            string content1 = File.ReadAllText(targetFilePath);
            string content2 = File.ReadAllText(outputFilePath);

            Assert.That(content2, Is.EqualTo(content1), "File contents are different.");
        }
    }
}
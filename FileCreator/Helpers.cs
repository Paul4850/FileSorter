using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCreator
{
    internal class Helpers
    {
        public static bool TryParseFileSize(string[] args, int defaultSize, int maxSize, out int fileSize)
        {
            fileSize = defaultSize;
            if (args.Length == 0)
                return true;

            if (!int.TryParse(args[0], out int size))
            {
                Console.WriteLine($"Incorrect file size specified: {args[0]}");
                return false;
            }
            if (size <= 0 || size > maxSize)
            {
                Console.WriteLine($"Incorrect file size specified: {args[0]}. Must be positive and not greater than {maxSize}");
                return false;
            }
            fileSize = size;
            return true;
        }


        public static bool TryGetConfiguration(IConfiguration configuration,
            out string wordsFileName, out int meanWordsInLine, out string outputFolder)
        {
            wordsFileName = configuration["WordsFileName"] ?? "";
            outputFolder = configuration["OutputFolder"] ?? "";
            meanWordsInLine = Convert.ToInt32(configuration["MeanWordsInLine"]);

            if (string.IsNullOrWhiteSpace(wordsFileName))
            {
                Console.WriteLine("WordsFileName is empty. Provide relative path and file name in appsettings.json");
                return false;
            }
            if (string.IsNullOrWhiteSpace(outputFolder) || !Directory.Exists(outputFolder))
            {
                Console.WriteLine($"Output folder '{outputFolder}' does not exist. Create the folder or change output folder in appsettings.json");
                return false;
            }
            return true;
        }

        public static bool ValidateFiles(string basePath, string wordsFileName, out string wordsPath)
        {
            wordsPath = Path.Combine(basePath, wordsFileName);
            if (!File.Exists(wordsPath))
            {
                Console.WriteLine($"File '{wordsPath}' does not exist. Provide relative path and file name in appsettings.json");
                return false;
            }
            return true;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Регистрируем логирование с выводом в консоль
            services.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.SetMinimumLevel(LogLevel.Information);
            });
        }
    }
}

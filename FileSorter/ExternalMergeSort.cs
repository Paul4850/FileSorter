using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FileSorter
{
    public class ExternalMergeSort
	{
		private readonly string inputFilePath;
		private readonly string outputFilePath;
		private readonly int linesInChunk;
        private string chunksFolder = "";

        public ExternalMergeSort(string inputFilePath, string outputFilePath, int linesInChunk, string chunksFolder)
		{
			this.inputFilePath = inputFilePath;
			this.outputFilePath = outputFilePath;
			this.linesInChunk = linesInChunk;
			this.chunksFolder = chunksFolder;
        }

		public void Sort()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var start = DateTime.Now;
			Console.WriteLine(start);

            List<string> tempFiles = SplitAndSortChunks(inputFilePath, linesInChunk, chunksFolder);
			Console.WriteLine($"SplitAndSortChunks: {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}");
			stopwatch.Restart();

			MergeChunks(tempFiles, outputFilePath);
			Console.WriteLine($"MergeChunks: {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}");
			stopwatch.Restart();

			Console.WriteLine($"Deleting files");
			foreach (var tempFile in tempFiles)
				File.Delete(tempFile);
			Console.WriteLine($"Files deleted: {stopwatch.Elapsed.ToString(@"hh\:mm\:ss")}");

			Console.WriteLine($"Total time: {DateTime.Now.Subtract(start).ToString(@"hh\:mm\:ss")}");
			stopwatch.Stop();
		}

		private static List<string> SplitAndSortChunks(string filePath, int linesInChunk, string chunksFolder)
		{
			int chunkCounter = 0;
			List<string> tempFileNames = new List<string>();
            List<Task> tasks = new List<Task>();
            object lockObject = new object();

            static List<Line> ReadChunk(IEnumerator<string> enumerator, int count)
            {
                var chunk = new List<Line>();
                for (int i = 0; i < count && enumerator.MoveNext(); i++)
                {
                    chunk.Add(new Line(enumerator.Current));
                }
                return chunk;
            }

            static void WriteChunk(List<Line> chunk, string fileName)
            {
                using (var writer = new StreamWriter(fileName, false, System.Text.Encoding.ASCII, bufferSize: 8192))
                {
                    foreach (var line in chunk.OrderBy(line => line))
                        writer.WriteLine(line.Compile());
                }
            }

            using (var linesEnumerator = File.ReadLines(filePath).GetEnumerator())
			{
                while (true)
				{
					var chunk = ReadChunk(linesEnumerator, linesInChunk);

                    if (chunk.Count == 0)
						break;
					
                    string tempFileName = Path.Combine(chunksFolder, $"temp_chunk_{chunkCounter}.txt");
                    chunkCounter++;

					tasks.Add(Task.Run(() =>
					{
						WriteChunk(chunk, tempFileName);

                        lock (lockObject)
						{
                            tempFileNames.Add(tempFileName);
						}
					}));
				}
				Task.WaitAll(tasks.ToArray());
			}
			return tempFileNames;
        }


		private static void MergeChunks(List<string> tempFiles, string outputFilePath)
		{
			using (var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough | FileOptions.Asynchronous))
			using (var bufferedStream = new BufferedStream(fileStream, 131072)) // 128Kb
			using (var writer = new StreamWriter(bufferedStream, Encoding.ASCII, 131072))  // 128Kb
			{
				List<StreamReader> readers = tempFiles
					.Select(f => new StreamReader(f, Encoding.ASCII, false, bufferSize: 8192))
					.ToList();
				try
				{
					var minHeap = new PriorityQueue<(Line line, int index), Line>();

					for (int i = 0; i < readers.Count; i++)
					{
						if (!readers[i].EndOfStream)
						{
							string? line = readers[i].ReadLine();
							while(line == null && !readers[i].EndOfStream)
								line = readers[i].ReadLine();
							if (line == null) continue;
							var l = new Line(line);
							minHeap.Enqueue((l, i), l);
						}
					}

					while (minHeap.Count > 0)
					{
						var (minLine, minIndex) = minHeap.Dequeue();
						writer.WriteLine(minLine.Compile());

						if (!readers[minIndex].EndOfStream)
						{
							string? nextLine;
							do
							{
								nextLine = readers[minIndex].ReadLine();
								if (nextLine == null) continue;

								var nextL = new Line(nextLine);
								minHeap.Enqueue((nextL, minIndex), nextL);

							} while (nextLine == null);
						}
					}


				}
				finally
				{
					foreach (var reader in readers)
					{
						reader.Close();
					}
				}
			}
		}
	}
}

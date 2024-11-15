using System.Text;

namespace FileSorter
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;

	public class ExternalMergeSort
	{
		public static string separator = ". ";
		private readonly string inputFilePath;
		private readonly string outputFilePath;
		private readonly int chunkSize;

		public ExternalMergeSort(string inputFilePath, string outputFilePath, int chunkSize)
		{
			this.inputFilePath = inputFilePath;
			this.outputFilePath = outputFilePath;
			this.chunkSize = chunkSize;
		}

		public void Sort()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var start = DateTime.Now;
			Console.WriteLine(start);
			
			List<string> tempFiles = SplitAndSortChunks(inputFilePath, chunkSize);
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

		class Line : IComparable<Line> {
			public  string str = "";
			int pos = 0;
			int hash = 0;
			public Line(string str) { 
				this.str = str;
				hash = str.GetHashCode();
				pos = str.IndexOf(separator);
				Number = int.Parse(str.AsSpan(0, pos));	
			}
			int Number { get;  }
			public ReadOnlySpan<char> AsSpan => str.AsSpan(pos + separator.Length);

			public int CompareTo(Line? other)
			{
				if (other == null)
					return 1;
				var res = AsSpan.CompareTo(other.AsSpan, StringComparison.Ordinal);
				if(res != 0)
					return res;
				return this.Number.CompareTo(other.Number);
			}
			public string Compile() => str;
		}

		private static List<string> SplitAndSortChunks(string filePath, int chunkSize)
		{
			int chunkCounter = 0;
			List<string> tempFiles = new List<string>();

			using (var linesEnum = File.ReadLines(filePath).GetEnumerator())
			{
				List<Task> tasks = new List<Task>(); 

				while (true)
				{
					var chunk = new List<Line>();

					for (int i = 0; i < chunkSize && linesEnum.MoveNext(); i++)
						chunk.Add(new Line(linesEnum.Current));

					if (chunk.Count == 0)
						break;

					string tempFileName = $"temp_chunk_{chunkCounter}.txt";
					chunkCounter++;

					tasks.Add(Task.Run(() =>
					{
						var sortedChunk = chunk.OrderBy(line => line).ToList();

						using (var writer = new StreamWriter(tempFileName, false, System.Text.Encoding.ASCII, 8192))
						{
							foreach (var line in sortedChunk)
							{
								writer.WriteLine(line.Compile());
							}
						}

						lock (tempFiles)
						{
							tempFiles.Add(tempFileName);
						}
					}));
				}
				Task.WaitAll(tasks.ToArray());
			}
			return tempFiles;
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
						string finalLine = minLine.Compile();
						writer.WriteLine(finalLine);

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

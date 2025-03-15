namespace FileCreator
{
	public class FileCreator
	{
		public static void Create(string fileName, int maxSizeMb,  Func<string> textGenerator)
		{
            long bytesToWrite = ((long)maxSizeMb) << 20;
			int maxNumber = 32767;

            Random random = new Random();
			using (StreamWriter outfile = new StreamWriter(fileName,  append: false, System.Text.Encoding.ASCII,  bufferSize: 131072))
			{
				var str = textGenerator();
				string line = $"{random.Next(maxNumber)}. {str}";
				long linesCount = 0;
				while(bytesToWrite > 0)
				{
					//condition to provide 20% of duplicated strings
					if (linesCount % 5 != 0)
						str = textGenerator();
					line = $"{random.Next(maxNumber)}. {str}";

					bytesToWrite -= (line.Length + Environment.NewLine.Length);
					outfile.WriteLine(line);
                    linesCount++;
				}
			}
		}
	}
}

namespace FileCreator
{
	public class FileCreator
	{
		RandomWordStringGenerator _stringGenerator;
		long fileSize = 0;
		public FileCreator(RandomWordStringGenerator stringGenerator, int maxSizeMb) {
			_stringGenerator = stringGenerator;
			fileSize = ((long)maxSizeMb)<<20;
		}
		public void CreateFile(string fileName)
		{
			Random random = new Random();
			using (StreamWriter outfile = new StreamWriter(fileName, false, System.Text.Encoding.ASCII, 131072))
			{
				var str = _stringGenerator.GenerateRandomWordString();
				string line = $"{random.Next()}. {str}";
				long linesCount = 0;
				while(fileSize >0)
				{
					//condition to provide 20% of duplicated strings
					if (linesCount % 5 != 0)
						str = _stringGenerator.GenerateRandomWordString();
					line = $"{random.Next()}. {str}";

					outfile.WriteLine(line);
					fileSize -= line.Length;
					linesCount++;
				}
			}
		}
	}
}

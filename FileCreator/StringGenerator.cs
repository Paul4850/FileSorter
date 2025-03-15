using System.Text;
using System.Text.RegularExpressions;

namespace FileCreator
{
	public class RandomWordStringGenerator
	{
		Random random = new Random();
		private int maxLength = 1024;

		private readonly List<string> words = new List<string>();
		private readonly int meanWordsInLine = 10;
        private readonly Regex alphanumericRegex = new Regex(@"^[a-zA-Z0-9]+$");

        public RandomWordStringGenerator(IEnumerable<string> words, int meanWordsInLine) 
		{
			this.meanWordsInLine = meanWordsInLine;
			this.words = words.Where(s => alphanumericRegex.IsMatch(s)).ToList();
			if (words.Count() == 0)
				throw new ArgumentException($"No valid words in the {nameof(words)} collection");
		}

		public string Generate()
		{
            //Generate realistic string length distribution.
            int wordCount = GenerateNormalDistributedValue(meanWordsInLine, (double)meanWordsInLine /3);
			if (wordCount <= 0)
				wordCount = 1;

            var result = new StringBuilder();

			for (int i = 0; i < wordCount; i++)
			{
				string word = words[random.Next(words.Count)];
				if (word.Length < 1) 
					continue;
				if (i == 0)
				{
					word = char.ToUpper(word[0]) + word.Substring(1);
				}
				result.Append(word);

				if (i < wordCount - 1)
				{
					result.Append(' ');
				}
			}
			var str = result.ToString();
			str = str.Length > maxLength ? str.Substring(0, maxLength) : str;
			return str;
		}

		/// <summary>
		/// Use Box-Muller transform to emulate normal distribution
		/// </summary>
		/// <param name="mean"></param>
		/// <param name="stdDev"></param>
		/// <returns></returns>
		int GenerateNormalDistributedValue(int mean, double stdDev)
		{
			double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
			double u2 = 1.0 - random.NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
						 Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
			double randNormal =
						 mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

			int randNum = (int)Math.Abs(Math.Round(randNormal));
			return randNum;
		}
	}
}

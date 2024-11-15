using System.Text;
using System.Text.RegularExpressions;

namespace FileCreator
{
	public class RandomWordStringGenerator
	{
		private int maxLength = 1024;
		private int averageWordLength = 5;

		private static readonly Random _random = new Random();
		private readonly List<string> _words = new List<string>();
		private readonly int _meanWordsInLine = 10;
		
		public RandomWordStringGenerator(IEnumerable<string> words, int meanWordsInLine) {
			var alphanumericRegex = new Regex(@"^[a-zA-Z0-9]+$");
			_meanWordsInLine = meanWordsInLine;
			_words = words.Where(s => alphanumericRegex.IsMatch(s)).ToList();
		}

		public string GenerateRandomWordString()
		{
			//Generate realistic string length distribution.
			int wordCount = GetNormal(_meanWordsInLine, (maxLength/averageWordLength - _meanWordsInLine)/4);
			if (wordCount <= 0) return string.Empty;

			var result = new StringBuilder();

			for (int i = 0; i < wordCount; i++)
			{
				string word = _words[_random.Next(_words.Count)];
				if (word.Length < 1) 
					continue;
				if (i == 0)
				{
					word = char.ToUpper(word[0]) + word.Substring(1);
				}
				result.Append(word);

				if (i < wordCount - 1)
				{
					result.Append(" ");
				}
			}
			var str = result.ToString();
			str = str.Length > maxLength ? str.Substring(0, maxLength) : str;
			return str;
		}

		Random rand = new Random();

		/// <summary>
		/// Use Box-Muller transform to emulate normal distribution
		/// </summary>
		/// <param name="mean"></param>
		/// <param name="stdDev"></param>
		/// <returns></returns>
		int GetNormal(int mean, double stdDev)
		{
			double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
			double u2 = 1.0 - rand.NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
						 Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
			double randNormal =
						 mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

			int randNum = (int)Math.Abs(Math.Round(randNormal));
			return randNum;
		}
	}
}

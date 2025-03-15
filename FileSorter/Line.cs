namespace FileSorter
{
    internal class Line : IComparable<Line>
    {
        private static string separator = ". ";
        private string str = "";
        private int pos = 0;

        public Line(string str)
        {
            this.str = str;
            pos = str.IndexOf(separator);
            Number = int.Parse(str.AsSpan(0, pos));
        }
        public int Number { get; }

        public ReadOnlySpan<char> AsSpan => str.AsSpan(pos + separator.Length);

        public string Compile() => str;

        #region IComparable
        public int CompareTo(Line? other)
        {
            if (other == null)
                return 1;
            var res = AsSpan.CompareTo(other.AsSpan, StringComparison.Ordinal);
            if (res != 0)
                return res;
            return this.Number.CompareTo(other.Number);
        }
        #endregion
    }

}

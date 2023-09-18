namespace Models
{
    public struct Score
    {
        public int NumericScore;
        public bool Spare;
        public bool Strike;

        public Score(int numericScore, bool spare = false, bool strike = false)
        {
            NumericScore = numericScore;
            Spare = spare;
            Strike = strike;
        }
    }
}
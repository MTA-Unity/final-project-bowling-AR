namespace Models
{
    public struct RollRecord
    {
        public int RollNumber;
        public int FallenPins;
        public Score Score;

        public RollRecord(int rollNumber, int fallenPins, Score score)
        {
            RollNumber = rollNumber;
            FallenPins = fallenPins;
            Score = score;
        }
    }
}
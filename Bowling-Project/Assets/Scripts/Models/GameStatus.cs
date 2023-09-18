
namespace Models
{
    public struct GameStatus
    {
        public bool GameStarted;
        public bool CheckFallenPins;
        public bool NextPlayerTurn;

        public FrameRecord CurrentFrame;
        public int CurrentPlayer;
        public int FallenPinsInFrame;

        public GameStatus(bool gameStarted = false)
        {
            NextPlayerTurn = true;
            GameStarted = gameStarted;
            CurrentFrame = new FrameRecord(1);
            CurrentPlayer = 1;
            FallenPinsInFrame = 0;
            CheckFallenPins = false;
        }
    }
}

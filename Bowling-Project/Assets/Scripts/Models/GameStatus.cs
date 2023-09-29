using FrameModel;
using UnityEngine;
using System.Collections;

namespace Models
{
    public class GameStatus
    {
        public bool GameStarted;
        public bool CheckFallenPins;

        public FrameRecord CurrentFrame;
        public int CurrentPlayer;
        public int FallenPinsInFrame;

        public GameStatus(bool gameStarted = false)
        {
            GameStarted = gameStarted;
            CurrentFrame = new FrameRecord(1);
            CurrentPlayer = 1;
            FallenPinsInFrame = 0;
            CheckFallenPins = false;
        }

        public FrameRecord GetCurrentFrame() {
            return CurrentFrame;
        }
    }
}

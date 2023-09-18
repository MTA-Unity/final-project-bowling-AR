using System.Collections.Generic;
using Models;

namespace Classes
{
    public class Player
    {
        private List<FrameRecord> _frames; // List of frames for the player
        public string Name { get; set; }

        public Player(int numberOfFrames = 10)
        {
            _frames = new List<FrameRecord>(numberOfFrames);
        }

        public void RecordRollScore(int frameNumber, int accumulatedScore)
        {
            // Record the throw in the current frame
            // _frames[frameNumber - 1].SetRollScore(accumulatedScore);
        }
    }
}
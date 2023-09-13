using System.Collections.Generic;

namespace Classes
{
    public class Player
    {
        private List<Frame> _frames; // List of frames for the player

        public Player(int numberOfFrames)
        {
            _frames = new List<Frame>(numberOfFrames);
        }

        public void RecordThrow(int frameNumber, int throwNumber, int accumulatedScore)
        {
            // Record the throw in the current frame
            _frames[frameNumber - 1].SetThrow(throwNumber, accumulatedScore);
        }
    }
}
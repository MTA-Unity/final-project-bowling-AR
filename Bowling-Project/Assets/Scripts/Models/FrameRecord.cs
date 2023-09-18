using System.Collections.Generic;

namespace Models
{
    public class FrameRecord
    {
        private int _frameNumber;
        private int _currentRollNumber;
        private int _numberOfRollsInFrame;
        private List<RollRecord> _rolls; // Rolls record of a frame. Store the accumulated score of the throw

        public FrameRecord(int frameNumber)
        {
            _frameNumber = frameNumber;
            _currentRollNumber = 0;
            _numberOfRollsInFrame = 2;
            _rolls = new List<RollRecord>();
        }

        public Score GetRollScore(int rollNumber)
        {
            if (rollNumber <= _currentRollNumber && rollNumber > 0)
            {
                return _rolls[rollNumber - 1].Score;
            }
            
            // Invalid roll number
            return new Score(-1);
        }

        public void SetRoll(int fallenPins, Score score)
        {
            _currentRollNumber++;

            RollRecord newRoll = new RollRecord(_currentRollNumber, fallenPins, score);
            _rolls.Add(newRoll);
        }

        public int GetCurrentRollNumber()
        {
            return _currentRollNumber;
        }

        public int GetNumberOfRollsInFrame()
        {
            return _numberOfRollsInFrame;
        }

        public int GetCurrentFrameNumber()
        {
            return _frameNumber;
        }
        
        public void IncrementCurrentFrameNumber()
        {
            _frameNumber++;
        }
    }
}
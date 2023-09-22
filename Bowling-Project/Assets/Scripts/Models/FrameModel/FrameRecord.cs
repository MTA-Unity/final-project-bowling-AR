using System.Collections.Generic;
using Models;

namespace FrameModel
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
            _currentRollNumber = 1;
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
            RollRecord newRoll = new RollRecord(_currentRollNumber, fallenPins, score);
            _rolls.Add(newRoll);
        }
        
        public int GetNumberOfRollsInFrame()
        {
            return _numberOfRollsInFrame;
        }
        
        public void SetNumberOfRollsInFrame(int numberOfRoles)
        {
            _numberOfRollsInFrame = numberOfRoles;
        }
        
        public int GetCurrentRollNumber()
        {
            return _currentRollNumber;
        }
        
        public void IncrementCurrentRollNumber()
        {
            _currentRollNumber++;
        }

        public int GetCurrentFrameNumber()
        {
            return _frameNumber;
        }
    }
}
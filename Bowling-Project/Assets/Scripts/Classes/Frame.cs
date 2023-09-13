using System.Collections.Generic;

namespace Classes
{
    public class Frame
    {
        private List<int> _throws; // Throws record of a frame. Store the accumulated score of the throw

        public Frame()
        {
            _throws = new List<int>();
        }

        public int GetThrow(int throwIndex)
        {
            return _throws[throwIndex];
        }

        public void SetThrow(int throwIndex, int accumulatedScore)
        {
            _throws[throwIndex] = accumulatedScore;
        }
    }
}
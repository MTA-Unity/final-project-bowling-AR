using System.Collections.Generic;
using FrameModel;
using UnityEngine;

namespace Models
{
    public class Player
    {
        private FrameRecord[] _frames; // List of frames for the player's game
        public int _currentFrameOfPlayer;
        private string _name;

        public Player(string name, int numberOfFrames = 10)
        {
            _name = name;
            _frames = new FrameRecord[numberOfFrames];
            _currentFrameOfPlayer = 1;
        }

        public void SetName(string name)
        {
            _name = name;
        }
        
        public string GetName()
        {
            return _name;
        }

        public void SetFrameRecord(FrameRecord frame)
        {
            _frames[_currentFrameOfPlayer - 1] = frame;
            _currentFrameOfPlayer++;
        }
    }
}

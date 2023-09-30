using System;
using System.Collections.Generic;
using UnityEngine;
using Models;
using FrameModel;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private int _currentScore = 0;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {   
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    public void ResetScore()
    {
        _currentScore = 0;
    }

    public int GetCurrentScore()
    {
        return _currentScore;
    }

    public Score CalculateScoreOfRoll(int _pinsInFrameStart, int fallenPins, FrameRecord CurrentFrame) {
        int currentRollNumber = CurrentFrame.GetCurrentRollNumber();

        if (currentRollNumber == 1 && _pinsInFrameStart == fallenPins) { // strike
            return new Score(fallenPins, false, true);
        }

        if (currentRollNumber == 2 && CurrentFrame.GetRollScore(1).NumericScore + fallenPins == _pinsInFrameStart) { // spare
            return new Score(fallenPins, true, false);
        }

        return new Score(fallenPins, false, false); // regular roll
    }

    public int CalculateScoreOfPlayer(Player player, int framesNumber) {
        int sumScore = 0;
        
        int roll1Mul = 1;
        int roll2Mul = 1;

        for (int currentFrameNumber = 1; currentFrameNumber <= framesNumber; currentFrameNumber++) {
            FrameRecord currentFrame = player.GetFrameRecord(currentFrameNumber);
            Score roll1Score = currentFrame.GetRollScore(1);
            Score roll2Score = currentFrame.GetRollScore(2);

            sumScore = sumScore + roll1Mul * roll1Score.NumericScore + roll2Mul * roll2Score.NumericScore;
            if (roll1Score.Spare) {
                roll1Mul = 2;
                roll2Mul = 1;
            } else if (roll1Score.Strike) {
                roll1Mul = 2;
                roll2Mul = 2;
            } else {
                roll1Mul = 1;
                roll2Mul = 1;
            }
        }

        return sumScore;
    }
}

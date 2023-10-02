using System;
using System.Collections.Generic;
using System.Linq;
using FrameModel;
using Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BallController ball;
    [SerializeField] private GameObject gameFinishScreenManager;
    private int framesNumber = 3;
    private Player[] _players;
    private PinState[] _pinsStates;
    private BowlingPin[] pins;
    private GameStatus _gameStatus = new GameStatus();

    private int _pinsInFrameStart;
    private int fallenPinsAmount;
    public static GameManager Instance { get; private set; }

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
        _pinsInFrameStart = GameObject.FindGameObjectsWithTag("pin").Length;
        Debug.Log("Number of pins in game: " + _pinsInFrameStart);
        
        // Reset all pins states to steady
        _pinsStates = Enumerable.Repeat(PinState.Steady, _pinsInFrameStart).ToArray();
        SetAllBowlingPins();
        
        // Subscribe to game events
        GameEvents.Instance.ballReachedFinishEvent.AddListener(OnBallStoppedMoving);
        GameEvents.Instance.ballReachedFinishEvent.AddListener(OnBallReachedFinish);
        GameEvents.Instance.PinSwingingEvent += OnPinSwinging;
        GameEvents.Instance.PinSteadyEvent += OnPinSteady;
        GameEvents.Instance.PinFallenEvent += OnPinFallen;
    }

    private void Update()
    {
        if (_gameStatus.GameStarted) {
            PlayGame();
        }
    }
    
    private void OnDestroy()
    {
        // Subscribe to game events
        GameEvents.Instance.ballReachedFinishEvent.RemoveListener(OnBallStoppedMoving);
        GameEvents.Instance.ballReachedFinishEvent.RemoveListener(OnBallReachedFinish);
        
        GameEvents.Instance.PinSwingingEvent -= OnPinSwinging;
        GameEvents.Instance.PinSteadyEvent -= OnPinSteady;
        GameEvents.Instance.PinFallenEvent -= OnPinFallen;
    }

    public void ResetGame()
    {
        _gameStatus = new GameStatus();

        for (int i = 0; i < _players.Length; i++)
        {
            _players[i] = new Player(name = _players[i].GetName(), framesNumber);
            Debug.Log("Player " + i +": " + _players[i].GetName());
        }

        _gameStatus.GameStarted = true;
        Debug.Log("GameStarted  " + _gameStatus.GameStarted);
        Debug.Log("The player " + _players[0].GetName() + " has started playing");
        GameUIController.Instance.SetCurrentPlayerImage(0, _players[0].GetName().ToString());
    }

    private void PlayGame()
    {
        if (_gameStatus.CheckFallenPins)
        {
            if (AllPinsStoppedMoving())
            {
                _gameStatus.CheckFallenPins = false;
                fallenPinsAmount = _pinsStates.Count(state => state is PinState.Fallen);

                Debug.Log("All pins stopped moving, fallenPinsAmount: " + fallenPinsAmount);
                bool isLastRollInFrame = FinishRoll();

                // every frame-cycle has 1/2 rolls
                
                if (isLastRollInFrame)
                {
                    bool isLastPlayerInFrameCycle = FinishPlayerFrame();
                    
                    Debug.Log("isLastPlayerInFrameCycle: " + isLastPlayerInFrameCycle);
                    if (isLastPlayerInFrameCycle)
                    {
                        bool isLastFrameCycleInGame = ContinueToNextFrameCycle();
                        
                        Debug.Log("isLastFrameCycleInGame: " + isLastFrameCycleInGame);
                        if (isLastFrameCycleInGame)
                        {
                            FinishGame();
                        }
                    }
                    else
                    {
                        // Create new frame record with current frame number
                        _gameStatus.CurrentFrame = new FrameRecord(_gameStatus.CurrentFrame.GetCurrentFrameNumber());
                    }
                }
            }
        }
    }

    private bool AllPinsStoppedMoving()
    {
        // If all pins are steady or already fallen - finish roll
        return _pinsStates.All(state => state is PinState.Steady or PinState.Fallen);
    }

    public void SetPlayersNames(List<string> playersNames)
    {
        _players = new Player[playersNames.Count];
        for (int i = 0; i < playersNames.Count; i++)
        {
            _players[i] = new Player(name = playersNames[i], framesNumber);
        }

        for (int i = 0; i < playersNames.Count; i++)
        {
            Debug.Log("Player " + i +": " + _players[i].GetName());
        }

        _gameStatus.GameStarted = true;
        Debug.Log("GameStarted  " + _gameStatus.GameStarted);
        Debug.Log("The player " + _players[0].GetName() + " has started playing");
        GameUIController.Instance.SetCurrentPlayerImage(0, _players[0].GetName().ToString());
    }

    private void SetAllBowlingPins() {
        pins = FindObjectsOfType<BowlingPin>();
    }

    private bool FinishRoll()
    {
        bool isLastRollInFrame = true;
        _gameStatus.CheckFallenPins = false;
        Debug.Log("FinishRoll, Player "+ _gameStatus.CurrentPlayer);
        
        // Save the current roll data (score, fallen pins, etc)
        Score score = SaveRollData();

        // checking if it's strike
        if (score.Strike) {
            _gameStatus.CurrentFrame.SetNumberOfRollsInFrame(1);
        }

        GameUIController.Instance.SetFrameScoreText(_gameStatus.CurrentPlayer, _gameStatus.CurrentFrame.GetCurrentFrameNumber(), _gameStatus.CurrentFrame.GetCurrentRollNumber(), score.NumericScore);


        // If it's not the last roll - progress to next roll and return false
        if (_gameStatus.CurrentFrame.GetCurrentRollNumber() < _gameStatus.CurrentFrame.GetNumberOfRollsInFrame())
        {
            isLastRollInFrame = false;
            _gameStatus.CurrentFrame.IncrementCurrentRollNumber();
            
            // Clean the fallen pins and reset ball position
            CleanFallenPins();
            ball.Reset();
        }
        // Else - progress to next frame by returning true
        return isLastRollInFrame;
    }

    private Score SaveRollData()
    {
        Score score = ScoreManager.Instance.CalculateScoreOfRoll(_pinsInFrameStart, fallenPinsAmount, _gameStatus.CurrentFrame);
        Debug.Log("score: "+ score.NumericScore);
        _gameStatus.FallenPinsInFrame = fallenPinsAmount;
        _gameStatus.CurrentFrame.SetRoll(_gameStatus.FallenPinsInFrame, score);
        return score;
    }

    private void CleanFallenPins()
    {        
        foreach (var pin in pins)
        {
            if (_pinsStates[pin.pinNumber - 1] == PinState.Fallen)
            {        
                _pinsStates[pin.pinNumber - 1] = PinState.Steady;
                pin.gameObject.SetActive(false);
            }
        }
    }

    private void ResetAllPins()
    {
        _pinsStates = Enumerable.Repeat(PinState.Steady, _pinsInFrameStart).ToArray();
        
        foreach (var pin in pins)
        {       
            pin.gameObject.SetActive(true);
            pin.Reset();
        }
    }

    private bool FinishPlayerFrame()
    {
        bool isLastPlayerInFrameCycle = false;

        Debug.Log("current frame of player - "+  _players[_gameStatus.CurrentPlayer - 1].GetName() +" is : " + _players[_gameStatus.CurrentPlayer - 1]._currentFrameOfPlayer + "/" + framesNumber);
        
        // Save the player's frame and update the frame number
        _players[_gameStatus.CurrentPlayer - 1].SetFrameRecord(_gameStatus.CurrentFrame);

        int playerScore =  ScoreManager.Instance.CalculateScoreOfPlayer(_players[_gameStatus.CurrentPlayer - 1], _players[_gameStatus.CurrentPlayer - 1]._currentFrameOfPlayer - 1);

        Debug.Log("playerScore of Player - " + _players[_gameStatus.CurrentPlayer - 1].GetName() + " is: " + playerScore);

        GameUIController.Instance.SetTotalScoreText(_gameStatus.CurrentPlayer, _gameStatus.CurrentFrame.GetCurrentFrameNumber(), playerScore);

        _gameStatus.CurrentPlayer++;
        if (_gameStatus.CurrentPlayer > _players.Length)
        {
            isLastPlayerInFrameCycle = true;
        }
        else
        {
            Debug.Log("The player " + _players[_gameStatus.CurrentPlayer - 1].GetName() + " has started playing");
            GameUIController.Instance.SetCurrentPlayerImage(_gameStatus.CurrentPlayer - 1, _players[_gameStatus.CurrentPlayer - 1].GetName().ToString());

        }

        // Reset all pins and reset ball position
        ResetAllPins();
        ball.Reset();

        return isLastPlayerInFrameCycle;
    }

    private bool ContinueToNextFrameCycle()
    {
        bool isLastFrameCycleInGame = false;
        int nextFrameNum = _gameStatus.CurrentFrame.GetCurrentFrameNumber() + 1;
        
        Debug.Log("nextFrameNum: " + nextFrameNum);
        if (nextFrameNum <= framesNumber)
        {
            _gameStatus.CurrentPlayer = 1;
            Debug.Log("The player " + _players[_gameStatus.CurrentPlayer - 1].GetName() + " has started playing");
            _gameStatus.CurrentFrame = new FrameRecord(nextFrameNum);

            GameUIController.Instance.SetCurrentPlayerImage(_gameStatus.CurrentPlayer - 1, _players[_gameStatus.CurrentPlayer - 1].GetName().ToString());
        }
        else
        {
            isLastFrameCycleInGame = true;
        }
        
        return isLastFrameCycleInGame;
    }
    
    private void FinishGame()
    {
        _gameStatus.GameStarted = false;
        Debug.Log("FinishGame - GameStarted: " + _gameStatus.GameStarted);
        gameFinishScreenManager.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnBallReachedFinish() {
    }

    private void OnBallStoppedMoving() {
        Debug.Log("stopppp!!!");
        _gameStatus.CheckFallenPins = true;
        ball.StopSound();
    }

    private void OnPinSwinging(int pinNumber)
    {
        _gameStatus.CheckFallenPins = true;
        ball.StopSound();
        _pinsStates[pinNumber - 1] = PinState.Swinging;
    }
    
    private void OnPinSteady(int pinNumber)
    {
        _pinsStates[pinNumber - 1] = PinState.Steady;
    }
    
    private void OnPinFallen(int pinNumber)
    {
        _pinsStates[pinNumber - 1] = PinState.Fallen;
    }

    public string GetCurrentPlayer()
    {
        return _players[_gameStatus.CurrentPlayer -1].GetName();
    }
}


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
    private int framesNumber = 2;
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
        
        // TODO Liraz pin script
        GameEvents.Instance.PinSwingingEvent -= OnPinSwinging;
        GameEvents.Instance.PinSteadyEvent -= OnPinSteady;
        GameEvents.Instance.PinFallenEvent -= OnPinFallen;
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
    }

    private void SetAllBowlingPins() {
        pins = FindObjectsOfType<BowlingPin>();
    }

    private bool FinishRoll()
    {
        bool isLastRollInFrame = true;
        _gameStatus.CheckFallenPins = false;
        Debug.Log("FinishRoll, Player - "+ _gameStatus.CurrentPlayer);
        
        // Save the current roll data (score, fallen pins, etc)
        SaveRollData();
        
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

    private void SaveRollData()
    {
        // TODO Implement this method in ScoreManager
        // Score score = ScoreManager.Instance.CalculateScore();
        _gameStatus.FallenPinsInFrame = fallenPinsAmount;
        // _gameStatus.CurrentFrame.SetRoll(_gameStatus.FallenPinsInFrame, score); // --- TODO
    }

    private void CleanFallenPins()
    {        
        foreach (var pin in pins)
        {
            if (_pinsStates[pin.pinNumber - 1] == PinState.Fallen)
            {        
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

        _gameStatus.CurrentPlayer++;
        if (_gameStatus.CurrentPlayer > _players.Length)
        {
            isLastPlayerInFrameCycle = true;
        }
        else
        {
            Debug.Log("The player " + _players[_gameStatus.CurrentPlayer - 1].GetName() + " has started playing");
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
        }
        else
        {
            isLastFrameCycleInGame = true;
        }
        
        return isLastFrameCycleInGame;
    }
    
    private void FinishGame()
    {
        //TODO show on screen the results with a button to go to main menu
        _gameStatus.GameStarted = false;
        Debug.Log("FinishGame - GameStarted: " + _gameStatus.GameStarted);
        SceneManager.LoadScene("MainMenu");
    }

    private void OnBallReachedFinish() {
        // _gameStatus.CheckFallenPins = true;
        // If the ball reached the finish point (after the pins) without hitting any pin
        // if (!_gameStatus.CheckFallenPins)
        // {
        //     ball.StopSound();
        //     FinishRoll();
        // }
    }

    private void OnBallStoppedMoving() {
        Debug.Log("stopppp!!!");
        _gameStatus.CheckFallenPins = true;
        ball.StopSound();
    }

    private void OnPinSwinging(int pinNumber)
    {
        // _gameStatus.CheckFallenPins = true;
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
}


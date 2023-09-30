using System;
using System.Collections.Generic;
using System.Linq;
using FrameModel;
using Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int framesNumber = 10;
    [SerializeField] private BallController ball;
    
    private Player[] _players;
    private PinState[] _pinsStates;
    private GameStatus _gameStatus;

    private int _pinsInFrameStart;
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
        
        // Subscribe to game events
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
                int fallenAmount = _pinsStates.Count(state => state is PinState.Fallen);

                Debug.Log("All pins stopped moving");
                bool isLastRollInFrame = FinishRoll();
                
                Debug.Log("isLastRollInFrame: " + isLastRollInFrame);
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
            _players[i] = new Player(name = playersNames[i]);
        }

        for (int i = 0; i < playersNames.Count; i++)
        {
            Debug.Log("Player " + i +": " + _players[i].GetName());
        }

        _gameStatus.GameStarted = true;
        Debug.Log("GameStarted  " + _gameStatus.GameStarted);
        Debug.Log("The player " + _players[0].GetName() + " has started playing");
        GameUIController.Instance.ChangeCurrPlayerImage(0);
    }

    private bool FinishRoll()
    {
        bool isLastRollInFrame = true;
        _gameStatus.CheckFallenPins = false;
        Debug.Log("CheckFallenPins: " + _gameStatus.CheckFallenPins);
        
        // Save the current roll data (score, fallen pins, etc)
        SaveRollData();
        
        // If it's not the last roll - progress to next roll and return false
        if (_gameStatus.CurrentFrame.GetCurrentRollNumber() < _gameStatus.CurrentFrame.GetNumberOfRollsInFrame())
        {
            isLastRollInFrame = false;
            _gameStatus.CurrentFrame.IncrementCurrentRollNumber();
            Debug.Log("CurrentFrame" + _gameStatus.CurrentFrame);
            
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
        
        Debug.Log("FallenPinsInFrame: " + _gameStatus.FallenPinsInFrame);
        // _gameStatus.CurrentFrame.SetRoll(_gameStatus.FallenPinsInFrame, score);
    }

    private void CleanFallenPins()
    {
        // TODO implementation 
        // PinController[] pins = gameObject.GetComponents<PinController>;
        //
        // foreach (var pin in pins)
        // {
        //     if (pin.GetPinNumber() ==)
        //     {
        //         
        //     }
        // }
    }

    private void CleanAllPins()
    {
        // TODO implementation 
    }

    private bool FinishPlayerFrame()
    {
        bool isLastPlayerInFrameCycle = false;
        
        // Save the player's frame
        _players[_gameStatus.CurrentPlayer - 1].SetFrameRecord(_gameStatus.CurrentFrame);

        _gameStatus.CurrentPlayer++;
        if (_gameStatus.CurrentPlayer > _players.Length)
        {
            isLastPlayerInFrameCycle = true;
        }
        else
        {
            Debug.Log("The player " + _players[_gameStatus.CurrentPlayer - 1].GetName() + "has started playing");
        }
        
        // Clean all pins and reset ball position
        CleanAllPins();
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
        Debug.Log("GameStarted: " + _gameStatus.GameStarted);
        SceneManager.LoadScene("MainMenu");
    }

    private void OnBallReachedFinish()
    {
        _gameStatus.CheckFallenPins = true;
        // If the ball reached the finish point (after the pins) without hitting any pin
        if (!_gameStatus.CheckFallenPins)
        {
            ball.StopSound();
            FinishRoll();
        }
    }

    private void OnPinSwinging(int pinNumber)
    {
        _gameStatus.CheckFallenPins = true;
        Debug.Log("CheckFallenPins: " + _gameStatus.CheckFallenPins);
        ball.StopSound();
        _pinsStates[pinNumber - 1] = PinState.Swinging;
    }
    
    private void OnPinSteady(int pinNumber)
    {
        _pinsStates[pinNumber - 1] = PinState.Steady;
        // Debug.Log("steady!!!");
    }
    
    private void OnPinFallen(int pinNumber)
    {
        // Debug.Log("fallllll!!!");

        _pinsStates[pinNumber - 1] = PinState.Fallen;
        _gameStatus.FallenPinsInFrame++;
    }

    public string GetCurrentPlayer()
    {
        return _players[_gameStatus.CurrentPlayer].GetName();
    }
}


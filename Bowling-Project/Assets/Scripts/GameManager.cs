using System;
using System.Collections.Generic;
using System.Linq;
using Classes;
using Models;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int framesNumber = 10;
    [SerializeField] private BallControlState ball;
    
    private Player[] _players;
    private PinState[] _pinsStates;
    private GameStatus _gameStatus = new GameStatus();

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

    private void PlayGame()
    {
        if (_gameStatus.NextPlayerTurn)
        {
            _gameStatus.CurrentPlayer++;
            
            // There are no more players. Progress to next frame
            if (_gameStatus.CurrentPlayer > _players.Length)
            {
                // Last frame in game. End the game
                if (_gameStatus.CurrentFrame.GetCurrentFrameNumber() == framesNumber)
                {
                    _gameStatus.GameStarted = false;
                    
                    //TODO show on screen the results with a button to go to main menu
                }
                else
                {
                    
                }
                _gameStatus.CurrentPlayer = 1;
            }
        }
        
        if (_gameStatus.CheckFallenPins) // TODO - set CheckFallenPins to true also if the ball passed the finish line/stopped moving 
        {
            if (AllPinsStoppedMoving())
            {
                _gameStatus.CheckFallenPins = false;
                int a = _pinsStates.Count(state => state is PinState.Steady);
                int b = _pinsStates.Count(state => state is PinState.Fallen);
                Debug.Log("status Steady -" + a);
                Debug.Log("status Fallen -" + b);

                // TODO - Progress to next roll
                if (_gameStatus.CurrentFrame.GetCurrentRollNumber() > _gameStatus.CurrentFrame.GetNumberOfRollsInFrame())
                {
                       
                }

            }
        }
    }

    private bool AllPinsStoppedMoving()
    {
        // If all pins are steady or already fallen - finish roll
        return _pinsStates.All(state => state is PinState.Steady or PinState.Fallen);
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

    public void SetPlayersNames(List<string> playersNames)
    {
        _players = new Player[playersNames.Count];
        for (int i = 0; i < playersNames.Count; i++)
        {
            _players[i] = new Player { Name = playersNames[i] };
        }

        for (int i = 0; i < playersNames.Count; i++)
        {
            Debug.Log("Player " + i +": " + _players[i].Name);
        }

        _gameStatus.GameStarted = true;
    }

    private void OnBallReachedFinish()
    {
        Debug.Log("Finish line ");
        _gameStatus.CheckFallenPins = true;
    }

    private void OnPinSwinging(int pinNumber)
    {
        // Debug.Log("swinnnggggg!!!!!!!!!!###!");

        // if (!_gameStatus.CheckFallenPins)
        // {
            _gameStatus.CheckFallenPins = true;
        // }

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
        // _gameStatus.CurrentFrame.SetRoll();
    }
}


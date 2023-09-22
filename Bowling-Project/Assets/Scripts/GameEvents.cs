using UnityEngine;
using UnityEngine.Events;

public class GameEvents: MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

    public UnityEvent playersNamesSetEvent = new UnityEvent();
    public UnityEvent ballReachedFinishEvent = new UnityEvent();
    
    // New delegate and event for an event with a string parameter
    public delegate void IntParamEventHandler(int value);
    public event IntParamEventHandler PinSwingingEvent;
    public event IntParamEventHandler PinSteadyEvent;
    public event IntParamEventHandler PinFallenEvent;

    
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

    public void TriggerPlayersNamesSetEvent()
    {
        playersNamesSetEvent?.Invoke();
    }
    
    public void TriggerBallReachedFinishEvent()
    {
        ballReachedFinishEvent?.Invoke();
    }
    
    public void TriggerPinSwingingEvent(int pinNumber)
    {
        PinSwingingEvent?.Invoke(pinNumber);
    }
    
    public void TriggerPinSteadyEvent(int pinNumber) 
    {
        PinSteadyEvent?.Invoke(pinNumber);
    }
    
    public void TriggerPinFallenEvent(int pinNumber)
    {
        PinFallenEvent?.Invoke(pinNumber);
    }
}

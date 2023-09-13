using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    
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
    
    
}

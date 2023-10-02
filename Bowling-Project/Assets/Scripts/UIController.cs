using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private UiInputNamesWindow inputWindow;
    [SerializeField] private Image canvasBackground;
    [SerializeField] private GameObject MainMenuOptions;
    [SerializeField] private GameObject GameUIConatiner;

    void Start()
    {
        // Subscribe to game events
        GameEvents.Instance.playersNamesSetEvent.AddListener(OnPlayersNamesSet);
        inputWindow.Hide();
        GameUIConatiner.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log("start game");
        MainMenuOptions.SetActive(false);
        inputWindow.Show();
    }

    public void ExitGame()
    {
        // Exit the game in in editor and in application
        Debug.Log("Game closed");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CancelInputWindow()
    {
        MainMenuOptions.SetActive(true);
        inputWindow.Hide();
    }

    
    private void OnDestroy()
    {
        GameEvents.Instance.playersNamesSetEvent.RemoveListener(OnPlayersNamesSet);
    }

    private void OnPlayersNamesSet()
    {
        inputWindow.Hide();
        canvasBackground.gameObject.SetActive(false);
        GameUIConatiner.SetActive(true);
    }

   
}

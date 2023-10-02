using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFinishScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject gameFinishScreen;
    [SerializeField] private Button mainMenuButton;
    
    private void Start()
    {
        gameFinishScreen.SetActive(false);
    }
    
    public void GoToMainMenu()
    {
        // Resume game pause and go to main menu
        Time.timeScale = 1f;
        SceneManager.LoadScene("BallTest");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        gameFinishScreen.SetActive(false);
        GameManager.Instance.ResetGame();
        ScoreManager.Instance.ResetScore();
        GameUIController.Instance.ResetScoreResult();
    }

}

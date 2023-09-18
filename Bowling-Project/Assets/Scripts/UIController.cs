using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    [SerializeField] private UiInputNamesWindow inputWindow;
    [SerializeField] private Image canvasBackground;

    void Start()
    {
        // Subscribe to game events
        GameEvents.Instance.playersNamesSetEvent.AddListener(OnPlayersNamesSet);
        inputWindow.Show();
    }

    private void OnDestroy()
    {
        GameEvents.Instance.playersNamesSetEvent.RemoveListener(OnPlayersNamesSet);
    }

    private void OnPlayersNamesSet()
    {
        inputWindow.Hide();
        canvasBackground.gameObject.SetActive(false);
    }
}

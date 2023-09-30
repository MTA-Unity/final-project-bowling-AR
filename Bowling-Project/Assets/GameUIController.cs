using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameUIController : MonoBehaviour
{

    /// <summary>
    /// Sound
    /// </summary>
    [SerializeField] private List<Sprite> SoundSpriteList;
    private int _curSoundSpriteID = 0;
    private Image _imageSoundComp;
    private GameObject _imageSoundCompGameObject;
    private bool _isSoundEnable = true;

    /// <summary>
    /// Player Turn
    /// </summary>
    [SerializeField] private List<Sprite> CurrentPlayerSpriteList;
    private int _currPlayerSpriteID = 0;
    private Image _imageCurrPlayerComp;
    private GameObject _imageCurrPlayerCompGameObject;
    private GameObject _PlayerTextGameObject;
    [SerializeField] private TextMeshProUGUI PlayerText;

    public static GameUIController Instance { get; private set; }


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

    void Start()
    {
        SetFirstSoundImage();
    }


    public void ChangeSoundImage()
    {
        _imageSoundCompGameObject = GameObject.FindWithTag("Sound");

        if (_imageSoundCompGameObject != null)
        {
            _imageSoundComp = _imageSoundCompGameObject.GetComponent<Image>();
        }


        _curSoundSpriteID++;
        if (_curSoundSpriteID == SoundSpriteList.Count)
        {
            Debug.Log("Sound unmute");
            _isSoundEnable = true;
            _curSoundSpriteID = 0;
        }
        else
        {
            Debug.Log("Sound mute");
            _isSoundEnable = false;        }
        _imageSoundComp.sprite = SoundSpriteList[_curSoundSpriteID];
    }

    private void SetFirstSoundImage()
    {
        _imageSoundCompGameObject = GameObject.FindWithTag("Sound");

        if (_imageSoundCompGameObject != null)
        {
            _imageSoundComp = _imageSoundCompGameObject.GetComponent<Image>();
            _imageSoundComp.sprite = SoundSpriteList[0];
        }
    }

    public void ChangeCurrPlayerImage(int playerID)
    {
        _imageCurrPlayerCompGameObject = GameObject.FindWithTag("PlayerTurn");

        if (_imageCurrPlayerCompGameObject != null)
        {
            _imageCurrPlayerComp = _imageCurrPlayerCompGameObject.GetComponent<Image>();
            _imageCurrPlayerComp.sprite = CurrentPlayerSpriteList[playerID];
            updateCurrentPlayerText();
        }
    }

    private void updateCurrentPlayerText()
    {
        _PlayerTextGameObject = GameObject.FindWithTag("PlayerText");

        if (_PlayerTextGameObject != null)
        {
            PlayerText = _PlayerTextGameObject.GetComponent<TextMeshProUGUI>();
            PlayerText.text = GameManager.Instance.GetCurrentPlayer().ToString();
        }
    }

    private void SetFirstCurrPlayerImage()
    {

        _imageCurrPlayerCompGameObject = GameObject.FindWithTag("Player");

        if (_imageCurrPlayerCompGameObject != null)
        {
            _imageCurrPlayerComp = _imageCurrPlayerCompGameObject.GetComponent<Image>();
            _imageCurrPlayerComp.sprite = CurrentPlayerSpriteList[0];
            updateCurrentPlayerText();
        }
    }
}

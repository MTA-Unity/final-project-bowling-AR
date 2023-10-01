using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameUIController : MonoBehaviour
{

    // Sound
    [SerializeField] private List<Sprite> SoundSpriteList;
    private int _curSoundSpriteID = 0;
    private Image _imageSoundComp;
    private GameObject _imageSoundCompGameObject;
    private bool _isSoundEnable = true;

    /// Player Turn
    [SerializeField] private List<Sprite> CurrentPlayerSpriteList;
    private Image _imageCurrPlayerComp;
    private GameObject _imageCurrPlayerCompGameObject;
    private GameObject _PlayerTextGameObject;
    private TextMeshProUGUI PlayerText;


    // Score Table
    [SerializeField] private List<TextMeshProUGUI> scoreFrameResultListPlayer;
    [SerializeField] private List<TextMeshProUGUI> scoreTotalResultListPlayer;
    private TextMeshProUGUI frameScoreText;
    private TextMeshProUGUI totalScoreText;


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
        ResetSoundImage();
    }


    public void ChangeSoundImage()
    {
        _imageSoundCompGameObject = GameObject.FindWithTag("Sound");

        if (_imageSoundCompGameObject != null)
        {
            _imageSoundComp = _imageSoundCompGameObject.GetComponent<Image>();

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
                _isSoundEnable = false;
            }
            _imageSoundComp.sprite = SoundSpriteList[_curSoundSpriteID];
        }
    }

    private void ResetSoundImage()
    {
        _imageSoundCompGameObject = GameObject.FindWithTag("Sound");

        if (_imageSoundCompGameObject != null)
        {
            _imageSoundComp = _imageSoundCompGameObject.GetComponent<Image>();
            _imageSoundComp.sprite = SoundSpriteList[0];
        }
    }

    public void SetCurrentPlayerImage(int playerID)
    {
        _imageCurrPlayerCompGameObject = GameObject.FindWithTag("PlayerTurn");

        if (_imageCurrPlayerCompGameObject != null)
        {
            _imageCurrPlayerComp = _imageCurrPlayerCompGameObject.GetComponent<Image>();
            _imageCurrPlayerComp.sprite = CurrentPlayerSpriteList[playerID];
            SetCurrentPlayerText();
        }
    }

    private void SetCurrentPlayerText()
    {
        _PlayerTextGameObject = GameObject.FindWithTag("PlayerText");

        if (_PlayerTextGameObject != null)
        {
            PlayerText = _PlayerTextGameObject.GetComponent<TextMeshProUGUI>();
            PlayerText.text = GameManager.Instance.GetCurrentPlayer().ToString();
        }
    }


    public void ResetScoreResult()
    {
        for (int i= 0; i< scoreFrameResultListPlayer.Count; i++)
        {
            frameScoreText = scoreFrameResultListPlayer[i].GetComponent<TextMeshProUGUI>();
            frameScoreText.text = "";
        }

        for (int i = 0; i < scoreTotalResultListPlayer.Count; i++)
        {
            totalScoreText = scoreTotalResultListPlayer[i].GetComponent<TextMeshProUGUI>();
            totalScoreText.text = "";
        }
    }


    public void SetFrameScoreText(int currentPlayer, int frameNumber, int rollNumber, int score)
    {
        var scoreText = "";
        int frameIndex = frameNumber;

        if (currentPlayer == 2)
        {
            frameIndex = frameNumber + 3;
        }

        frameScoreText = scoreFrameResultListPlayer[frameIndex].GetComponent<TextMeshProUGUI>();

        if (rollNumber == 1) 
        {
            if (score == 10)
            {
                scoreText = "10/-";
            }
            else
            {
                scoreText = score.ToString() + "/";
            }
        }
        else
        {
            scoreText = frameScoreText.text.ToString() + " " + score.ToString();
        }

        frameScoreText.text = scoreText;

    }


    public void SetTotalScoreText(int currentPlayer, int frameNumber, int score)
    {

        int frameIndex = frameNumber;
        int totalFrameIndex = 0; // The last index in list is total result of player2

        if (currentPlayer == 2)
        {
            frameIndex += 3;
            totalFrameIndex = scoreTotalResultListPlayer.Count -1; // The last index in list is total result of player2
        }

        totalScoreText = scoreTotalResultListPlayer[frameIndex].GetComponent<TextMeshProUGUI>();
        totalScoreText.text = score.ToString();
        totalScoreText = scoreTotalResultListPlayer[totalFrameIndex].GetComponent<TextMeshProUGUI>();
        totalScoreText.text = score.ToString();

    }

    public bool IsAudioEnable()
    {
        return _isSoundEnable;
    }

    public void SetAudioEnable(bool audio)
    {
        _isSoundEnable = audio;
    }
}

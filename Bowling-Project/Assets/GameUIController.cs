using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    //[SerializeField] private List<Sprite> PlayerTurnSpriteList;
    //private int _curPlayerTurnSpriteID = 0;
    //private Image _imagePlayerTurnComp;
    //private GameObject _imagePlayerTurnCompGameObject;

    void Start()
    {
        SetSoundImage();
    }


    public void ChangeSoundImage()
    {
        _imageSoundCompGameObject = GameObject.FindWithTag("Sound");

        if (_imageSoundCompGameObject != null)
        {
            _imageSoundComp = _imageSoundCompGameObject.GetComponent<Image>();
            Debug.Log(_imageSoundComp);
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

    private void SetSoundImage()
    {
        Debug.Log("SetImage " + _isSoundEnable);

        _imageSoundCompGameObject = GameObject.FindWithTag("Sound");

        if (_imageSoundCompGameObject != null)
        {
            _imageSoundComp = _imageSoundCompGameObject.GetComponent<Image>();

            if (_isSoundEnable)
            {
                _curSoundSpriteID = 0;
            }
            else
            {
                _curSoundSpriteID = 1;
            }
            _imageSoundComp.sprite = SoundSpriteList[_curSoundSpriteID];
        }
    }
}

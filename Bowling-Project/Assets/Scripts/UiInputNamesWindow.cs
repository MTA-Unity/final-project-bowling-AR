using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiInputNamesWindow : MonoBehaviour
{
    [SerializeField] private Button okBtn;
    [SerializeField] private Button cancelBtn;
    private TMP_InputField[] _inputFieldsPlayers;

    private List<string> _playersNames = new List<string>();
    private int _maxPlayers;
    
    private void Awake()
    {
        _inputFieldsPlayers = GetComponentsInChildren<TMP_InputField>();
        _maxPlayers = _inputFieldsPlayers.Length;
        Hide();
    }

    private void Start()
    {
        okBtn.onClick.AddListener(OnOkClick);
    }

    void OnDestroy()
    {
        okBtn.onClick.RemoveListener(OnOkClick);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnOkClick()
    {
        // Loop over all the inputs and pass over to the GameManager all the not empty inputs
        for (int i = 0; i < _maxPlayers; i++)
        {
            string playerName = _inputFieldsPlayers[i].text;

            if (!string.IsNullOrEmpty(playerName) && !string.IsNullOrWhiteSpace(playerName))
            {
                _playersNames.Add(playerName);
            }
        }

        if (_playersNames.Count > 0)
        {
            GameManager.Instance.SetPlayersNames(_playersNames);
            GameEvents.Instance.TriggerPlayersNamesSetEvent();
        }
    }
}

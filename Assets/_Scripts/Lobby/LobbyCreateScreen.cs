using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LobbyCreateScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_Dropdown _maxPlayersDropDown;

    public static event Action<LobbyData> LobbyCreated;

    private void Start()
    {
        SetOptions(_maxPlayersDropDown, Constants.MaxPlayers);

        void SetOptions(TMP_Dropdown dropdown, IEnumerable<string> values)
        {
            dropdown.options = values.Select(type => new TMP_Dropdown.OptionData { text = type }).ToList();
        }
    }

    public void OnCreateButtonClicked()
    {
        var lobbyData = new LobbyData()
        {
            Name = _nameInput.text,
            MaxPlayers = int.Parse(_maxPlayersDropDown.name)
        };

        LobbyCreated?.Invoke(lobbyData);
    }
}

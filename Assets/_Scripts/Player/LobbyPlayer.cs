using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _readyText;

    public void UpdateLobbyPlayer(LobbyPlayerData player)
    {
        _playerName.text = player.Name;
        UpdateReady(player.IsReady);
    }

    private void UpdateReady(bool isReady)
    {
        if(isReady)
        {
            _readyText.text = "Ready";
            _readyText.color = Color.green;
        }
        else
        {
            _readyText.text = "Waiting";
            _readyText.color = Color.red;
        }
    }
}

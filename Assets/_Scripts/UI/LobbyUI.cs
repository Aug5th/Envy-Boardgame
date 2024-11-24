using lobby;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _startButton;

    [SerializeField] TextMeshProUGUI textLobbyCode;
    [SerializeField] private List<LobbyPlayer> lobbyPlayers;

    private void OnEnable()
    {
        LobbyEvent.OnLobbyUpdated += UpdateLobbyUI;

        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        _cancelButton.onClick.AddListener(OnCancelButtonClicked);
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnDisable()
    {
        LobbyEvent.OnLobbyUpdated -= UpdateLobbyUI;

        _readyButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();
        _startButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        textLobbyCode.text = $"Lobby Code: {LobbyManager.Instance.GetLobbyCode()}";
    }

    private async void OnReadyButtonClicked()
    {
        await LobbyManager.Instance.UpdateLobbyReadyStatus(true);
    }

    private async void OnCancelButtonClicked()
    {
        await LobbyManager.Instance.UpdateLobbyReadyStatus(false);
    }

    private void OnStartButtonClicked()
    {
        Debug.Log("Start button clicked");
    }

    private void UpdateLobbyUI()
    {
        var players = LobbyManager.Instance.Players;
        if (players != null)
        {
            int i = 0;
            foreach (var player in players)
            {
                lobbyPlayers[i].gameObject.SetActive(true);
                lobbyPlayers[i].UpdateLobbyPlayer(player);
                i++;
            }
        }

        UpdateButtonUI();

    }

    private void UpdateButtonUI()
    {
        var player = LobbyManager.Instance.CurrentPlayer;
        if (player == null)
        {
            return;
        }

        if (player.GameTag.Equals("Member"))
        {
            _startButton.gameObject.SetActive(false);
            _readyButton.gameObject.SetActive(!player.IsReady);
            _cancelButton.gameObject.SetActive(player.IsReady);
        }
        else if (player.GameTag.Equals("Host") && LobbyManager.Instance.IsLoobyReady)
        {
            Debug.Log("Lobby not ready");
            _startButton.gameObject.SetActive(true);
            _readyButton.gameObject.SetActive(false);
            _cancelButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Lobby ready");
            _startButton.gameObject.SetActive(false);
            _readyButton.gameObject.SetActive(false);
            _cancelButton.gameObject.SetActive(false);
        }
    }

    private void ReadyToStartLobby()
    {
        var player = LobbyManager.Instance.CurrentPlayer;
        if (player != null && player.GameTag.Equals("Host"))
        {
            _startButton.gameObject.SetActive(true);
            _readyButton.gameObject.SetActive(false);
            _cancelButton.gameObject.SetActive(false);
        }
    }
}

using lobby;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : PresistentSingleton<MenuManager>
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _joinMenu;

    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _joinAcceptButton;

    [SerializeField] private TextMeshProUGUI _textLobby;

    private void Start()
    {
        _hostButton.onClick.AddListener(OnHostButtonClicked);
        _joinButton.onClick.AddListener(OnJoinButtonClicked);
        _joinAcceptButton.onClick.AddListener(OnJoinAcceptButtonClicked);
    }

    public async void OnHostButtonClicked()
    {
        if (await CreateLobby())
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    public void OnJoinButtonClicked()
    {
        _mainMenu.SetActive(false);
        _joinMenu.SetActive(true);
    }

    private async void OnJoinAcceptButtonClicked()
    {

        if (await JoinLobby())
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    private async Task<bool> CreateLobby()
    {
        LobbyPlayerData lobbyPlayerData = new(AuthenticationService.Instance.PlayerId, PlayerPrefs.GetString("username"), "Host");
        lobbyPlayerData.SetIsReady(true);
        bool succeeded = await LobbyManager.Instance.CreateLobby("Lobby", 4, lobbyPlayerData.GetData(), true);
        return succeeded;
    }

    private async Task<bool> JoinLobby()
    {
        LobbyPlayerData lobbyPlayerData = new(AuthenticationService.Instance.PlayerId, PlayerPrefs.GetString("username"), "Member");
        string lobbyCode = _textLobby.text;
        lobbyCode = lobbyCode.Substring(0, lobbyCode.Length - 1).ToUpper();

        bool succeeded = await LobbyManager.Instance.JoinLobby(lobbyCode, lobbyPlayerData.GetData());
        return succeeded;
    }


}

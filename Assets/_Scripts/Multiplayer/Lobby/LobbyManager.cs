using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace lobby
{
    public class LobbyManager : PresistentSingleton<LobbyManager>
    {
        private static Lobby _lobby;
        private LobbyPlayerData _currentPlayer;
        private List<LobbyPlayerData> _players;
        private bool _isLobbyReady;

        private Coroutine _heartbeatCoroutine;
        private Coroutine _refreshCoroutine;

        private CancellationTokenSource _updateLobbySource;

        public LobbyPlayerData CurrentPlayer => _currentPlayer;
        public List<LobbyPlayerData> Players => _players;
        public bool IsLoobyReady => _isLobbyReady;
        private void UpdateLobby()
        {
            int readyCount = 0;
            var playerDataList = GetPlayerDataList();
            _players = new List<LobbyPlayerData>();
            foreach (var playerData in playerDataList)
            {
                LobbyPlayerData player = new LobbyPlayerData(playerData);
                if (string.Equals(player.Id, AuthenticationService.Instance.PlayerId))
                {
                    _currentPlayer = player;
                }
                
                if(player.IsReady)
                {
                    readyCount++;
                }

                _players.Add(player);
            }

            _isLobbyReady = readyCount >= _players.Count;
            LobbyEvent.OnLobbyUpdated?.Invoke();   
        }

        private List<Dictionary<string, PlayerDataObject>> GetPlayerDataList()
        {
            List<Dictionary<string, PlayerDataObject>> playerDataList = new List<Dictionary<string, PlayerDataObject>>();

            foreach (var player in _lobby.Players)
            {
                playerDataList.Add(player.Data);
            }

            return playerDataList;
        }

        public async Task<bool> CreateLobby(string name, int maxPlayers, Dictionary<string, string> data, bool isPrivate)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);

            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = isPrivate,
                Player = player
            };

            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers, options);
                UpdateLobby();
            }
            catch (System.Exception)
            {

                return false;
            }

            _heartbeatCoroutine = StartCoroutine(LobbyHearbeatCoroutine(_lobby.Id, 6f));
            _refreshCoroutine = StartCoroutine(LobbyRefresh(_lobby.Id, 1f));

            return true;
        }

        public async Task<bool> JoinLobby(string lobbyCode, Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);

            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions()
            {
                Player = player
            };

            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
                UpdateLobby();
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error : {e.Message}");
                return false;
            }

            _refreshCoroutine = StartCoroutine(LobbyRefresh(_lobby.Id, 1f));
            return true;
        }

        public async Task<bool> UpdateLobbyReadyStatus(bool isReady)
        {
            _currentPlayer.SetIsReady(isReady);
            return await UpdateLobbyPlayer();
        }

        public async Task<bool> UpdateLobbyPlayer()
        {
           
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(_currentPlayer.GetData());
            UpdatePlayerOptions options = new UpdatePlayerOptions()
            {
                Data = playerData
            };

            try
            {
                _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, _currentPlayer.Id, options);
                UpdateLobby();
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error : {e.Message}");
                return false;
            }

            return true;
        }

        public string GetLobbyCode()
        {
            return _lobby.LobbyCode;
        }

        IEnumerator LobbyHearbeatCoroutine(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Debug.Log("Lobby heartbeat");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return new WaitForSeconds(waitTimeSeconds);
            }
        }

        IEnumerator LobbyRefresh(string lobbyId, float waitTimeSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);
                Lobby newLobby = task.Result;
                _updateLobbySource = new CancellationTokenSource();

                if (newLobby.LastUpdated > _lobby.LastUpdated)
                {
                    //Debug.Log("Lobby updated by last updated");
                    _lobby = newLobby;
                    UpdateLobby();
                }
                else if (!_updateLobbySource.IsCancellationRequested && _lobby != null) // TODO : Need recheck 
                {
                    //Debug.Log("Lobby updated by cancellation requested");
                    _lobby = newLobby;
                    UpdateLobby();
                }
                yield return new WaitForSeconds(waitTimeSeconds);
            }
        }

        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {

            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();
            foreach (var (key, value) in data)
            {
                playerData.Add(key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value));
            }

            return playerData;
        }

        protected override void OnApplicationQuit()
        {
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            }
            base.OnApplicationQuit();
        }
    }
}




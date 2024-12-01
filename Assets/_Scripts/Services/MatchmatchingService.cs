using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using Object = UnityEngine.Object;

public static class MatchmatchingService
{
    private const int HeartbeatInterval = 15;
    private const int LobbyRefreshRate = 2; // Rate limits at 2

    private static Lobby _currentLobby;
    private static UnityTransport _transport;
    private static CancellationTokenSource _heartbeatResource, _updateLobbyResource;

    private static UnityTransport Transport
    {
        get => _transport != null ? _transport : _transport = Object.FindObjectOfType<UnityTransport>();
        set => _transport = value;
    }

    public static event Action<Lobby> CurrentLobbyRefreshed;

    public static async Task<List<Lobby>> GetLobbies(int numberOfLobbies)
    {
        var options = new QueryLobbiesOptions
        {
            Count = numberOfLobbies,
            Filters = new List<QueryFilter>
            {
                new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                new(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ)
            }
        };

        var allLobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
        return allLobbies.Results;
    }

    public static async Task CreateLobbyWithAllocation(LobbyData data)
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(data.MaxPlayers);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        var options = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { Constants.JoinCode , new DataObject(DataObject.VisibilityOptions.Public,joinCode) }
            }
        };

        _currentLobby = await Lobbies.Instance.CreateLobbyAsync(data.Name, data.MaxPlayers, options);
        Transport.SetHostRelayData(allocation.RelayServer.IpV4
            , (ushort)allocation.RelayServer.Port
            , allocation.AllocationIdBytes
            , allocation.Key
            , allocation.ConnectionData);

        Heartbeat();
        PeriodicallyRefreshLobby();
    }

    public static async Task JoinLobbyWithAllocation(string lobbyId)
    {
        _currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
        var allocation = await RelayService.Instance.JoinAllocationAsync(_currentLobby.Data[Constants.JoinCode].Value);

        Transport.SetClientRelayData(allocation.RelayServer.IpV4
           , (ushort)allocation.RelayServer.Port
           , allocation.AllocationIdBytes
           , allocation.Key
           , allocation.ConnectionData
           , allocation.HostConnectionData);

        PeriodicallyRefreshLobby();
    }

    public static async Task LockLobby()
    {
        try
        {
            await Lobbies.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions { IsLocked = true });
        }
        catch (Exception e)
        {

            Debug.Log($"Fail closing lobby {e.Message}");
        }
    }

    public static async Task LeaveLobby()
    {
        _heartbeatResource?.Cancel();
        _updateLobbyResource?.Cancel();

        if (_currentLobby != null)
        {
            try
            {
                if (_currentLobby.HostId == Authentication.PlayerId)
                {
                    await Lobbies.Instance.DeleteLobbyAsync(_currentLobby.Id);
                }
                else
                {
                    await Lobbies.Instance.RemovePlayerAsync(_currentLobby.Id, Authentication.PlayerId);
                }
                _currentLobby = null;
            }
            catch (Exception e)
            {
                Debug.Log($"Fail leaving lobby {e.Message}");
            }
        }
    }

    private static async void PeriodicallyRefreshLobby()
    {
        _updateLobbyResource = new CancellationTokenSource();
        await Task.Delay(LobbyRefreshRate * 1000);
        while (!_updateLobbyResource.IsCancellationRequested && _currentLobby != null)
        {
            _currentLobby = await Lobbies.Instance.GetLobbyAsync(_currentLobby.Id);
            CurrentLobbyRefreshed?.Invoke(_currentLobby);
            await Task.Delay(LobbyRefreshRate * 1000);
        }
    }

    private static async void Heartbeat()
    {
        _heartbeatResource = new CancellationTokenSource();
        while (!_heartbeatResource.IsCancellationRequested & _currentLobby != null)
        {
            await Lobbies.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            await Task.Delay(HeartbeatInterval * 1000);
        }
    }
}

public struct LobbyData
{
    public string Name;
    public int MaxPlayers;
}
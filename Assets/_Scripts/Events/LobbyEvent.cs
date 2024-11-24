using Unity.Services.Lobbies.Models;

public static class LobbyEvent
{
    public delegate void LobbyUpdated();
    public static LobbyUpdated OnLobbyUpdated;

    public delegate void LobbyReady();
    public static LobbyReady OnLobbyReady;
}

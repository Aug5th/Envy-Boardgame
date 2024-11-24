using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyPlayerData
{
    private string _id;
    private string _name;
    private string _gameTag;
    private bool _isReady;

    public string Id => _id;
    public string Name => _name;
    public string GameTag => _gameTag;
    public bool IsReady => _isReady;

    public LobbyPlayerData(string id, string name, string gameTag)
    {
        _id = id;
        _name = name;
        _gameTag = gameTag;
    }

    public LobbyPlayerData(Dictionary<string,PlayerDataObject> data) 
    { 
        UpdateData(data);
    }

    public void SetIsReady(bool isReady)
    {
        _isReady = isReady;
    }

    public Dictionary<string, string> GetData()
    {
        return new Dictionary<string, string>()
        {
            {"Id", _id },
            {"Name", _name},
            {"GameTag", _gameTag },
            {"IsReady", _isReady.ToString() }
        };
    }

    public void UpdateData(Dictionary<string, PlayerDataObject> data)
    {
        if (data.ContainsKey("Id"))
        {
            _id = data["Id"].Value;
        }

        if (data.ContainsKey("Name"))
        {
            _name = data["Name"].Value;
        }

        if (data.ContainsKey("GameTag"))
        {
            _gameTag = data["GameTag"].Value;
        }

        if (data.ContainsKey("IsReady"))
        {
            _isReady = data["IsReady"].Value == "True";
        }

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private NetworkManager _networkMgr;

    private void Awake()
    {
        _networkMgr = GetComponent<NetworkManager>();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (_networkMgr.IsClient && !_networkMgr.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabel();
        }

        GUILayout.EndArea();
    }

    private void StatusLabel()
    {
        var mode = _networkMgr.IsHost ?
                "Host" : _networkMgr.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            _networkMgr.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    private void StartButtons()
    {
        if (GUILayout.Button("Host")) _networkMgr.StartHost();
        if (GUILayout.Button("Client")) _networkMgr.StartClient();
        if (GUILayout.Button("Server")) _networkMgr.StartServer();
    }
}

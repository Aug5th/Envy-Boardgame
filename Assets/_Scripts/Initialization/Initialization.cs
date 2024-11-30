using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initialization : MonoBehaviour
{
    private void Awake()
    {
        LoadLoadingScene();
    }
    async void Start()
    {
        await Authentication.LoginAnonymous();
        SceneManager.LoadScene("Menu");
    }

    private void LoadLoadingScene()
    {
        if (!SceneManager.GetSceneByName("Loading").isLoaded)
        {
            SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        }
    }
}
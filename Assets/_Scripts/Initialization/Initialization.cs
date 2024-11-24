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
        await UnityServices.InitializeAsync();

        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            AuthenticationService.Instance.SignedIn += OnSignedIn;

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn)
            {
                string userName = PlayerPrefs.GetString("username");
                if (userName == "")
                {
                    userName = $"Player-{Random.Range(1, 1000)}";
                    PlayerPrefs.SetString("username", userName);
                }

                SceneManager.LoadScene("Menu");
            }
        }
    }

    private void LoadLoadingScene()
    {
        if (!SceneManager.GetSceneByName("Loading").isLoaded)
        {
            SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);

        }
    }
    private void OnSignedIn()
    {
        Debug.Log($"AccessToken : {AuthenticationService.Instance.AccessToken}");
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunnerManager : MonoBehaviour
{
    [Header("LoadingCanvas")]
    public GameObject LoadingCanvas;

    [Header("Prefabs")]
    public NetworkRunner runnerPrefab;
    private NetworkRunner runnerInstance;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Applicarion functions
    public void QuitGame()
    {
        Debug.Log("Quitting the game");
        Application.Quit();
        if (Application.isEditor) UnityEditor.EditorApplication.isPlaying = false;
    }

    // Network functions
    public async void StartSharedServer(string sessionName)
    {
        if (!!this.LoadingCanvas)
        {
            this.LoadingCanvas.SetActive(true);
        }
        Debug.Log("Starting a shared server");
        if (!!this.runnerInstance) await this.Disconnect();

        this.runnerInstance = Instantiate(this.runnerPrefab);

        var events = this.runnerInstance.GetComponent<NetworkEvents>();
        events.OnShutdown.AddListener(OnShutdown);
        events.PlayerJoined.AddListener((runner, player) =>
        {
            Debug.Log("Player joined: " + player);
        });

        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(1));

        var startTask = this.runnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 8,
            SessionProperties = new Dictionary<string, SessionProperty>() { ["GameMode"] = sessionName },
            SceneManager = this.runnerInstance.GetComponent<NetworkSceneManagerDefault>(),
        });
        await startTask;

        if (startTask.Result.Ok)
        {
            if (!!this.LoadingCanvas)
            {
                this.LoadingCanvas.SetActive(false);
            }
            Debug.Log("Server started successfully");
            await this.runnerInstance.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
        }
        else
        {
            if (!!this.LoadingCanvas)
            {
                this.LoadingCanvas.SetActive(false);
            }
            Debug.LogError("Failed to start server: " + startTask.Result.ShutdownReason);
        }
    }

    private void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public async Task Disconnect()
    {
        Debug.Log("Disconnecting from the server");
        if (this.runnerInstance != null)
        {
            await this.runnerInstance.Shutdown();
            this.runnerInstance = null;
        }
    }
}

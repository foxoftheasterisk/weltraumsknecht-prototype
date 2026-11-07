using UnityEngine;
using UnityEngine.SceneManagement;
using Weltraumsknecht.Player;

public class Door : MonoBehaviour
{
    public string scene;
    private AsyncOperation sceneLoading;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneLoading = SceneManager.LoadSceneAsync(scene, new LoadSceneParameters(LoadSceneMode.Single));
        sceneLoading.allowSceneActivation = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerMovementController>(out _))
        {
            sceneLoading.allowSceneActivation = true;
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string levelSceneName = "Level";

    [Header("UI Elements")]
    [SerializeField] private Button startButton;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogWarning("Start Button not assigned in LobbyManager!");
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting Game... Loading " + levelSceneName);
        SceneManager.LoadScene(levelSceneName);
    }

    public void BuyWeapon(string weaponName)
    {
        Debug.Log("Buying weapon: " + weaponName);
    }
}

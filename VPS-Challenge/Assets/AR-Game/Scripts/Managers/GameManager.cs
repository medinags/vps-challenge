using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public event Action OnLocationSelected;
    public event Action OnLocationFound;
    public event Action OnMinimumMeshesFound;
    public event Action OnGameStart;
    public event Action OnEggLaid;
    public event Action OnSnakeBorn;
    public event Action OnSnakeSpawen;
    public event Action OnappleEaten;
    public event Action OnGameOver;
    public event Action OnSnakePlaced;


    public GameObject SnakeManager;

    public static GameManager Instance;

    public bool UseVPS = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void UseVPSFeature(bool state)
    {
        UseVPS = state;
    }

    public void LocalizationSelected()
    {
        Debug.Log("Location Selected");
        OnLocationSelected?.Invoke();
    }
    public void LocationFound()
    {
        Debug.Log("Location found");
        OnLocationFound?.Invoke();
    }
    public void MinimumMeshesFound()
    {
        Debug.Log("Minimum Meshes Found");
        OnMinimumMeshesFound?.Invoke();
    }
    public void StartGame()
    {
        OnGameStart?.Invoke();
    }
    public void EggLaid()
    {
        Debug.Log("Egg Laif");
        OnEggLaid?.Invoke();
    }

    public void SnakeBorn()
    {
        Debug.Log("Snake Born");
        OnSnakeBorn?.Invoke();
    }

    public void SnakePlaced()
    {
        Debug.Log("Snake Placed");
        OnSnakePlaced?.Invoke();
    }

    public void AppleEaten()
    {
        Debug.Log("Apple Eate");
        OnappleEaten?.Invoke();
    }

    public void SnakeSpawen()
    {
        Debug.Log("On Sanke Spawen");
        OnSnakeSpawen?.Invoke();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        Debug.Log("Restart");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}

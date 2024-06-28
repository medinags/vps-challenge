using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayManager : MonoBehaviour
{
    public event Action OnPowerUp;
    public event Action<PowerUpType> OnNewPowerUp;

    public static GamePlayManager Instance;
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
    [Header("Game Play States")]
    [SerializeField] private float countdownTime = 10f;
    [HideInInspector]
    public int ApplesCount = 0;
    [HideInInspector]
    public int PowerUpsCount = 0;
    [HideInInspector]
    public int PowerUpDestroyCount;
    [HideInInspector]
    public int PowerUpGrassCount;
    [HideInInspector]
    public int Score;
    public int LifeCount = 2;

    [HideInInspector]
    public bool IsPowerUpAppleActivated;
    [HideInInspector]
    public bool IsPowerUpDestroyActivated;
    [HideInInspector]
    public bool IsPowerUpGrassActivated;
    [HideInInspector]
    public bool ShouldSpawnApples;
    public int deadTime = 4500;

    public const int LIVE_VALUE = 5;
    public const int APPLE_VALUE = 20;
    public const int POWER_UP = 10;
    public const int POWER_UP_APPLE = 10;
    public const int POWER_UP_DESTROY = 10;
    public const int POWER_UP_GRASS = 15;

    public int[] PowerUpIn = { 1, 4, 10, 15, 20, 22, 30 };

    public List<GameObject> ActiveApples = new List<GameObject>();

    public event Action OnCountdownFinished;
    [SerializeField] private float currentTime;
    [SerializeField] private bool pauseTimer = true;
    private void Start()
    {
        pauseTimer = true ;
        currentTime = countdownTime;
        GameManager.Instance.OnSnakePlaced += SnakePlaced;
        GameManager.Instance.OnGameOver += GameOver;
    }
    private void GameOver()
    {
        pauseTimer = true;
    }
    private void SnakePlaced()
    {
        pauseTimer = false;
    }

    private void Update()
    {
        if (pauseTimer)
        {
            return;
        }
        currentTime -= Time.deltaTime;
        UIManager.Instance.UpdateGameTime(currentTime);

        if (currentTime <= 0f)
        {
            pauseTimer = true;
            OnCountdownFinished?.Invoke();
            GameManager.Instance.GameOver();
            UIManager.Instance.UpdateGameTime(0);
        }
    }


    public void ResetTimer(float newTime)
    {
        countdownTime = newTime;
        currentTime = countdownTime;
        pauseTimer = true;
    }
    public void PowerUP()
    {
        OnPowerUp?.Invoke();
    }

    public void NewPowerUp(PowerUpType powerUpType)
    {
        PowerUpsCount++;


        switch (powerUpType)
        {
            case PowerUpType.Apples:
                IsPowerUpAppleActivated = true;
                break;
            case PowerUpType.Immudity:

                break;
            case PowerUpType.Grass:
                PowerUpGrassCount++;
                IsPowerUpGrassActivated = true;
                break;
            default:
                break;
        }

        Instance.CalculateScore();
        OnNewPowerUp?.Invoke(powerUpType);
    }

    public void CalculateScore()
    {
        Score = ApplesCount * APPLE_VALUE +
            PowerUpsCount * POWER_UP +
            LifeCount*LIVE_VALUE +
            PowerUpDestroyCount * POWER_UP_DESTROY +
            PowerUpGrassCount * POWER_UP_GRASS;
        UIManager.Instance.UpdatePoints(Score.ToString(), LifeCount.ToString());
    }

}

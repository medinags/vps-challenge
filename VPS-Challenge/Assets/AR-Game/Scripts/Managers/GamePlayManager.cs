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
    public int ApplesCount = 0;
    public int PowerUpsCount = 0;
    public int PowerUpAppleCount;
    public int PowerUpDestroyCount;
    public int PowerUpGrassCount;
    public int Points;
    public int LifeCount = 2;


    public bool IsPowerUpAppleActivated;

    public bool IsPowerUpDestroyActivated;

    public bool IsPowerUpGrassActivated;
    [HideInInspector]
    public bool ShouldSpawnApples;
    public int deadTime = 4500;

    public const int APPLE_VALUE = 20;
    public const int POWER_UP = 10;
    public const int POWER_UP_APPLE = 10;
    public const int POWER_UP_DESTROY = 10;
    public const int POWER_UP_GRASS = 15;

    public int[] PowerUpIn = { 1, 4, 10, 15, 20, 22, 30 }; 
    public List<GameObject> ActiveApples = new List<GameObject>();
    public void PowerUP() 
    {
        OnPowerUp?.Invoke();
    }

    public void NewPowerUp(PowerUpType powerUpType)
    {
        PowerUpsCount++;
        Instance.PointCounts();

        switch (powerUpType)
        {
            case PowerUpType.Apples:
                IsPowerUpAppleActivated = true;
                break;
            case PowerUpType.Immudity:

                break;
            case PowerUpType.Grass:

                break;
            default:
                break;
        }

   
        OnNewPowerUp?.Invoke(powerUpType);
    }

    public void PointCounts() 
    {
        Points = ApplesCount * APPLE_VALUE +
            PowerUpsCount*POWER_UP +
            PowerUpDestroyCount*POWER_UP_DESTROY +
            PowerUpGrassCount*POWER_UP_GRASS;
    }

}

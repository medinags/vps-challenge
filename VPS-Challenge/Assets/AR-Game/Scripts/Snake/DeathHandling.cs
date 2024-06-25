using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System;
using System.Diagnostics;


public class DeathHandling : MonoBehaviour
{
    private Stopwatch timer = new Stopwatch();
    private bool shouldKill = true;
    public float currentTime;


    void Update()
    {
        if (!timer.IsRunning)
            return;

        currentTime = timer.ElapsedMilliseconds;
        if (currentTime >= GamePlayManager.Instance.deadTime)
        {
            shouldKill = true;
            timer.Reset();
        }

    }

    public void StartDeadTimer() 
    {
        Debug.Log("Aush, one life gone");
        if (shouldKill)
        {
            timer.Start();
            KillPart();
            shouldKill = false;

        }
        else
        {
            timer.Restart();
        }
    }

    private void KillPart() 
    {

        if (GamePlayManager.Instance.LifeCount >= 0)
        {
            SnakeControllerManager.Instance.snakeBody[SnakeManager.Instance.visibleBody -1].transform.GetChild(0).gameObject.SetActive(false); 
            SnakeManager.Instance.visibleBody--;
            GamePlayManager.Instance.ApplesCount--;
            GamePlayManager.Instance.LifeCount--;
            Handheld.Vibrate();
        }
        if (GamePlayManager.Instance.LifeCount < 0)
        {
            GameManager.Instance.GameOver();
        }

    }
}

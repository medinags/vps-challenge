using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EggController : MonoBehaviour
{
    public GameObject Snake;
    // Start is called before the first frame update
    void Start()
    {
        CallSnakeGeneration();
    }

    public void CallSnakeGeneration() 
    {
        StartCoroutine(SnakeGeneration());
    }

    IEnumerator SnakeGeneration() 
    {
        //GameManager.instance.SnakeBorn();
        Debug.Log("Snake Generation");
        this.transform.DOShakeRotation(1.5f, 60, -90, 90, true);
        yield return new WaitForSeconds(1.25f);

        Snake.transform.position = this.transform.position;
        Snake.SetActive(true);
        this.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.45f);
        GameManager.Instance.SnakeBorn();
    }
}

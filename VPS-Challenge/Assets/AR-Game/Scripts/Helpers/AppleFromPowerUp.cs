using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AppleFromPowerUp : MonoBehaviour
{

    private void Start()
    {
        RegularSize();
    }

    public void RegularSize() 
    {
        StartCoroutine(SetScale());
    }

    public IEnumerator SetScale()
    {
        Debug.Log("Power Up Aplle" + name);
        GamePlayManager.Instance.ActiveApples.Add(gameObject);
        //RadarManager.Instance.GenerateHelper(this.gameObject);
        UIRadar.Instance.GeneratePointer(this.gameObject, Color.red);
        var Pos = SnakeControllerManager.Instance.snakeBody[0].transform.position;

        yield return new WaitForSeconds(0.5f);

        this.transform.DOScale(Vector3.one * 0.1f, 0.5f);
        this.transform.DOMove(new Vector3(this.transform.position.x, Pos.y, this.transform.position.z), 0.5f);
        this.transform.DOShakeRotation(0.3f,90,10,40,true);
        this.transform.parent = null;
        this.GetComponent<MeshCollider>().enabled = false;
        //TODO RADAR
        //this.transform.GetChild(0).GetComponent<MeshCollider>().enabled = false;

    }

    public void OnDestroy()
    {
        GamePlayManager.Instance.OnPowerUp -= RegularSize;
    }

}

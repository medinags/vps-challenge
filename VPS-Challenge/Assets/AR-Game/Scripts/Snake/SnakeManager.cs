using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{

    public static SnakeManager Instance;


    public Mesh currentMesh;
    public GameObject currenMeshContainer;
    public bool justSpawned = true;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        var meshFilter = collision.gameObject.GetComponent<MeshFilter>();

        if (meshFilter)
        {
            currentMesh = meshFilter.sharedMesh;
            currenMeshContainer = collision.gameObject;
            if (justSpawned)
            {
                GameManager.Instance.SnakePlaced();
                justSpawned = false;
            }
        }
    }
}

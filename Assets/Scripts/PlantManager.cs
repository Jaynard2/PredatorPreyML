using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    [SerializeField]
    private Vector2 spawnArea;
    [SerializeField]
    private float spawnHeight;
    [SerializeField]
    private int initialPop;
    [SerializeField]
    private GameObject plantPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Spawner spawner = gameObject.AddComponent<Spawner>();
        spawner.setup(gameObject);
        spawner.rectSpawn(plantPrefab, initialPop, spawnArea, spawnHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField]
    private float spawnRadius;
    [SerializeField]
    private float spawnHeight;
    [SerializeField]
    private int initialPop;
    [SerializeField]
    private GameObject preyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Spawner spawner = gameObject.AddComponent<Spawner>();
        spawner.circularSpawn(preyPrefab, initialPop, spawnRadius, spawnHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

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
        for (int i = 0; i < initialPop; i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(0, spawnRadius), Random.Range(0, 2 * Mathf.PI));
            spawnPos = new Vector2(spawnPos.x * Mathf.Cos(spawnPos.y), spawnPos.x * Mathf.Sin(spawnPos.y));
            Instantiate(preyPrefab, transform.position + new Vector3(spawnPos.x, spawnHeight, spawnPos.y), Quaternion.identity);
        }
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

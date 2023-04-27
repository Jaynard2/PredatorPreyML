using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner: MonoBehaviour
{
    private GameObject _par;

    public void setup(GameObject spawnParant)
    {
        _par = spawnParant;
    }

    public void circularSpawn(GameObject prefab, int num, float spawnRadius, float spawnHeight)
    {
        for (int i = 0; i < num; i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(0, spawnRadius), Random.Range(0, 2 * Mathf.PI));
            spawnPos = new Vector2(spawnPos.x * Mathf.Cos(spawnPos.y), spawnPos.x * Mathf.Sin(spawnPos.y));
            bool isAnimal = Instantiate(prefab, transform.position + new Vector3(spawnPos.x, spawnHeight, spawnPos.y), Quaternion.identity, _par.transform).TryGetComponent(out Animal animal);
            if(isAnimal)
                animal.setParent(_par);
        }
        transform.rotation = Quaternion.identity;
    }

    public void circularSpawnWithFFNN(GameObject prefab, int num, float spawnRadius, float spawnHeight, string FFNN)
    {
        for (int i = 0; i < num; i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(0, spawnRadius), Random.Range(0, 2 * Mathf.PI));
            spawnPos = new Vector2(spawnPos.x * Mathf.Cos(spawnPos.y), spawnPos.x * Mathf.Sin(spawnPos.y));
            bool isAnimal = Instantiate(prefab, transform.position + new Vector3(spawnPos.x, spawnHeight, spawnPos.y), Quaternion.identity, _par.transform).TryGetComponent(out Animal animal);
            if (isAnimal)
            {
                animal.setParent(_par);
                animal.init(global::FFNN.load(FFNN));
            }
        }
        transform.rotation = Quaternion.identity;

    }

    public void rectSpawn(GameObject prefab, int num, Vector2 spawnArea, float spawnHeight)
    {
        for (int i = 0; i < num; i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(-spawnArea.x/2, spawnArea.x/2), Random.Range(-spawnArea.y/2, spawnArea.y/2));
            bool isAnimal = Instantiate(prefab, transform.position + new Vector3(spawnPos.x, spawnHeight, spawnPos.y), Quaternion.identity, _par.transform).TryGetComponent(out Animal animal);
            if(isAnimal)
                animal.setParent(_par);
        }
    }
}

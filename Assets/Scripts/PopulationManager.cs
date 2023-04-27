using System.IO;
using System.Linq;
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
    [SerializeField]
    private string brainFile;

    string bestBrain;
    float bestScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        Spawner spawner = gameObject.AddComponent<Spawner>();
        spawner.setup(gameObject);
        if (brainFile != "")
        {
            if (!File.Exists(brainFile))
                throw new System.Exception("File must exist");
            gameObject.GetComponent<Spawner>().circularSpawnWithFFNN(preyPrefab, initialPop, spawnRadius, spawnHeight, File.ReadAllText(brainFile));
        }
        spawner.circularSpawn(preyPrefab, initialPop, spawnRadius, spawnHeight);
    }

    // Update is called once per frame
    void Update()
    {
        int bestIndex = -1;
        for(int i = 0; i < transform.childCount; i++) 
        {
            var child = transform.GetChild(i).GetComponent<Animal>();
            if(!child.isActiveAndEnabled)
            {
                Destroy(child.gameObject);
                continue;
            }
            if (child.score > bestScore)
            {
                bestScore = child.score;
                bestIndex = i;
            }
        }
        if(bestIndex != -1)
            bestBrain = transform.GetChild(bestIndex).GetComponent<Animal>().brain.save();

        if (transform.childCount == 0)
        {
            for(int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
            Debug.Log("Extinct. Respawing with score of " + bestScore.ToString());
            gameObject.GetComponent<Spawner>().circularSpawnWithFFNN(preyPrefab, initialPop, spawnRadius, spawnHeight, bestBrain);
        }
    }

    ~PopulationManager()
    {
        string fileName = bestScore.ToString() + "-0";
        int counter = 1;
        while (File.Exists(fileName))
        {
            fileName = fileName.Remove(fileName.Length - 1, 1) + counter.ToString();
            counter++;
        }

        File.WriteAllText(fileName + ".csv", bestBrain.ToString());
    }
}

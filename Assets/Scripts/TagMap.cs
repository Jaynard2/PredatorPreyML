using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagMap : MonoBehaviour
{
    private Dictionary<string, int> tags = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        tags.Add("plant", 0);
        tags.Add("prey", 1);
    }

    public int getTagID(string tag)
    {
        bool success = tags.TryGetValue(tag, out int tagID);
        if (success) return tagID;
        return -1;
    }

    public static TagMap inst { get; private set; }

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}


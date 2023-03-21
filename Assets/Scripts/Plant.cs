using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField]
    private float startVal;
    [SerializeField]
    private float maxVal;
    [SerializeField]
    private float growthRate;
    [field: SerializeField]
    public float currVal { get; private set; }

    // Update is called once per frame
    void Update()
    {
        if(currVal < maxVal)
        {
            currVal += growthRate * Time.deltaTime;
            currVal = Mathf.Min(currVal, maxVal);
            transform.localScale = new Vector3(1, currVal,1);
        }
    }

    float eat(float quantity)
    {
        float actualAmount = Mathf.Min(quantity, currVal);
        currVal -= actualAmount;
        return actualAmount;
    }
}

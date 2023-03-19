using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class MatrixTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*Matrix<double> m = CreateMatrix.Dense<double>(3, 4, 1.5f);
        Matrix<double> n = CreateMatrix.Dense<double>(4, 3, 2.5f);
        Debug.Log(m * n);

        int total = 0;
        for (int i = 0; i < 100; i++)
        {
            if (Random.Range(0f, 1f) < 0.1f)
            {
                total++;
            }
        }
        Debug.Log(total);*/

        FFNN testBrain = new FFNN(new int[2] { 3, 5});
        Matrix<double> inputs = Matrix<double>.Build.Dense(1, 3);
        inputs[0, 0] = 1;
        inputs[0, 1] = 2;
        inputs[0, 2] = 3;
        Debug.Log(testBrain.getLayer(0).weights);
        Debug.Log(testBrain.getLayer(0).biases);
        Debug.Log(testBrain.forward(inputs));
        testBrain.mutate(0.5f, 0.2);
        Debug.Log("Mutation");
        Debug.Log(testBrain.getLayer(0).weights);
        Debug.Log(testBrain.getLayer(0).biases);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

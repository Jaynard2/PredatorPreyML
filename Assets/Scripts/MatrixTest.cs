using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class MatrixTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Matrix<double> m = CreateMatrix.Dense<double>(3, 4, 1.5f);
        Matrix<double> n = CreateMatrix.Dense<double>(4, 3, 2.5f);
        Debug.Log(m * n);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

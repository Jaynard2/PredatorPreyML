using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public struct FFNNLayer
{
    public Matrix<double> weights;
    public Matrix<double> biases;

    public FFNNLayer(Matrix<double> Weights, Matrix<double> Biases)
    {
        weights = Weights;
        biases = Biases;
    }
}

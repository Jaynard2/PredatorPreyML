using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using UnityEngine;

public class FFNN
{
    private Matrix<double>[] weights;
    private Matrix<double>[] biases;

    public FFNN(int[] layerSizes)
    {
        if(layerSizes == null || layerSizes.Length <= 1)
        {
            throw new System.Exception("Invalid network size");
        }

        weights = new Matrix<double>[layerSizes.Length - 1];
        biases = new Matrix<double>[layerSizes.Length - 1];
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Matrix<double>.Build.Random(layerSizes[i], layerSizes[i+1]);
            biases[i] = Matrix<double>.Build.Random(1, layerSizes[i+1]);
        }
    }

    public FFNN(FFNN other)
    {
        if (other == null)
        {
            throw new System.Exception("Invalid network");
        }

        weights = new Matrix<double>[other.weights.Length];
        biases = new Matrix<double>[other.weights.Length];
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = other.weights[i].Clone();
            biases[i] = other.biases[i].Clone();
        }
    }

    public FFNN breedWith(FFNN other, float geneWeight = 0.5f)
    {
        FFNN child = new FFNN(this);
        //breed weights
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].RowCount; j++)
            {
                for (int k = 0; k < weights[i].ColumnCount; k++)
                {
                    if (Random.Range(0f, 1f) < geneWeight)
                    {
                        child.weights[i][j, k] = other.weights[i][j,k];
                    }
                }
            }
        }

        //breed biases
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].ColumnCount; j++)
            {
                child.biases[i][0, j] = other.biases[i][0, j];
            }
        }

        return child;
    }

    public FFNNLayer getLayer(int layerNum)
    {
        if(layerNum < 0 || layerNum >= weights.Length)
        {
            throw new System.Exception("Invalid layer number: " + layerNum);
        }
        return new FFNNLayer(weights[layerNum], biases[layerNum]);
    }

    public Matrix<double> forward(Matrix<double> inputs)
    {

        if (inputs == null || inputs.RowCount != 1 || inputs.ColumnCount != weights[0].RowCount)
        {
            throw new System.Exception("Invalid input size: " + inputs.RowCount + " x " + inputs.ColumnCount);
        }

        Matrix<double> answer = inputs;

        for (int i = 0; i < weights.Length; i++)
        {
            //perform operation
            answer = answer * weights[i] + biases[i];

            //activation function
            for (int j = 0; j < answer.ColumnCount; j++)
            {
                answer[0, j] = sigmoid(answer[0, j]);
            }
        }

        return answer;
    }

    public void mutate(float chance, double strength)
    {
        //mutate weights
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].RowCount; j++)
            {
                for (int k = 0; k < weights[i].ColumnCount; k++)
                {
                    if(Random.Range(0f,1f) < chance)
                    {
                        weights[i][j, k] += Normal.Sample(0, strength);
                    }
                }
            }
        }

        //mutate biases
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].ColumnCount; j++)
            {
                if (Random.Range(0f, 1f) < chance)
                {
                    biases[i][0, j] += Normal.Sample(0, strength);
                }
            }
        }
    }

    public double ReLU(double x)
    {
        if(x <= 0)
        {
            return 0;
        }
        return x;
    }

    public double sigmoid(double x)
    {
        return MathNet.Numerics.SpecialFunctions.Logistic(x);
    }

}

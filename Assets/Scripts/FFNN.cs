using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using UnityEngine;

public class FFNN
{
    private Matrix<double>[] weights { set;  get; }
    private Matrix<double>[] biases { set; get; }
    private int[] layerSizes;

    public FFNN(int[] LayerSizes)
    {
        if(LayerSizes == null || LayerSizes.Length <= 1)
        {
            throw new System.Exception("Invalid network size");
        }

        layerSizes = (int[])LayerSizes.Clone();
        weights = new Matrix<double>[LayerSizes.Length - 1];
        biases = new Matrix<double>[LayerSizes.Length - 1];
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Matrix<double>.Build.Random(LayerSizes[i], LayerSizes[i+1]);
            biases[i] = Matrix<double>.Build.Random(1, LayerSizes[i+1]);
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

    public static FFNN load(string data)
    {
        string[] dataLines = data.Split('\n');
        int currLineNum = 0;
        string currLine = dataLines[currLineNum];
        string[] currLineParts = currLine.Split(',');

        int[] layerSizes = new int[currLineParts.Length];
        //load layerSizes
        for (int i = 0; i < currLineParts.Length; i++)
        {
            layerSizes[i] = int.Parse(currLineParts[i]);
        }
        currLineNum += 2;

        Matrix<double>[] weights = new Matrix<double>[layerSizes.Length - 1];
        Matrix<double>[] biases = new Matrix<double>[layerSizes.Length - 1];

        //load weights
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Matrix<double>.Build.Dense(layerSizes[i], layerSizes[i + 1]);
            for (int j = 0; j < layerSizes[i]; j++)
            {
                currLine = dataLines[currLineNum];
                currLineParts = currLine.Split(',');
                currLineNum++;
                for (int k = 0; k < layerSizes[i + 1]; k++)
                {
                    weights[i][j, k] = double.Parse(currLineParts[k]);
                }
            }
            currLineNum++;
        }

        //load biases
        for (int i = 0; i < weights.Length; i++)
        {
            currLine = dataLines[currLineNum];
            currLineParts = currLine.Split(',');
            currLineNum++;
            biases[i] = Matrix<double>.Build.Dense(1, layerSizes[i + 1]);
            for (int j = 0; j < layerSizes[i+1]; j++)
            {
                biases[i][0,j] = double.Parse(currLineParts[j]);
            }
            currLineNum++;
        }

        FFNN ffnn = new FFNN(layerSizes);
        ffnn.weights = weights;
        ffnn.biases = biases;
        return ffnn;
    }

    public string save()
    {
        string s = "";

        //save layer sizes
        for (int i = 0; i < layerSizes.Length; i++)
        {
            s += layerSizes[i];
            if(i != layerSizes.Length - 1)
            {
                s += ",";
            }
        }

        s += '\n';
        s += '\n';

        //save weights
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].RowCount; j++)
            {
                for (int k = 0; k < weights[i].ColumnCount; k++)
                {
                    s += weights[i][j, k];
                    if (k != weights[i].ColumnCount - 1)
                    {
                        s += ",";
                    }
                }
                s += '\n';
            }
            s += '\n';
        }

        //save biases
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].ColumnCount; j++)
            {
                s += biases[i][0, j];
                if (j != biases[i].ColumnCount - 1)
                {
                    s += ",";
                }
            }
            s += '\n';
            s += '\n';
        }

        return s;
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

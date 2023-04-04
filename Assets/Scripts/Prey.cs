using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class Prey : Animal
{
    [SerializeField]
    private float maxStationarySpeed;
    [SerializeField]
    private double[] brainOutputs;
    protected override void die()
    {
        Debug.Log("oof");
        Destroy(this.gameObject);
    }

    protected override void eat()
    {
        hunger -= eatSpeedMult * speed;
        if(hunger < 0)
        {
            hunger = 0;
        }
    }

    protected override void think()
    {
        //generate inputs
        Matrix<double> inputs = Matrix<double>.Build.Dense(1, 6);
        inputs[0, 0] = age;
        inputs[0, 1] = hunger;
        inputs[0, 2] = reproUrge;
        inputs[0, 3] = rb.velocity.x;
        inputs[0, 4] = rb.velocity.z;
        inputs[0, 5] = transform.eulerAngles.y;

        Matrix<double> outputs = brain.forward(inputs);
        brainOutputs = outputs.Row(0).ToArray();

        //rotate
        transform.eulerAngles += new Vector3(0, speed*(2*(float)outputs[0, 0]-1), 0);

        //movement between -1 and 1
        Vector3 movement = new Vector3((float)outputs[0, 1], 0f, 0f);
        movement *= 2;
        movement += new Vector3(-1, 0, 0);
        move(movement);

        //can't eat or reproduce while running
        if(rb.velocity.magnitude > maxStationarySpeed)
        {
            return;
        }

        //can't reproduce while eating
        if(outputs[0, 2] > 0.5)
        {
            eat();
            return;
        }

        if(outputs[0, 3] > 0.5)
        {
            reproduce();
        }
    }
}

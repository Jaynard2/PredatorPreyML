using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Drawing;
using System.Net;
using UnityEngine.Assertions;
using Unity.VisualScripting.Antlr3.Runtime;

public class Prey : Animal
{
    [SerializeField]
    private float maxStationarySpeed;
    [SerializeField]
    private double[] brainOutputs;
    [SerializeField]
    private float maxSight;
    [SerializeField]
    private int numRaycast;
    [SerializeField]
    private float viewAngle;
    [SerializeField]
    private float eatRange;
    protected override void die()
    {
        Destroy(this.gameObject);
    }

    // Use overload instead
    protected override void eat()
    {
    }

    protected void eat(Plant pl)
    {
        float eaten = pl.eat(hunger);
        hunger -= eaten;
    }

    protected override void think()
    {
        //generate inputs
        Matrix<double> inputs = Matrix<double>.Build.Dense(1, 6 + numRaycast * 2);
        inputs[0, 0] = age;
        inputs[0, 1] = hunger;
        inputs[0, 2] = reproUrge;
        inputs[0, 3] = rb.velocity.x;
        inputs[0, 4] = rb.velocity.z;
        inputs[0, 5] = transform.eulerAngles.y;

        float angleStep = viewAngle / System.Math.Max(numRaycast - 1, 1) * Mathf.Deg2Rad;
        float startAngle = Mathf.Atan2(transform.forward.z, transform.forward.x) - viewAngle * Mathf.Deg2Rad / 2;
        Plant food = null;
        float closestPlantDist = int.MaxValue;
        for (int i = 0; i < numRaycast; i++) 
        {
            Vector3 lookDir = new Vector3(Mathf.Cos(startAngle + angleStep * i), 0.0f, Mathf.Sin(startAngle + angleStep * i));
            bool hit = Physics.Raycast(transform.position, lookDir, out RaycastHit ray, maxSight);
            inputs[0, 5 + i * 2] = hit ? ray.distance : float.MaxValue;
            inputs[0, 6 + i * 2] = hit ? tagMap.getTagID(ray.transform.tag) : -1;

            // Determine if food is in range
            if(hit && ray.distance < eatRange && ray.distance < closestPlantDist && tagMap.getTagID(ray.transform.tag) == tagMap.getTagID("plant"))
            {
                closestPlantDist = ray.distance;
                food = ray.transform.GetComponent<Plant>();
            }


            if (hit)
                Debug.DrawRay(transform.position, maxSight * lookDir, UnityEngine.Color.red);
            else
                Debug.DrawRay(transform.position, maxSight * lookDir, UnityEngine.Color.blue);
        }

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
        if(outputs[0, 2] > 0.5 && food != null)
        {
            eat(food);
            return;
        }

        if(outputs[0, 3] > 0.5)
        {
            reproduce();
        }
    }
}

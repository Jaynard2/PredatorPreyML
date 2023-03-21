using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;

public abstract class Animal : MonoBehaviour
{
    //tuning parameters
    [SerializeField]
    private float speedMult;
    [SerializeField]
    private float speedPenalty;
    [SerializeField]
    private float movePenalty;
    [SerializeField]
    private float fertilePenalty;
    [SerializeField]
    protected float eatSpeedMult;
    [SerializeField]
    private float litterVariance;
    [SerializeField]
    private float maxAge;
    [SerializeField]
    private float softMaxSpeed;
    [SerializeField]
    private float drag;
    [SerializeField]
    private float bioMutRate;
    [SerializeField]
    private float brainMutRate;
    [SerializeField]
    private int[] layerSizes;

    //biology components
    [SerializeField]
    protected float hunger;
    [SerializeField]
    protected float age;
    [SerializeField]
    protected float reproUrge;

    //evolving parameters
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float dReproUrge;
    [SerializeField]
    protected float avgLitterSize;
    [SerializeField]
    protected float minReproAge;

    [SerializeField]
    private bool forceRepro;
    protected FFNN brain;
    private bool hasInit = false;
    [SerializeField]
    private GameObject babyPrefab;
    protected Rigidbody rb;

    protected abstract void think();
    protected abstract void die();
    protected abstract void eat();

    public void init(FFNN Brain = null, float startHunger = 0, float startReproUrge = 0, GameObject BabyPrefab = null, float startAge = 0)
    {
        hunger = startHunger;
        age = startAge;
        reproUrge = startReproUrge;
        if (BabyPrefab != null)
        {
            babyPrefab = BabyPrefab;
        }

        if(brain != null)
        {
            brain = Brain;
        }
        else
        {
            brain = new FFNN(layerSizes);
        }

        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = speed * softMaxSpeed;

        hasInit = true;
    }


    // Start is called before the first frame update
    void Start()
    {
        if (!hasInit)
        {
            init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currVel = rb.velocity;
        currVel /= drag;
        currVel.y = rb.velocity.y;
        rb.velocity = currVel;
        updateBio();
        think();
        if (forceRepro)
        {
            forceRepro = false;
            reproduce();
        }
    }

    private void mutateVals()
    {
        speed += (float)Normal.Sample(0, bioMutRate);
        dReproUrge += (float)Normal.Sample(0, bioMutRate);
        avgLitterSize += (float)Normal.Sample(0, bioMutRate);
        minReproAge += (float)Normal.Sample(0, bioMutRate);
    }

    private void updateBio()
    {
        hunger += Time.deltaTime * calcHunger();
        age += Time.deltaTime;
        if (age > minReproAge)
        {
            reproUrge += Time.deltaTime * dReproUrge;
        }
        if (hunger > 100 || age >= maxAge)
        {
            die();
        }
    }

    protected void reproduce()
    {
        if(age < minReproAge)
        {
            return;
        }

        int numBabies = Mathf.FloorToInt((float)Normal.Sample(avgLitterSize, litterVariance));
        if(numBabies < 1)
        {
            numBabies = 1;
        }

        //ensure food is conserved. Parent gets double portion
        float sharedFood = 100 - hunger;
        sharedFood = sharedFood / (numBabies + 2);
        hunger = 100 - (sharedFood * 2);

        for (int i = 0; i < numBabies; i++)
        {
            Animal currBaby = Instantiate(babyPrefab, transform.position + new Vector3(0, 2 * i, 0), Quaternion.identity).GetComponent<Animal>();
            FFNN newBrain = new FFNN(brain);
            newBrain.mutate(1f, brainMutRate);
            currBaby.mutateVals();
            currBaby.init(newBrain, 100 - sharedFood);
        }

        reproUrge = 0;
    }

    private float calcHunger()
    {
        return speed * speedPenalty * (age > minReproAge? fertilePenalty:1);
    }

    protected void move(Vector3 direction)
    {
        direction.Normalize();
        direction.y = 0;
        rb.AddForce(direction * speed * speedMult);
        hunger += direction.magnitude * movePenalty;
    }
}
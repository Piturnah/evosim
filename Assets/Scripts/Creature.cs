using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Creature : MonoBehaviour
{
    public Traits traits;
    Traits otherTraits;

    public bool isGestating = false;
    public GameObject creature;
    float timeToAdulthood = 30;
    float lookAngle;
    public float startingEnergy;
    public float energy;
    Vector2 moveDirection;

    float oldBoredTime;
    float previousSecond;
    float accumulatedCost;
    float gestationStartTime;
    public float offSpringStartingEnergy;

    bool targeting;
    public bool targetingMate = false;
    Vector2 targetPosition;
    Vector2 explorePos;
    Coroutine exploring;

    public Material maleMat, femaleMat, gestationMat;

    public GameObject data;
    public DataHandling dataHandling;

    private void Start()
    {
        targeting = false;
        transform.localScale = Vector2.one * traits.size;
        if (traits.isMale)
        {
            transform.GetChild(0).gameObject.GetComponent<Renderer>().material = maleMat;
            gameObject.tag = "Male";
        } else
        {
            transform.GetChild(0).gameObject.GetComponent<Renderer>().material = femaleMat;
            gameObject.tag = "Female";
        }
        energy = startingEnergy;
        offSpringStartingEnergy = 0;

        //data = GameObject.FindGameObjectWithTag("Data Handler");
        //dataHandling = data.GetComponent<DataHandling>();
        //traits.creatureID = dataHandling.GiveID();
        //dataHandling.creatureData.Add(traits);
    }
    private void Update()
    {
        Move();
        
        if (oldBoredTime + traits.boredomThreshold < Time.timeSinceLevelLoad 
            || (targetPosition - new Vector2 (transform.position.x, transform.position.y)).magnitude <= 0.5)
        {
            oldBoredTime = Time.timeSinceLevelLoad;
            targeting = false;
        }
        targetingMate = energy / (4000) >= traits.matingEnergyThreshold;
        if (isGestating && gestationStartTime + traits.femaleGestationLength <= Time.timeSinceLevelLoad)
        {
            isGestating = false;
            transform.GetChild(0).gameObject.GetComponent<Renderer>().material = femaleMat;
            Birth();
        }
    }
    void Move()
    {
        if (Mathf.Abs(transform.position.x) > 60 + Mathf.Abs(transform.localScale.x))
        {
            transform.position = new Vector2(transform.position.x * -1, transform.position.y);
        }
        if (Mathf.Abs(transform.position.y) > 60 + Mathf.Abs(transform.localScale.y))
        {
            transform.position = new Vector2(transform.position.x, transform.position.y * -1);
        }
        if (exploring == null)
        {
            exploring = StartCoroutine(Exploration());
        }
        if (!targeting)
        {
            moveDirection = FindTargetDirection();
        }
        transform.Translate(moveDirection * traits.movementSpeed * Time.deltaTime, Space.World);

        lookAngle = 90-Mathf.Atan2(moveDirection.y, moveDirection.x * -1) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.forward * lookAngle;

        //Deactivated code for testing energy cost per second
        //accumulatedCost += DetermineEnergyCost();

        //if (previousSecond + 1 < Time.timeSinceLevelLoad)
        //{
        //    print(accumulatedCost);
        //    accumulatedCost = 0;
        //    previousSecond = Time.timeSinceLevelLoad;
        //}
        energy -= DetermineEnergyCost() * Time.deltaTime;
        if (energy <= 0)
        {
            //dataHandling.RemoveCreature(traits.creatureID);
            Destroy(gameObject);
        }
    }
    Vector2 FindTargetDirection()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, traits.viewRadius);
        if (hits != null)
        {
            foreach (Collider hit in hits)
            {
                if (hit != this.gameObject.GetComponent<MeshCollider>() && hit.gameObject.tag == "Food" && !targetingMate)
                {
                    targetPosition = hit.transform.position;
                    Vector2 directionToCollider = (hit.transform.position - transform.position).normalized;
                    float angleToCollider = 90 - Mathf.Atan2(directionToCollider.y, directionToCollider.x) * Mathf.Rad2Deg;
                    float relativeAngle = Mathf.Abs(Mathf.Abs(angleToCollider) - Mathf.Abs(lookAngle));
                    if (relativeAngle < traits.viewAngle)
                    {
                        targeting = true;
                        return directionToCollider;
                    }
                } else if (hit != this.gameObject.GetComponent<MeshCollider>() 
                    && hit.gameObject.tag == "Male" && gameObject.tag == "Female" && targetingMate && !hit.gameObject.GetComponent<Creature>().isGestating ||
                    hit != this.gameObject.GetComponent<MeshCollider>()
                    && hit.gameObject.tag == "Female" && gameObject.tag == "Male" && targetingMate && !isGestating)
                {
                    targetPosition = hit.transform.position;
                    Vector2 directionToCollider = (hit.transform.position - transform.position).normalized;
                    float angleToCollider = 90 - Mathf.Atan2(directionToCollider.y, directionToCollider.x) * Mathf.Rad2Deg;
                    float relativeAngle = Mathf.Abs(Mathf.Abs(angleToCollider) - Mathf.Abs(lookAngle));
                    if (relativeAngle < traits.viewAngle)
                    {
                        targeting = true;
                        return directionToCollider;
                    }
                }
            }
            return (explorePos).normalized;
        } else
        {
            return (explorePos).normalized;
        }
    }
    float DetermineEnergyCost()
    {
        float cost = Mathf.Pow(traits.size, 3) * Mathf.Pow(traits.movementSpeed, 2);
        cost += traits.viewAngle * traits.viewRadius / 3;
        if (isGestating)
        {
            float energyLostToFoetus = energy * traits.energyPercentToOfspring;
            cost += energyLostToFoetus;
            offSpringStartingEnergy += energyLostToFoetus;
        }

        return cost;
    }
    public struct Traits
    {
        public int creatureID;

        public float viewRadius, viewAngle;
        public float maledesirability;
        public float matingEnergyThreshold, maleToFemaleOffspringRatio;
        public float movementSpeed;
        public float size;
        public float meatToVeggieDigestionEfficiencyRatio;
        public float boredomThreshold;
        public float exploreMultiplier;

        public float maleEnergyToOffspring;

        public float energyPercentToOfspring;
        public float femaleGestationLength;
        public float femaleStandards;

        public bool isMale;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food")
        {
            float newEnergy = 0;
            if (other.name == "Mea")
            {
                if (traits.meatToVeggieDigestionEfficiencyRatio > 0.3f)
                {
                    newEnergy = energy += 700 * traits.meatToVeggieDigestionEfficiencyRatio;
                } else
                {
                    return;
                }
            } else
            {
                if (1-traits.meatToVeggieDigestionEfficiencyRatio > 0.3f)
                {
                    newEnergy = energy += 700 * (1 - traits.meatToVeggieDigestionEfficiencyRatio);
                } else
                {
                    return;
                }
            }
            energy = Mathf.Min(newEnergy, 4000);
            Destroy(other.transform.parent.gameObject);
        } else if (gameObject.tag == "Female" && targetingMate && other.tag == "Male" && !isGestating)
        {
            Creature mateTraits = other.GetComponent<Creature>();
            if (traits.femaleStandards + mateTraits.traits.maledesirability > UnityEngine.Random.Range(0f,1f))
            {
                isGestating = true;
                gestationStartTime = Time.timeSinceLevelLoad;
                offSpringStartingEnergy = 0;
                otherTraits = other.GetComponent<Creature>().traits;
                other.GetComponent<Creature>().MaleGiveEnergyToOffspring(this.GetComponent<Creature>());
                transform.GetChild(0).gameObject.GetComponent<Renderer>().material = gestationMat;
            }
        }
    }
    void Birth()
    {
        GameObject childCreature = Instantiate(creature, transform.position, transform.rotation);

        Creature creatureScript = childCreature.GetComponent<Creature>();

        float offspringIsMale = UnityEngine.Random.Range(0f, 1f);
        if (offspringIsMale > traits.maleToFemaleOffspringRatio)
        {
            creatureScript.traits.isMale = true;
        }
        else
        {
            creatureScript.traits.isMale = false;
        }
        Vector2 percent = new Vector2(0f, 1f);
        creatureScript.startingEnergy = Mathf.Clamp(offSpringStartingEnergy, 0, 4000);
        creatureScript.traits.size = DetermineFloatInheritance(traits.size, otherTraits.size, new Vector2(0.1f, 20));
        creatureScript.traits.viewRadius = DetermineFloatInheritance(traits.viewRadius, otherTraits.viewRadius, new Vector2(0, 50));
        creatureScript.traits.viewAngle = DetermineFloatInheritance(traits.viewAngle, otherTraits.viewAngle, new Vector2(0, 180));
        creatureScript.traits.maledesirability = DetermineFloatInheritance(traits.maledesirability, otherTraits.maledesirability, percent);
        creatureScript.traits.matingEnergyThreshold = DetermineFloatInheritance(traits.matingEnergyThreshold, otherTraits.matingEnergyThreshold, percent);
        creatureScript.traits.maleToFemaleOffspringRatio = DetermineFloatInheritance(traits.maleToFemaleOffspringRatio, otherTraits.maleToFemaleOffspringRatio, percent);
        creatureScript.traits.movementSpeed = DetermineFloatInheritance(traits.movementSpeed, otherTraits.movementSpeed, new Vector2(0.8f, 100));
        creatureScript.traits.meatToVeggieDigestionEfficiencyRatio = DetermineFloatInheritance(traits.meatToVeggieDigestionEfficiencyRatio, otherTraits.meatToVeggieDigestionEfficiencyRatio, percent);
        creatureScript.traits.boredomThreshold = DetermineFloatInheritance(traits.boredomThreshold, otherTraits.boredomThreshold, new Vector2(0, float.MaxValue));
        creatureScript.traits.exploreMultiplier = DetermineFloatInheritance(traits.exploreMultiplier, otherTraits.exploreMultiplier, new Vector2(1, int.MaxValue));
        creatureScript.traits.maleEnergyToOffspring = DetermineFloatInheritance(traits.maleEnergyToOffspring, otherTraits.maleEnergyToOffspring, percent);
        creatureScript.traits.energyPercentToOfspring = DetermineFloatInheritance(traits.energyPercentToOfspring, otherTraits.energyPercentToOfspring, percent);
        creatureScript.traits.femaleGestationLength = DetermineFloatInheritance(traits.femaleGestationLength, otherTraits.femaleGestationLength, new Vector2(0f, float.MaxValue));
        creatureScript.traits.femaleStandards = DetermineFloatInheritance(traits.femaleStandards, otherTraits.femaleStandards, percent);
    }
    float DetermineFloatInheritance(float motherGene, float fatherGene, Vector2 clamp)
    {
        float toReturn = 0;
        switch (UnityEngine.Random.Range(1,3))
        {
            case 1:
                toReturn = motherGene;
                break;
            case 2:
                toReturn = fatherGene;
                break;
        }
        toReturn += UnityEngine.Random.Range(-0.2f * toReturn, 0.2f * toReturn);
        return Mathf.Clamp(toReturn, clamp.x, clamp.y);
    }
    void MaleGiveEnergyToOffspring(Creature mate)
    {
        energy -= energy * traits.maleEnergyToOffspring;
        mate.offSpringStartingEnergy += energy * traits.maleEnergyToOffspring;
    }
    IEnumerator Exploration()
    {
        explorePos = new Vector2(UnityEngine.Random.Range(-60, 60), UnityEngine.Random.Range(-60, 60));
        yield return new WaitForSeconds(traits.exploreMultiplier);
    }
}

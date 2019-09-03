using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] foodTypes;
    int initialFoodAmount = 3000;
    float spawnRate = 300;

    public GameObject creature;
    int initialCreatureAmount = 750;

    float secondsBetweenSpawns;
    float previousSpawnTime;

    private void Start()
    {
        secondsBetweenSpawns = 60 / spawnRate;

        //Generate the initial food pellets
        for (int i = 0; i < initialFoodAmount; i++)
        {
            CreatePellet();
        }
        
        //Generate the initial creatures
        for (int i = 0; i < initialCreatureAmount; i++)
        {
            GameObject newCreature = Instantiate(creature, new Vector2(Random.Range(-60, 60), Random.Range(-60, 60)), transform.rotation);
            newCreature.transform.parent = transform;
            Creature creatureScript = newCreature.GetComponent<Creature>();

            creatureScript.startingEnergy = 4000;
            creatureScript.traits.viewRadius = Random.Range(1, 10);
            creatureScript.traits.viewAngle = Random.Range(10, 30);
            creatureScript.traits.maledesirability = Random.Range(0f, 1f);
            creatureScript.traits.matingEnergyThreshold = Random.Range(0f, 1f);
            creatureScript.traits.maleToFemaleOffspringRatio = Random.Range(0f, 1f);
            int offspringIsMale = Random.Range(1, 3);
            if (offspringIsMale > 1)
            {
                creatureScript.traits.isMale = true;
            } else
            {
                creatureScript.traits.isMale = false;
            }
            creatureScript.traits.movementSpeed = Random.Range(1, 50); //1-50
            creatureScript.traits.size = Random.Range(3, 6);
            creatureScript.traits.meatToVeggieDigestionEfficiencyRatio = Random.Range(0f, 1f);
            creatureScript.traits.boredomThreshold = Random.Range(1, 15);
            creatureScript.traits.energyPercentToOfspring = Random.Range(0f, .8f);
            creatureScript.traits.femaleGestationLength = Random.Range(5f, 30f);
            creatureScript.traits.femaleStandards = Random.Range(0.3f, 1f);
            creatureScript.traits.exploreMultiplier = Random.Range(5, 30);
            creatureScript.traits.maleEnergyToOffspring = Random.Range(0f, 1f);

        }
    }
    private void Update()
    {
        if (Time.timeSinceLevelLoad > previousSpawnTime + secondsBetweenSpawns)
        {
            CreatePellet();
            previousSpawnTime = Time.timeSinceLevelLoad;
        }
    }
    void CreatePellet()
    {
        GameObject newPellet = Instantiate(foodTypes[Random.Range(0, 2)], new Vector2(Random.Range(-60, 60), Random.Range(-60, 60)), transform.rotation);
        newPellet.transform.parent = transform;
    }
}

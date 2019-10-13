using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataHandling : MonoBehaviour
{
    public static List<Creature.Traits> creatureData = new List<Creature.Traits>();
    static int iD;
    string path;
    float secondsBtwSaves = 20;

    float previousSaveTime = 0;

    int frameCounter = 0;
    float timeCounter = 0f;
    float lastFramerate = 0f;
    
    int frames = 0;

    private void Update()
    {
        frames++;
        GetFrameRate(0.5f);
        if (lastFramerate > 5)
        {
            if (Time.timeSinceLevelLoad - previousSaveTime >= secondsBtwSaves || Time.timeSinceLevelLoad == 0)
            {
                previousSaveTime = Time.timeSinceLevelLoad;
                DataOut(Mathf.RoundToInt(Time.timeSinceLevelLoad));

                frames = 0;
            }
        }
        else
        {
            if (Time.timeSinceLevelLoad - previousSaveTime >= secondsBtwSaves / lastFramerate * 60 || Time.timeSinceLevelLoad == 0)
            {
                Debug.LogWarning("EXTREME FRAME LOSS");
                previousSaveTime = Time.timeSinceLevelLoad + (frames / 60);
                DataOut(Mathf.RoundToInt(Time.timeSinceLevelLoad + (frames / 60)));
            }
        }
    }
    void GetFrameRate(float refreshTime)
    {
        if (timeCounter < refreshTime)
        {
            timeCounter += Time.deltaTime;
            frameCounter++;
        }
        else
        {
            lastFramerate = ((float)frameCounter / timeCounter);
            frameCounter = 0;
            timeCounter = 0f;
        }
    }
    void DataOut(int timeStamp)
    {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
        {
            file.WriteLine(timeStamp.ToString() + ","
                + FindMax("generation") + ","
                + creatureData.Count.ToString() + ","
                + FindMean("isMale") + ","
                + FindMean("size") + ","
                + FindMean("speed") + ","
                + FindMean("viewRadius") + ","
                + FindMean("viewAngle") + ","
                + FindMean("vBoostLikelihood") + ","
                + FindMean("vBoostStrength") + ","
                + FindMean("maleEnergyToOffspring") + ","
                + FindMean("femaleEnergyToOffspring") + ","
                + FindMean("mTFOffspringRatio") + ","
                + FindMean("maleDesirability") + ","
                + FindMean("femaleStandards") + ","
                + FindMean("femaleGestationLength") + ","
                + FindMean("meatToVeggieDigestionEfficiencyRatio") + ","
                + FindMean("matingEnergyThreshold") + ","
                + FindMean("boredomThreshold") + ","
                + FindMean("exploreMultiplier") + ",");
            file.Close();
        }
        Debug.Log("wrote");
    }
    string FindMax(string trait)
    {
        float max = 0;

        for (int i = 0; i < creatureData.Count; i++)
        {
            switch (trait)
            {
                case "generation":
                    max = (creatureData[i].generation > max) ? creatureData[i].generation : max;
                    break;
            }
        }

        return max.ToString(); 
    }
    string FindMean(string trait)
    {
        float mean = 0;
        float total = 0;

        for (int i = 0; i < creatureData.Count; i++)
        {
            switch (trait)
            {
                case "generation":
                    total += creatureData[i].generation;
                    break;
                case "isMale":
                    total += (creatureData[i].isMale) ? 1 : 0;
                    break;
                case "size":
                    total += creatureData[i].size;
                    break;
                case "speed":
                    total += creatureData[i].movementSpeed;
                    break;
                case "viewRadius":
                    total += creatureData[i].viewRadius;
                    break;
                case "viewAngle":
                    total += creatureData[i].viewAngle;
                    break;
                case "vBootLikelihood":
                    total += creatureData[i].vBoostLikelihood;
                    break;
                case "vBoostStrength":
                    total += creatureData[i].vBoostStrength;
                    break;
                case "maleEnergyToOffspring":
                    total += creatureData[i].maleEnergyToOffspring;
                    break;
                case "femaleEnergyToOffspring":
                    total += creatureData[i].energyPercentToOfspring;
                    break;
                case "mTFOffspringRatio":
                    total += creatureData[i].maleToFemaleOffspringRatio;
                    break;
                case "maleDesirability":
                    total += creatureData[i].maledesirability;
                    break;
                case "femaleStandards":
                    total += creatureData[i].femaleStandards;
                    break;
                case "femaleGestationLength":
                    total += creatureData[i].femaleGestationLength;
                    break;
                case "meatToVeggieDigestionEfficiencyRatio":
                    total += creatureData[i].meatToVeggieDigestionEfficiencyRatio;
                    break;
                case "matingEnergyThreshold":
                    total += creatureData[i].matingEnergyThreshold;
                    break;
                case "boredomThreshold":
                    total += creatureData[i].boredomThreshold;
                    break;
                case "exploreMultiplier":
                    total += creatureData[i].exploreMultiplier;
                    break;
            }
        }
        mean = total / creatureData.Count;
        return mean.ToString();
    }
    // Remove a creature's stats by its unique ID
    public static void RemoveCreature(int creatureID)
    {
        foreach (Creature.Traits creature in creatureData)
        {
            if (creature.creatureID == creatureID)
            {
                creatureData.Remove(creature);
                break;
            }
        }
    }
    // Give a newly created creature a unique ID
    public static int GiveID()
    {
        iD++;
        return iD;
    }
    private void Start()
    {
        path = Application.dataPath + "/DATA/Simulation_Data" + System.DateTime.Now.ToString("yyyy-MM-ddTHH.mm.ss") + ".csv";
        File.Create(path).Dispose();
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
        {
            file.WriteLine("TIME(S),GENERATION,COUNT,MALE%,SIZE,SPEED,V_RADIUS,V_ANGLE,VA_BOOST_LIKELIHOOD,VA_BOOST_STRENGTH,M_ENERGY_TO_OFFSPRING%,F_ENERGY_TO_OFFSPRING%,MTF_OFFSPRING%,M_DESIRABILITY%,F_STANDARDS%,F_GESTATION_PERIOD,MTV_DIG%,MATE_ENERGY_THRESH,BOREDOM_THRESH,EXPLOREMTP");
            file.Close();
        }
        iD = 0;
    }
}

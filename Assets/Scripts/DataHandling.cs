using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataHandling : MonoBehaviour
{
    public List<Creature.Traits> creatureData;
    int iD;

    void DataOut()
    {
        string path = Application.dataPath + "/Simulation_Data" + System.DateTime.Now.ToString("yyyy-MM-ddTHH.mm.ss") + ".txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, System.DateTime.Now.ToString());
        }
    }
    public void RemoveCreature(int creatureID)
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
    public int GiveID()
    {
        iD++;
        return iD;
    }
    private void Start()
    {
        
        iD = 0;
    }
}

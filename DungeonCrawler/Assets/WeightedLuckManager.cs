using System.Collections.Generic;
using UnityEngine;

/* Example of how to use this wRng Manager in a monobehavior script:

using UnityEngine;

public class Toy : MonoBehaviour
{
    void Start()
    {
        // Add toys to the luck table
        WeightedLuckManager.Instance.Append("Toy", "Action Figure", 40);
        WeightedLuckManager.Instance.Append("Toy", "Doll", 35);
        WeightedLuckManager.Instance.Append("Toy", "Puzzle", 25);

        // get + print a random toy
        Debug.Log("Random Toy: " + WeightedLuckManager.Instance.Get("Toy"));
    }
}

*/


public class WeightedLuckManager
{
    // C# Singleton instance!
    private static WeightedLuckManager _instance;
    public static WeightedLuckManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new WeightedLuckManager();
            }
            return _instance;
        }
    }

    // dict to store multiple luck tables, each identified by a string category.
    private Dictionary<string, LuckTable> luckTables = new Dictionary<string, LuckTable>();

    // prevent external instantiation. Init other stuff here.
    private WeightedLuckManager() {}

    // Add/update a value in a specific luck table
    public void Append(string category, string key, float value)
    {
        if (!luckTables.ContainsKey(category))
        {
            luckTables[category] = new LuckTable();
        }

        luckTables[category].Append(key, value);
    }

    // Get a random key from a luck table
    public string Get(string category)
    {
        if (luckTables.ContainsKey(category))
        {
            return luckTables[category].Get();
        }

        Debug.LogWarning($"Category '{category}' does not exist!");
        return null;
    }

    public List<string> GetWithoutReplacement(string category, int iterations){
        if (iterations <= 0) return null;
        List<string> RemovedKeys = new List<string>();
        float[] RemovedValues = new float[iterations];
        for(int i = 0; i < iterations; i++){
            RemovedKeys.Add(Get(category));
            RemovedValues[i] = luckTables[category].GetWeight(RemovedKeys[i]);
            Remove(category, RemovedKeys[i]);
        }
        for(int i = 0; i < iterations; i++){
            Append(category, RemovedKeys[i], RemovedValues[i]);
        }
        return RemovedKeys;
    }

    public bool Remove(string category, string LTkey){
        if(luckTables.ContainsKey(category)){
            return luckTables[category].Remove(LTkey);
        } return false;
    }

    // clears a category's table
    public void Clear(string category){
        if(luckTables.ContainsKey(category)){
            luckTables[category].Clear(); return;
        }
        Debug.Log("failed to clear or table is empty.");
    }

    // Prints a specific luck table for debugging use.
    public void PrintTable(string category)
    {
        if (luckTables.ContainsKey(category))
        {
            Debug.Log($"Luck Table for Category '{category}':");
            luckTables[category].PrintTable();
        }
        else
        {
            Debug.LogWarning($"Category '{category}' does not exist!");
        }
    }

    // class to represent a single luck table
    private class LuckTable
    {
        private Dictionary<string, float> table = new Dictionary<string, float>();
        private float totalWeight = 0;

        public void Append(string key, float value)
        {
            if (table.ContainsKey(key))
            {
                totalWeight -= table[key];
                table[key] = value; 
            }
            else
            {
                table[key] = value;
            }

            totalWeight += value; 
        }

        public string Get()
        {
            if (table.Count == 0)
            {
                Debug.LogWarning("Luck table is empty!");
                return null;
            }

            float randomValue = Random.Range(0, totalWeight);
            float cumulativeWeight = 0;

            foreach (var entry in table)
            {
                cumulativeWeight += entry.Value;
                if (randomValue < cumulativeWeight)
                {
                    return entry.Key;
                }
            }

            Debug.LogError("Weighted selection failed!");
            return null;
        }
        public float GetWeight(string key){
            return table[key];
        }
        public bool Remove(string key){
            if(table.ContainsKey(key)){
                totalWeight -= table[key];
                return table.Remove(key);
            } return false;
        }
        public void Clear(){
            table.Clear();
            totalWeight = 0;
        }
        // Internal print debugger
        public void PrintTable()
        {
            foreach (var entry in table)
            {
                Debug.Log($"Name: {entry.Key}, Weight: {entry.Value}");
            }
            Debug.Log($"Total Weight: {totalWeight}");
        }
    }
}

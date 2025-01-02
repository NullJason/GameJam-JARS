using System.Collections.Generic;
using UnityEngine;

/* Example of how to use this wRng Manager in another monobehavior:

using UnityEngine;

public class Toy : MonoBehaviour
{
    void Start()
    {
        // Add toys to the luck table
        WeightedLuckManager.Instance.Append("Toy", "Action Figure", 40);
        WeightedLuckManager.Instance.Append("Toy", "Doll", 35);
        WeightedLuckManager.Instance.Append("Toy", "Puzzle", 25);

        // Get a random toy
        Debug.Log("Random Toy: " + WeightedLuckManager.Instance.Get("Toy"));
    }
}

*/

public class WeightedLuckManager : MonoBehaviour
{
    // Singleton instance
    public static WeightedLuckManager Instance;

    // Dictionary to store multiple luck tables, each identified by a category.
    private Dictionary<string, LuckTable> luckTables = new Dictionary<string, LuckTable>();

    private void Awake()
    {
        // Ensure there's only one manager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this manager alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Add or update a value in a specific luck table
    public void Append(string category, string key, float value)
    {
        if (!luckTables.ContainsKey(category))
        {
            // Create a new luck table if it doesn't exist for this category
            luckTables[category] = new LuckTable();
        }

        // Add or update the key in the corresponding table
        luckTables[category].Append(key, value);
    }

    // Get a random key from a specific luck table
    public string Get(string category)
    {
        if (luckTables.ContainsKey(category))
        {
            return luckTables[category].Get();
        }

        Debug.LogWarning($"Category '{category}' does not exist!");
        return null;
    }

    // Prints a specific luck table for debugging.
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

    // private class to represent a single luck table
    private class LuckTable
    {
        private Dictionary<string, float> table = new Dictionary<string, float>();
        private float totalWeight = 0;

        public void Append(string key, float value)
        {
            if (table.ContainsKey(key))
            {
                // Update existing key
                totalWeight -= table[key]; // Subtract old weight
                table[key] = value; // Update weight
            }
            else
            {
                // Add new key
                table[key] = value;
            }

            totalWeight += value; // Add new weight
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
        
        // internal print debugger.
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

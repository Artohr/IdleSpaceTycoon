using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    // stocke les montants actuels
    private Dictionary<string, double> amounts = new Dictionary<string, double>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public double Get(string resourceName)
    {
        if (!amounts.ContainsKey(resourceName)) amounts[resourceName] = 0.0;
        return amounts[resourceName];
    }

    public void Set(string resourceName, double value)
    {
        amounts[resourceName] = value;
    }

    public void Add(string resourceName, double delta)
    {
        if (!amounts.ContainsKey(resourceName)) amounts[resourceName] = 0.0;
        amounts[resourceName] += delta;
    }

    public Dictionary<string, double> GetAllSnapshot()
    {
        return new Dictionary<string, double>(amounts);
    }

    // helper pour debugging / editor
    public void DebugLogAll()
    {
        foreach (var kv in amounts) Debug.Log($"{kv.Key} = {kv.Value}");
    }
}

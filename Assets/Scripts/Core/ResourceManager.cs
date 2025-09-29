using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    // Événement : notifie quand une ressource change
    public event Action<string, double> OnResourceChanged;

    // Stock des ressources
    private Dictionary<string, double> amounts = new Dictionary<string, double>();

    // Liste des planètes actives (tu assignes ça via GameManager)
    public List<PlanetData> activePlanets = new List<PlanetData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Core ---
    public double Get(string resourceName)
    {
        if (!amounts.ContainsKey(resourceName))
            amounts[resourceName] = 0.0;
        return amounts[resourceName];
    }

    public void Set(string resourceName, double value)
    {
        value = Math.Max(0, value); // clamp à 0
        amounts[resourceName] = value;
        OnResourceChanged?.Invoke(resourceName, value);
    }

    public void Add(string resourceName, double delta)
    {
        if (!amounts.ContainsKey(resourceName))
            amounts[resourceName] = 0.0;

        amounts[resourceName] = Math.Max(0, amounts[resourceName] + delta);
        OnResourceChanged?.Invoke(resourceName, amounts[resourceName]);
    }

    // --- Production ---
    void Update()
    {
        double delta = Time.deltaTime; // temps écoulé depuis le dernier frame

        foreach (var planet in activePlanets)
        {
            // Commun
            double commonProd = planet.commonProdPerSecond * (1 + UpgradeManager.Instance.GetLevel(planet, false));
            commonProd = SpecialMechanics.ApplySpecial(planet, planet.commonResourceName, commonProd);
            Add(planet.commonResourceName, commonProd * delta);

            // Rare
            double rareProd = planet.rareProdPerSecond * (1 + UpgradeManager.Instance.GetLevel(planet, true));
            rareProd = SpecialMechanics.ApplySpecial(planet, planet.rareResourceName, rareProd);
            Add(planet.rareResourceName, rareProd * delta);
        }
    }

    // --- Utils ---
    public Dictionary<string, double> GetAllSnapshot()
    {
        return new Dictionary<string, double>(amounts);
    }

    public void DebugLogAll()
    {
        foreach (var kv in amounts)
            Debug.Log($"{kv.Key} = {kv.Value}");
    }
}

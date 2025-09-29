using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Progression des planètes")]
    public PlanetData[] planets; // assignées dans l'inspector
    public int currentPlanetIndex = 0;

    [Header("Offline Earnings")]
    public double offlineCapHours = 12.0;
    public double offlineBonusMultiplier = 3.0;

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

    void Start()
    {
        // Au démarrage : active la première planète
        if (planets.Length > 0 && ResourceManager.Instance != null)
        {
            ResourceManager.Instance.activePlanets.Clear();
            ResourceManager.Instance.activePlanets.Add(planets[currentPlanetIndex]);
        }
    }

    // --- Progression ---
    public void UnlockNextPlanet()
    {
        if (currentPlanetIndex < planets.Length - 1)
        {
            currentPlanetIndex++;
            PlanetData next = planets[currentPlanetIndex];
            if (!ResourceManager.Instance.activePlanets.Contains(next))
            {
                ResourceManager.Instance.activePlanets.Add(next);
                Debug.Log($"Nouvelle planète débloquée : {next.planetName}");
            }
        }
    }

    // --- Calcul offline ---
    public (double commonEarned, double rareEarned) CalculateOfflineEarnings(PlanetData planet, double lastSecondsElapsed)
    {
        // cap
        double capSeconds = offlineCapHours * 3600.0;
        double seconds = Math.Min(lastSecondsElapsed, capSeconds);

        // Prod de base
        double commonBase = planet.commonProdPerSecond * seconds;
        double rareBase = planet.rareProdPerSecond * seconds;

        // Appliquer upgrades
        double commonMult = 1 + ResourceManager.Instance.GetLevel(planet.commonResourceName);
        double rareMult = 1 + ResourceManager.Instance.GetLevel(planet.rareResourceName);

        // Appliquer mécaniques spéciales
        commonBase = SpecialMechanics.ApplySpecial(planet, planet.commonResourceName, commonBase);
        rareBase = SpecialMechanics.ApplySpecial(planet, planet.rareResourceName, rareBase);

        // Appliquer bonus offline
        commonBase *= commonMult * offlineBonusMultiplier;
        rareBase *= rareMult * offlineBonusMultiplier;

        return (commonBase, rareBase);
    }
}

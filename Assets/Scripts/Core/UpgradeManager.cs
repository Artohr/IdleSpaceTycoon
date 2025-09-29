using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    // Niveaux d’upgrade sérialisables via un simple Dictionary<string,int>
    // Clé = "<PlanetName>:common" ou "<PlanetName>:rare"
    private Dictionary<string, int> levelsById = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // -------- Helpers d'identifiant --------
    private static string MakeKey(PlanetData planet, bool isRare)
    {
        if (planet == null) return null;
        return $"{planet.planetName}:{(isRare ? "rare" : "common")}";
    }

    // -------- Get / Set Level --------
    public int GetLevel(PlanetData planet, bool isRare)
    {
        var key = MakeKey(planet, isRare);
        if (string.IsNullOrEmpty(key)) return 0;

        if (!levelsById.TryGetValue(key, out var lvl))
        {
            lvl = 0;
            levelsById[key] = lvl; // initialise à 0
        }
        return lvl;
    }

    public void SetLevel(PlanetData planet, bool isRare, int newLevel)
    {
        var key = MakeKey(planet, isRare);
        if (string.IsNullOrEmpty(key)) return;

        levelsById[key] = Mathf.Max(0, newLevel);
    }

    // -------- Coûts & Achat --------
    // Formule GDD : cost = baseCost * (costMultiplier ^ level)
    public double GetUpgradeCost(PlanetData planet, bool isRare)
    {
        int level = GetLevel(planet, isRare);
        double baseCost = isRare ? planet.rareBaseUpgradeCost : planet.commonBaseUpgradeCost;
        double mult = isRare ? planet.rareCostMultiplier : planet.commonCostMultiplier;
        return baseCost * System.Math.Pow(mult, level);
    }

    // Incrémente juste le niveau (le paiement doit être fait côté bouton)
    public void ApplyUpgrade(PlanetData planet, bool isRare)
    {
        int lvl = GetLevel(planet, isRare);
        SetLevel(planet, isRare, lvl + 1);
    }

    // -------- Snapshots pour SaveManager --------
    public Dictionary<string, int> GetAllLevelsSnapshot()
    {
        // Copie défensive
        return new Dictionary<string, int>(levelsById);
    }

    public void RestoreLevels(Dictionary<string, int> snapshot)
    {
        levelsById.Clear();
        if (snapshot == null) return;

        foreach (var kv in snapshot)
        {
            // Clamp par sécurité
            levelsById[kv.Key] = Mathf.Max(0, kv.Value);
        }
    }
}

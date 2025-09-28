using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Serializable]
    public class UpgradeState
    {
        public string id; // ex "Lune_common_minor" (unique)
        public int level;
        public double baseMultiplierPerLevel = 2.0; // multiplicateur par niveau (ex: 2 => level1 x2, level2 x4)
        public double baseCost = 50.0;
        public double costMultiplier = 1.15;
    }

    public List<UpgradeState> upgrades = new List<UpgradeState>();
    private Dictionary<string, UpgradeState> map;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        map = new Dictionary<string, UpgradeState>();
        foreach (var u in upgrades) map[u.id] = u;
    }

    public int GetLevel(string id)
    {
        if (map.TryGetValue(id, out var s)) return s.level;
        return 0;
    }

    public void LevelUp(string id)
    {
        if (map.TryGetValue(id, out var s))
        {
            s.level++;
        }
        else
        {
            Debug.LogWarning($"Upgrade id not found: {id}");
        }
    }

    public double GetMultiplier(string id)
    {
        if (map.TryGetValue(id, out var s))
        {
            // multiplier = baseMultiplierPerLevel ^ level
            return Math.Pow(s.baseMultiplierPerLevel, s.level);
        }
        return 1.0;
    }

    public double GetUpgradeCost(string id)
    {
        if (map.TryGetValue(id, out var s))
        {
            // cost = baseCost * costMultiplier ^ level
            return s.baseCost * Math.Pow(s.costMultiplier, s.level);
        }
        return double.PositiveInfinity;
    }
}

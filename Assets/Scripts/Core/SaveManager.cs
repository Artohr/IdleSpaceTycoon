using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // Ressources
    public List<string> resourceKeys = new();
    public List<double> resourceValues = new();

    // Upgrades
    public List<string> upgradeKeys = new();
    public List<int> upgradeValues = new();

    // Planètes
    public int currentPlanetIndex;
    public List<string> unlockedPlanets = new();

    public long lastSaveTimestamp; // UNIX secs
}


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private const string SAVE_KEY = "IdleSave";

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // Sauvegarde
    public void SaveGame()
    {
        SaveData data = new SaveData();

        // --- Ressources ---
        var snap = ResourceManager.Instance.GetAllSnapshot();
        foreach (var kv in snap) { data.resourceKeys.Add(kv.Key); data.resourceValues.Add(kv.Value); }


        // --- Upgrades ---
        var ups = UpgradeManager.Instance.GetAllLevelsSnapshot(); // Dictionary<string,int>
        foreach (var kv in ups) { data.upgradeKeys.Add(kv.Key); data.upgradeValues.Add(kv.Value); }

        // --- Planètes ---
        data.currentPlanetIndex = GameManager.Instance.currentPlanetIndex;
        foreach (var planet in ResourceManager.Instance.activePlanets)
        {
            data.unlockedPlanets.Add(planet.planetName);
        }

        // --- Timestamp ---
        data.lastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Sérialisation JSON
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("Sauvegarde effectuée !");
    }

    // Chargement
    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("Pas de sauvegarde existante, nouveau jeu !");
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Ressources
        for (int i = 0; i < data.resourceKeys.Count; i++)
            ResourceManager.Instance.Set(data.resourceKeys[i], data.resourceValues[i]);

        // Upgrades
        var restore = new Dictionary<string, int>();
        for (int i = 0; i < data.upgradeKeys.Count; i++)
            restore[data.upgradeKeys[i]] = data.upgradeValues[i];
        UpgradeManager.Instance.RestoreLevels(restore);

        // --- Planètes débloquées ---
        GameManager.Instance.currentPlanetIndex = data.currentPlanetIndex;
        ResourceManager.Instance.activePlanets.Clear();
        foreach (var planet in GameManager.Instance.planets)
        {
            if (data.unlockedPlanets.Contains(planet.planetName))
            {
                ResourceManager.Instance.activePlanets.Add(planet);
            }
        }

        // --- Offline Earnings ---
        double elapsed = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - data.lastSaveTimestamp;
        if (elapsed > 10) // on calcule seulement si > 10 sec
        {
            PlanetData current = GameManager.Instance.planets[data.currentPlanetIndex];
            var (commonEarned, rareEarned) = GameManager.Instance.CalculateOfflineEarnings(current, elapsed);

            ResourceManager.Instance.Add(current.commonResourceName, commonEarned);
            ResourceManager.Instance.Add(current.rareResourceName, rareEarned);

            Debug.Log($"Gains offline ajoutés ({elapsed} sec) : +{commonEarned} {current.commonResourceName}, +{rareEarned} {current.rareResourceName}");
        }
    }
}

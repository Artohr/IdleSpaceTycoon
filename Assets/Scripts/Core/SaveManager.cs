using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string[] resourceKeys;
    public string[] resourceValues; // stockées en string pour précision
    public string[] upgradeIds;
    public int[] upgradeLevels;
    public int currentPlanetIndex;
    public long lastSaveUnix; // seconds since epoch
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string saveFileName = "save.json";

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void SaveAll()
    {
        var sd = new SaveData();

        // ressources
        var snapshot = ResourceManager.Instance.GetAllSnapshot();
        sd.resourceKeys = new string[snapshot.Count];
        sd.resourceValues = new string[snapshot.Count];
        int i = 0;
        foreach (var kv in snapshot)
        {
            sd.resourceKeys[i] = kv.Key;
            sd.resourceValues[i] = kv.Value.ToString(CultureInfo.InvariantCulture);
            i++;
        }

        // upgrades (take from UpgradeManager.upgrades)
        var ups = UpgradeManager.Instance.upgrades;
        sd.upgradeIds = new string[ups.Count];
        sd.upgradeLevels = new int[ups.Count];
        for (int j = 0; j < ups.Count; j++)
        {
            sd.upgradeIds[j] = ups[j].id;
            sd.upgradeLevels[j] = ups[j].level;
        }

        sd.currentPlanetIndex = GameManager.Instance.currentPlanetIndex;
        sd.lastSaveUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        string json = JsonUtility.ToJson(sd);
        System.IO.File.WriteAllText(System.IO.Path.Combine(Application.persistentDataPath, saveFileName), json);
        Debug.Log("Saved to " + Application.persistentDataPath);
    }

    public void LoadAll()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, saveFileName);
        if (!System.IO.File.Exists(path))
        {
            Debug.Log("No save found.");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        var sd = JsonUtility.FromJson<SaveData>(json);

        // restore resources
        for (int i = 0; i < sd.resourceKeys.Length; i++)
        {
            string key = sd.resourceKeys[i];
            string val = sd.resourceValues[i];
            double d = 0.0;
            double.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out d);
            ResourceManager.Instance.Set(key, d);
        }

        // restore upgrades
        for (int j = 0; j < sd.upgradeIds.Length; j++)
        {
            string id = sd.upgradeIds[j];
            int lvl = sd.upgradeLevels[j];
            var ups = UpgradeManager.Instance.upgrades;
            var u = ups.Find(x => x.id == id);
            if (u != null) u.level = lvl;
        }

        // restore planet index
        GameManager.Instance.currentPlanetIndex = sd.currentPlanetIndex;

        // offline calc
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long elapsed = now - sd.lastSaveUnix;
        if (elapsed > 0)
        {
            var planet = GameManager.Instance.planets[GameManager.Instance.currentPlanetIndex];
            var (commonEarned, rareEarned) = GameManager.Instance.CalculateOfflineEarnings(planet, elapsed);
            ResourceManager.Instance.Add(planet.commonResourceName, commonEarned);
            ResourceManager.Instance.Add(planet.rareResourceName, rareEarned);
            Debug.Log($"Offline: {commonEarned} {planet.commonResourceName}, {rareEarned} {planet.rareResourceName}");
        }
    }
}

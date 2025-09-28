using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlanetData[] planets; // assigner dans l'inspector (ordre progression)
    public int currentPlanetIndex = 0;
    public double offlineCapHours = 12.0;
    public double offlineBonusMultiplier = 3.0;

    private double accumulatedDelta = 0.0;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        // au démarrage on devra charger le save (SaveManager fera l'appel)
    }

    void Update()
    {
        double delta = Time.deltaTime;
        accumulatedDelta += delta;

        // mise à jour continue : ajoute production * delta
        ProducePerFrame(delta);
    }

    void ProducePerFrame(double deltaSeconds)
    {
        var planet = planets[currentPlanetIndex];

        // calcule multiplicateurs à partir de UpgradeManager (IDs suivant convention)
        double commonMultiplier = 1.0;
        double rareMultiplier = 1.0;

        // convention d'ids
        commonMultiplier *= UpgradeManager.Instance.GetMultiplier($"{planet.planetName}_common_minor");
        commonMultiplier *= UpgradeManager.Instance.GetMultiplier($"{planet.planetName}_common_major");

        rareMultiplier *= UpgradeManager.Instance.GetMultiplier($"{planet.planetName}_rare_minor");
        rareMultiplier *= UpgradeManager.Instance.GetMultiplier($"{planet.planetName}_rare_major");

        // production par seconde * deltaSeconds
        double commonProduced = planet.commonProdPerSecond * commonMultiplier * deltaSeconds;
        double rareProduced = planet.rareProdPerSecond * rareMultiplier * deltaSeconds;

        ResourceManager.Instance.Add(planet.commonResourceName, commonProduced);
        ResourceManager.Instance.Add(planet.rareResourceName, rareProduced);
    }

    // CALCUL OFFLINE -> appelé par SaveManager quand on charge une sauvegarde
    public (double commonEarned, double rareEarned) CalculateOfflineEarnings(PlanetData planet, double lastSecondsElapsed)
    {
        // cap
        double capSeconds = offlineCapHours * 3600.0;
        double seconds = Math.Min(lastSecondsElapsed, capSeconds);

        // multiplicateurs
        double commonMultiplier = UpgradeManager.Instance.GetMultiplier($"{planet.planetName}_common_minor")
                                  * UpgradeManager.Instance.GetMultiplier($"{planet.planetName}_common_major");
        double rareMultiplier = UpgradeManager.Instance.GetMultiplier($"{planet.planetName}_rare_minor")
                                * UpgradeManager.Instance.GetMultiplier($"{planet.planetName}_rare_major");

        double commonHourly = planet.commonProdPerSecond * commonMultiplier * seconds;
        double rareHourly = planet.rareProdPerSecond * rareMultiplier * seconds;

        // appliquer bonus offline
        commonHourly *= offlineBonusMultiplier;
        rareHourly *= offlineBonusMultiplier;

        return (commonHourly, rareHourly);
    }
}

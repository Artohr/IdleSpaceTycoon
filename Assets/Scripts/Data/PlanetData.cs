using UnityEngine;
public enum SpecialMechanicType
{
    None,
    SolarBoost,   // Lune
    Terraforming, // Mars
    DeepMining,   // Europa
    MethaneBonus  // Titan
}

[CreateAssetMenu(menuName = "Idle/PlanetData", fileName = "PlanetData")]
public class PlanetData : ScriptableObject
{
    public string planetName;

    [Header("Ressource commune")]
    public string commonResourceName;
    public double commonProdPerSecond = 1.0; // unit�s / sec
    public double commonBaseUpgradeCost = 50.0;
    public double commonCostMultiplier = 1.15;

    [Header("Ressource rare")]
    public string rareResourceName;
    public double rareProdPerSecond = 0.1; // unit�s / sec
    public double rareBaseUpgradeCost = 200.0;
    public double rareCostMultiplier = 1.20;

    [Header("Progression")]
    public double rareToUnlockNext = 200.0; // quantit� rare requise pour d�bloquer la plan�te suivante

    [Header("M�canique sp�ciale")]
    public SpecialMechanicType specialMechanic = SpecialMechanicType.None;
    public bool hasSpecialBoost = false;

}

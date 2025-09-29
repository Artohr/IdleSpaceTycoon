using UnityEngine;

public static class SpecialMechanics
{
    // Ici tu pourrais stocker les �tats (ex : est-ce que le joueur a achet� le panneau solaire ?)
    // Pour le MVP on met une simple variable statique
    public static bool solarBoostUnlocked = false;
    public static bool terraformingProgress = false;
    public static bool deepMiningUnlocked = false;

    public static double ApplySpecial(PlanetData planet, string resourceName, double baseProduction)
    {
        switch (planet.specialMechanic)
        {
            case SpecialMechanicType.SolarBoost:
                // Lune : boost x2 sur ressource commune (Eau gel�e)
                if (solarBoostUnlocked && resourceName == planet.commonResourceName)
                    return baseProduction * 2;
                break;

            case SpecialMechanicType.Terraforming:
                // Mars : production commune et rare augment�e si terraformage activ�
                if (terraformingProgress)
                    return baseProduction * 1.5; // exemple
                break;

            case SpecialMechanicType.DeepMining:
                // Europa : rare ? x3 mais seulement si forage d�bloqu�
                if (deepMiningUnlocked && resourceName == planet.rareResourceName)
                    return baseProduction * 3;
                break;

            case SpecialMechanicType.MethaneBonus:
                // Titan : ressource rare (M�thane) a + valeur marchande
                // Ici tu peux affecter la conversion argent ou multiplier prod
                if (resourceName == planet.rareResourceName)
                    return baseProduction * 1.2;
                break;

            default:
                break;
        }

        return baseProduction;
    }
}


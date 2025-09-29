using UnityEngine;
using UnityEngine.UI;

public class SpecialUpgradeButton : MonoBehaviour
{
    public PlanetData planet;
    public Button button;

    void Start()
    {
        button.onClick.AddListener(ActivateSpecial);
    }

    void Update()
    {
        // Bouton actif uniquement si la plan�te a une m�canique sp�ciale
        button.interactable = planet.hasSpecialBoost;
    }

    void ActivateSpecial()
    {
        if (!planet.hasSpecialBoost) return;

        // Ici tu dispatches selon le nom de la plan�te ou son type
        switch (planet.planetName)
        {
            case "Lune":
                SpecialMechanics.Instance.ActivateSolarBoost();
                break;

            case "Mars":
                SpecialMechanics.Instance.ActivateSandstorm();
                break;

            case "Europa":
                SpecialMechanics.Instance.ActivateCryoBoost();
                break;

            case "Titan":
                SpecialMechanics.Instance.ActivateMethaneFlood();
                break;

            default:
                Debug.Log("Pas de m�canique sp�ciale d�finie pour " + planet.planetName);
                break;
        }
    }
}

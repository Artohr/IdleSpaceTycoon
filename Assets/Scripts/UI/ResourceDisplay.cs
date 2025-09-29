using System.Collections;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [Tooltip("Nom exact de la ressource tel qu'il est utilisé dans ResourceManager")]
    public string resourceName;

    [Tooltip("Le composant TMP Text à mettre à jour")]
    public TMP_Text label;

    public float refreshInterval = 0.25f;
    private Coroutine _runner;

    void Start()
    {
        // S'abonner à l'événement
        ResourceManager.Instance.OnResourceChanged += HandleChange;

        // Initialiser le texte
        double startValue = ResourceManager.Instance.Get(resourceName);
        label.text = $"{resourceName}: {NumberFormatter.FormatCompact(startValue)}";
    }

    private void HandleChange(string name, double value)
    {
        if (name == resourceName)
            label.text = $"{resourceName}: {NumberFormatter.FormatCompact(value)}";
    }
}

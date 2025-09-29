using System.Collections;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    [Tooltip("Nom exact de la ressource tel qu'il est utilis� dans ResourceManager")]
    public string resourceName;

    [Tooltip("Le composant TMP Text � mettre � jour")]
    public TMP_Text label;

    public float refreshInterval = 0.25f;
    private Coroutine _runner;

    void Start()
    {
        // S'abonner � l'�v�nement
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

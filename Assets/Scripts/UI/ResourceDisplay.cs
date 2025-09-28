using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    public string resourceName;
    public TMP_Text label;

    void Update()
    {
        if (ResourceManager.Instance == null) return;
        double v = ResourceManager.Instance.Get(resourceName);
        label.text = $"{resourceName}: {NumberFormatter.FormatCompact(v)}";
    }
}

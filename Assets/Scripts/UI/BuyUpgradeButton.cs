using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyUpgradeButton : MonoBehaviour
{
    public PlanetData planet;
    public bool isRare;               // false = commune, true = rare
    public string payWithResource;    // ex: "Eau gelée" ou "Hélium-3"
    public TMP_Text labelPrice;
    public Button btn;

    void Start()
    {
        btn.onClick.AddListener(OnClick);
        Refresh();
    }

    void Update() => Refresh();

    void Refresh()
    {
        int lvl = UpgradeManager.Instance.GetLevel(planet, isRare);
        double cost = UpgradeManager.Instance.GetUpgradeCost(planet, isRare);
        labelPrice.text = $"Lvl {lvl} ? {NumberFormatter.FormatCompact(cost)}";

        double have = ResourceManager.Instance.Get(payWithResource);
        btn.interactable = have >= cost;
    }

    void OnClick()
    {
        double cost = UpgradeManager.Instance.GetUpgradeCost(planet, isRare);
        if (ResourceManager.Instance.Get(payWithResource) >= cost)
        {
            ResourceManager.Instance.Add(payWithResource, -cost);
            UpgradeManager.Instance.ApplyUpgrade(planet, isRare);
            Refresh();
        }
    }
}

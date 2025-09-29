using UnityEngine;

public class SaveTester : MonoBehaviour
{
    [ContextMenu("Sauvegarder")]
    public void SaveNow()
    {
        SaveManager.Instance.SaveAll();
        Debug.Log("Sauvegarde effectuée !");
    }

    [ContextMenu("Charger")]
    public void LoadNow()
    {
        SaveManager.Instance.LoadAll();
        Debug.Log("Chargement effectué !");
    }
}
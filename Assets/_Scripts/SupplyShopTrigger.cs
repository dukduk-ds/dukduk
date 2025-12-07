using UnityEngine;

public class SupplyShopTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (ShopUIManager.Instance != null)
            ShopUIManager.Instance.OpenShop();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (ShopUIManager.Instance != null)
            ShopUIManager.Instance.CloseShopUI();
    }
}

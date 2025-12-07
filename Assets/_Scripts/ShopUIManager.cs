using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance { get; private set; }

    [Header("가격 설정")]
    public int gunPrice = 5;
    public int breadPrice = 1;
    public int medicinePrice = 3;

    [Header("UI 연결")]
    public GameObject shopPanel;   // Panel_SupplyShop
    public bool pauseOnOpen = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ===== 상점 열기 / 닫기 =====
    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);

        if (pauseOnOpen)
            Time.timeScale = 0f;
    }

    public void CloseShopUI()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (pauseOnOpen)
            Time.timeScale = 1f;
    }

    // ===== 버튼에서 호출할 함수들 =====
    public void BuyGun()
    {
        TryBuyItem(SupplyType.Gun, gunPrice);
    }

    public void BuyBread()
    {
        TryBuyItem(SupplyType.Bread, breadPrice);
    }

    public void BuyMedicine()
    {
        TryBuyItem(SupplyType.Medicine, medicinePrice);
    }

    // ===== 실제 구매 로직 =====
    void TryBuyItem(SupplyType type, int price)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager.Instance 없음!");
            return;
        }

        int coinCount = InventoryManager.Instance.GetCount(SupplyType.Coin);
        if (coinCount < price)
        {
            Debug.Log("동전 부족!");
            return;
        }

        bool success = InventoryManager.Instance.UseItem(SupplyType.Coin, price);
        if (!success)
        {
            Debug.Log("동전 차감 실패");
            return;
        }

        InventoryManager.Instance.AddItem(type, 1);
        Debug.Log(type + " 구매 완료!");
    }
}

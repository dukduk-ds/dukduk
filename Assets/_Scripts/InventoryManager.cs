using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;   // ★ TMP 사용

[System.Serializable]
public class InventorySlotUI
{
    public SupplyType type;           // 이 슬롯이 담당하는 아이템 종류
    public Image iconImage;           // 아이콘 이미지
    public TextMeshProUGUI countText; // 개수 텍스트 (TMP)
}

[System.Serializable]
public class StartItem
{
    public SupplyType type;
    public int count = 1;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("슬롯 UI (coin, lighter, brokenPhone, earbuds, bread, med, gun, flare 등)")]
    public InventorySlotUI[] slots;

    [Header("시작 아이템 설정 (인스펙터에서 조절)")]
    public StartItem[] startItems;

    // 실제 보유 개수
    private Dictionary<SupplyType, int> itemCounts =
        new Dictionary<SupplyType, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 필요하면 유지하고 싶으면 주석 해제
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 모든 아이템 0으로 초기화
        foreach (SupplyType t in System.Enum.GetValues(typeof(SupplyType)))
        {
            itemCounts[t] = 0;
        }
    }

    private void Start()
    {
        // 🔸 시작 아이템: AddItem으로 넣기 (UI까지 같이 반영)
        if (startItems != null)
        {
            foreach (var s in startItems)
            {
                if (s.count <= 0) continue;
                AddItem(s.type, s.count);
                Debug.Log($"[Inventory] StartItem -> {s.type} x{s.count}");
            }
        }

        LogAllCounts("[Inventory] After StartItems");
        RefreshUI();
    }

    // 아이템 추가(줍거나 상점에서 살 때 호출)
    public void AddItem(SupplyType type, int amount = 1)
    {
        if (!itemCounts.ContainsKey(type))
            itemCounts[type] = 0;

        itemCounts[type] += amount;
        Debug.Log($"[Inventory] AddItem: {type} +{amount}, total={itemCounts[type]}");
        RefreshUI();
    }

    // 아이템 사용 / 차감
    public bool UseItem(SupplyType type, int amount = 1)
    {
        if (!itemCounts.ContainsKey(type)) return false;
        if (itemCounts[type] < amount) return false;

        itemCounts[type] -= amount;
        Debug.Log($"[Inventory] UseItem: {type} -{amount}, total={itemCounts[type]}");
        RefreshUI();
        return true;
    }

    public int GetCount(SupplyType type)
    {
        return itemCounts.ContainsKey(type) ? itemCounts[type] : 0;
    }

    // 🔥 UI에 숫자 / 아이콘 반영
    private void RefreshUI()
    {
        if (slots == null) return;

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            int count = GetCount(slot.type);

            // 👉 0개면 숫자 숨기고, 1개 이상이면 숫자 표시
            if (slot.countText != null)
            {
                slot.countText.text = count > 0 ? count.ToString() : "";
            }

            // 👉 0개면 아이콘 숨기고, 1개 이상이면 아이콘 보이게
            if (slot.iconImage != null)
            {
                slot.iconImage.enabled = (count > 0);
            }
        }
    }

    // 아이템 전체 개수 로그 찍어보기용
    private void LogAllCounts(string prefix)
    {
        foreach (SupplyType t in System.Enum.GetValues(typeof(SupplyType)))
        {
            int c = GetCount(t);
            Debug.Log($"{prefix} | {t} = {c}");
        }
    }
}

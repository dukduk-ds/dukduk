using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    [Header("어떤 아이템인지")]
    public SupplyType itemType;

    [Header("인벤토리에 추가될 개수")]
    public int amount = 1;

    [Header("줍는 순간 체력/배고픔 회복량 (옵션)")]
    public float hpHeal = 0f;       // 약일 때 사용
    public float hungerHeal = 0f;   // 빵일 때 사용

    [Header("줍을 때 오브젝트 삭제할지")]
    public bool destroyOnPickup = true;

    private bool playerInRange = false;
    private PlayerStatus playerStatus;

    void Reset()
    {
        // 자동으로 Trigger 권장
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnValidate()
    {
        // 에디터에서 실수로 isTrigger 풀리는 것 방지
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerStatus = other.GetComponent<PlayerStatus>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerStatus = null;
        }
    }

    void Update()
    {
        if (!playerInRange) return;

        // Space로 줍기
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        // 1) 스텟 회복
        if (playerStatus != null)
        {
            if (hpHeal != 0f)
                playerStatus.AddHP(hpHeal);

            if (hungerHeal != 0f)
                playerStatus.AddHunger(hungerHeal);
        }

        // 2) 인벤토리에 추가
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemType, amount);
        }
        else
        {
            Debug.LogWarning("[ItemPickup] InventoryManager.Instance 없음!");
        }

        // 3) (선택) 미션 매니저에 보고하고 싶으면 여기서 호출

        // 4) 오브젝트 삭제
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}

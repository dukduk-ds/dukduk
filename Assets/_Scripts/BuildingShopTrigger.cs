using UnityEngine;

public class BuildingShopTrigger : MonoBehaviour
{
    [Header("상점 대화창 UI")]
    public DialogueUI dialogueUI;   // 상점 UI(패널)에 붙어 있는 DialogueUI 스크립트

    [Header("한 번만 열리게 할지 여부")]
    public bool openOnlyOnce = false;

    private bool hasOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 아니면 무시
        if (!other.CompareTag("Player")) return;

        // 한 번만 열리게 옵션 켰고 이미 열렸으면 무시
        if (openOnlyOnce && hasOpened) return;

        if (dialogueUI != null)
        {
            Debug.Log("[ShopTrigger] 상점 UI Open() 호출");

            dialogueUI.Open();
            hasOpened = true;
        }
        else
        {
            Debug.LogWarning("[BuildingShopTrigger] dialogueUI가 인스펙터에 연결되지 않았어요!");
        }
    }

    // 나갈 때 자동으로 닫고 싶으면 여기서 dialogueUI.OnClickClose() 호출해도 됨
    // private void OnTriggerExit(Collider other) { ... }
}

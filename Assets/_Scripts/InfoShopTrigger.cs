using UnityEngine;

public class InfoShopTrigger : MonoBehaviour
{
    public InfoDialogueUI dialogueUI;      // 정보상 UI 연결
    public string[] infoLines;            // 대사 넣기
    public string npcName = "정보상 NPC"; // 이름
    public Sprite npcPortrait;            // 초상화(선택)

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // UI 연결 체크
        if (dialogueUI == null)
        {
            Debug.LogError("[InfoShopTrigger] dialogueUI가 인스펙터에 연결 안 됨!");
            return;
        }

        // Start 전에 설정 덮어씌우기
        dialogueUI.npcName = npcName;
        dialogueUI.portraitSprite = npcPortrait;
        dialogueUI.lines = infoLines;

        dialogueUI.Open();
    }
}

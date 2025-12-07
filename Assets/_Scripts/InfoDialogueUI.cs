using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoDialogueUI : MonoBehaviour
{
    [Header("UI 레퍼런스")]
    public GameObject rootPanel;          // 전체 패널 (켜고 끌 대상)
    public TextMeshProUGUI nameText;      // NPC 이름
    public TextMeshProUGUI dialogueText;  // 대사 텍스트
    public Image portraitImage;           // NPC 이미지(옵션)

    public Button nextButton;             // 다음 버튼
    public Button closeButton;            // 닫기 버튼

    [Header("대사 내용")]
    [TextArea(2, 4)]
    public string[] lines;                // 한 줄씩 대사 입력

    [Header("NPC 이름 & 초상 (옵션)")]
    public string npcName = "정보상 NPC";
    public Sprite portraitSprite;

    private int currentIndex = 0;
    private bool isOpen = false;

    private void Start()
    {
        // 처음에는 패널 꺼두기
        if (rootPanel != null)
            rootPanel.SetActive(false);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnClickNext);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    // 바깥에서 호출해서 대화 열기
    public void Open()
    {
        if (isOpen) return;

        isOpen = true;
        currentIndex = 0;

        if (rootPanel != null)
            rootPanel.SetActive(true);

        if (nameText != null)
            nameText.text = npcName;

        if (portraitImage != null && portraitSprite != null)
            portraitImage.sprite = portraitSprite;

        ShowCurrentLine();
    }

    // 현재 인덱스의 대사 표시
    private void ShowCurrentLine()
    {
        if (dialogueText != null)
        {
            if (lines != null && lines.Length > 0 && currentIndex >= 0 && currentIndex < lines.Length)
                dialogueText.text = lines[currentIndex];
            else
                dialogueText.text = "";
        }

        // 마지막 줄이면 Next 숨기기
        if (nextButton != null)
        {
            bool hasNext = (lines != null && currentIndex < lines.Length - 1);
            nextButton.gameObject.SetActive(hasNext);
        }
    }

    private void OnClickNext()
    {
        if (lines == null || lines.Length == 0) return;

        if (currentIndex < lines.Length - 1)
        {
            currentIndex++;
            ShowCurrentLine();
        }
        else
        {
            // 마지막 줄에서 한 번 더 누르면 그냥 닫게 하고 싶으면:
            Close();
        }
    }

    public void Close()
    {
        isOpen = false;

        if (rootPanel != null)
            rootPanel.SetActive(false);
    }
}

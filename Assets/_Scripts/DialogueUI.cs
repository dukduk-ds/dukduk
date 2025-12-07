using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ShopChoice
{
    [Header("버튼에 표시될 글자")]
    public string label;              // 예: "빵"

    [Header("인벤토리에 추가할 SupplyType")]
    public SupplyType supplyType;     // 예: SupplyType.Bread

    [Header("추가할 개수")]
    public int amount = 1;            // 기본 1개
}

public class DialogueUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    [Header("버튼")]
    public GameObject nextButton;     // ▶ 버튼 (GameObject)
    public GameObject closeButton;    // X 버튼 (GameObject)

    [Header("대화 데이터")]
    public string npcName = "상점 NPC";
    public Sprite portraitSprite;

    [TextArea(2, 5)]
    public string[] lines;            // 0번: "무엇을 사러 왔는가" 같은 첫 대사

    [Header("선택지 UI")]
    public GameObject choicesRoot;            // 선택지 전체 Panel (ChoicesRoot)
    public Button[] choiceButtons;            // Choice, Choice (1) ...
    public TextMeshProUGUI[] choiceTexts;     // 각 Choice 안의 TMP Text
    public ShopChoice[] shopChoices;          // 데이터 (빵, 약, 총, 조명탄 등)

    [Header("선택 후 NPC 한마디")]
    [TextArea(2, 3)]
    public string afterChoiceLine = "여깄네.";

    [Header("컷신 동안 숨길 HUD 그룹들 (HP바, 미니맵 등)")]
    public GameObject[] hudGroups;

    private int currentIndex = 0;
    private bool isOpen = false;

    private enum State { Talking, Choosing, AfterChoice }
    private State state = State.Talking;

    // 어떤 선택지를 골랐는지
    private int selectedChoice = -1;

    // Start()는 사용 안 함. 처음에 꺼두고 싶으면 Hierarchy에서 패널 체크만 끄기

    // ==============================
    // 외부에서 호출 (BuildingShopTrigger)
    // ==============================
    public void Open()
    {
        if (isOpen) return;

        Debug.Log("[DialogueUI] Open() 호출");

        isOpen = true;
        state = State.Talking;
        selectedChoice = -1;
        currentIndex = 0;

        // 패널 켜기
        gameObject.SetActive(true);

        // HUD 숨기기
        if (hudGroups != null)
        {
            foreach (var hud in hudGroups)
            {
                if (hud != null) hud.SetActive(false);
            }
        }

        // 선택지 패널 / 버튼 초기화
        if (choicesRoot != null) choicesRoot.SetActive(false);

        if (nextButton != null)
        {
            nextButton.SetActive(true);
            var btn = nextButton.GetComponent<Button>();
            if (btn != null) btn.interactable = true;
        }

        if (closeButton != null)
            closeButton.SetActive(true);

        // 게임 멈추는 연출
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateLine();  // 첫 대사 출력
    }

    // ==============================
    // ▶ 버튼 (OnClick)
    // ==============================
    public void OnClickNext()
    {
        if (!isOpen) return;

        Debug.Log($"[DialogueUI] OnClickNext, state={state}, currentIndex={currentIndex}");

        switch (state)
        {
            case State.Talking:
                // 0번 대사 후 선택지 열기
                if (currentIndex == 0)
                {
                    state = State.Choosing;
                    ShowChoices();
                    return;
                }

                // 그 외 대사가 더 있다면 그냥 넘기기
                currentIndex++;
                if (currentIndex >= lines.Length)
                {
                    CloseDialogue();
                }
                else
                {
                    UpdateLine();
                }
                break;

            case State.Choosing:
                // 아직 선택 안 했으면 무시
                if (selectedChoice < 0)
                {
                    Debug.Log("[DialogueUI] 아직 선택 안 함");
                    return;
                }

                // 아이템 지급 + 한 마디
                GiveSelectedItem();

                state = State.AfterChoice;

                if (choicesRoot != null)
                    choicesRoot.SetActive(false);

                // 이후에는 ▶ 버튼 숨기고 X만 보이게
                if (nextButton != null)
                {
                    var btn = nextButton.GetComponent<Button>();
                    if (btn != null) btn.interactable = false;
                    nextButton.SetActive(false);
                }

                if (dialogueText != null)
                    dialogueText.text = afterChoiceLine;
                break;

            case State.AfterChoice:
                CloseDialogue();
                break;
        }
    }

    // ==============================
    // X 버튼 (OnClick)
    // ==============================
    public void OnClickClose()
    {
        CloseDialogue();
    }

    // ==============================
    // 선택지 표시
    // ==============================
    private void ShowChoices()
    {
        Debug.Log("[Shop] ShowChoices 호출");

        if (choicesRoot == null)
        {
            Debug.LogWarning("[Shop] choicesRoot가 비어 있음!");
            return;
        }

        choicesRoot.SetActive(true);
        Debug.Log("[Shop] choicesRoot 활성화됨");

        // 🔥 선택지 뜰 때 기존 질문 대사 지우기
        if (dialogueText != null)
            dialogueText.text = "";

        selectedChoice = -1;

        // Next 버튼은 보이지만, 선택 전까지는 비활성화
        if (nextButton != null)
        {
            nextButton.SetActive(true);
            var btn = nextButton.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
            Debug.Log("[Shop] Next 버튼 비활성화 (선택 전)");
        }

        if (choiceButtons == null || choiceButtons.Length == 0)
        {
            Debug.LogWarning("[Shop] choiceButtons 배열이 비어 있음!");
            return;
        }

        int count = (shopChoices != null) ? shopChoices.Length : 0;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            var btn = choiceButtons[i];
            if (btn == null) continue;

            // 버튼은 무조건 켜두기
            btn.gameObject.SetActive(true);

            string labelText = $"선택 {i + 1}";

            if (i < count && shopChoices[i] != null)
            {
                labelText = shopChoices[i].label;
            }

            if (choiceTexts != null && i < choiceTexts.Length && choiceTexts[i] != null)
            {
                choiceTexts[i].text = labelText;
            }

            int index = i;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnClickChoice(index));

            Debug.Log($"[Shop] choice 버튼 세팅 완료 index={i}, label={labelText}");
        }
    }

    // 선택지 버튼 클릭
    private void OnClickChoice(int index)
    {
        selectedChoice = index;

        Debug.Log($"[Shop] 선택지 클릭 index = {index}");

        if (nextButton != null)
        {
            var btn = nextButton.GetComponent<Button>();
            if (btn != null) btn.interactable = true;
        }
    }

    // 실제 인벤토리에 아이템 지급
    private void GiveSelectedItem()
    {
        Debug.Log($"[Shop] GiveSelectedItem 호출, 선택 index = {selectedChoice}");

        if (shopChoices == null ||
            selectedChoice < 0 ||
            selectedChoice >= shopChoices.Length)
        {
            Debug.LogWarning("[Shop] shopChoices 데이터가 없거나 index 범위 밖");
            return;
        }

        ShopChoice choice = shopChoices[selectedChoice];

        Debug.Log($"[Shop] 상점 선택 아이템 지급: {choice.supplyType} x{choice.amount}");

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(choice.supplyType, choice.amount);
        }
        else
        {
            Debug.LogWarning("[Shop] InventoryManager.Instance 가 없음!");
        }
    }

    // 대화창 닫기
    private void CloseDialogue()
    {
        if (!isOpen) return;

        Debug.Log("[DialogueUI] CloseDialogue");

        isOpen = false;
        state = State.Talking;
        selectedChoice = -1;

        if (choicesRoot != null)
            choicesRoot.SetActive(false);

        if (nextButton != null)
        {
            nextButton.SetActive(true);
            var btn = nextButton.GetComponent<Button>();
            if (btn != null) btn.interactable = true;
        }

        gameObject.SetActive(false);

        if (hudGroups != null)
        {
            foreach (var hud in hudGroups)
            {
                if (hud != null) hud.SetActive(true);
            }
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 기본 대사 한 줄 갱신
    private void UpdateLine()
    {
        if (portraitImage != null && portraitSprite != null)
            portraitImage.sprite = portraitSprite;

        if (nameText != null)
            nameText.text = npcName;

        if (dialogueText != null && lines != null && lines.Length > 0)
            dialogueText.text = lines[currentIndex];
    }
}

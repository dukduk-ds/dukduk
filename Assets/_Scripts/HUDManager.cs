using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    // === 1. HUD UI 요소 연결 (Image Fill Bar) ===

    [Header("Stat Images (Fill Bars)")]
    // 굶주림 게이지를 채우는 Image 컴포넌트 
    public Image hungerFillImage;
    // 정신력 게이지를 채우는 Image 컴포넌트 
    public Image sanityFillImage;

    [Header("Warning Colors")]
    [Tooltip("게이지가 30% 이하일 때 변경될 색상")]
    public Color lowStatColor = Color.red;
    [Tooltip("게이지가 정상 범위일 때의 색상")]
    public Color normalStatColor = Color.green;

    // NOTE: inventoryPanel 변수와 inventoryPanel 관련 로직은 모두 제거되었습니다.

    [Header("Money Panel")]
    [Tooltip("돈 정보만 보여주는 패널을 연결하세요.")]
    public GameObject moneyInfoPanel;

    // GameManager 인스턴스
    private GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;

        if (gm == null)
        {
            Debug.LogError("HUDManager: GameManager 인스턴스를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        // 초기 색상 설정
        if (hungerFillImage != null) hungerFillImage.color = normalStatColor;
        if (sanityFillImage != null) sanityFillImage.color = normalStatColor;

        // 돈 정보 패널도 시작 시 꺼둡니다.
        if (moneyInfoPanel != null)
        {
            moneyInfoPanel.SetActive(false);
        }
    }

    // === 2. 실시간 데이터 업데이트 로직 (Update) ===
    void Update()
    {
        if (gm == null) return;

        // --- 굶주림 업데이트 ---
        UpdateFillBar(gm.hunger, gm.maxStatValue, hungerFillImage, gm.lowHungerThreshold);

        // --- 정신력 업데이트 ---
        UpdateFillBar(gm.sanity, gm.maxStatValue, sanityFillImage, gm.lowHungerThreshold);
    }

    /// <summary>
    /// 생존 통계(Stat)를 Image Fill과 경고 색상에 따라 업데이트하는 함수
    /// </summary>
    private void UpdateFillBar(float currentValue, float maxValue, Image fillImage, float threshold)
    {
        if (fillImage != null)
        {
            // Fill Amount 업데이트 (0~1 사이의 비율 사용)
            fillImage.fillAmount = currentValue / maxValue;

            // 경고 색상 로직: 위험 수위(30% 이하)일 때 색상 변경
            if (currentValue <= threshold)
            {
                fillImage.color = lowStatColor;
            }
            else
            {
                fillImage.color = normalStatColor;
            }
        }
    }

    // === 3. 돈 주머니 버튼 클릭 이벤트 함수 (Btn_Money에 연결) ===

    /// <summary>
    /// 돈 주머니 버튼을 클릭했을 때 호출되는 함수
    /// </summary>
    public void OnMoneyButtonClicked()
    {
        if (moneyInfoPanel != null)
        {
            // 돈 정보 패널 토글
            bool isActive = moneyInfoPanel.activeSelf;
            moneyInfoPanel.SetActive(!isActive);

            if (!isActive)
            {
                // 패널이 켜질 때 현재 돈 수치를 업데이트합니다.
                TextMeshProUGUI moneyText = moneyInfoPanel.GetComponentInChildren<TextMeshProUGUI>();
                if (moneyText != null && GameManager.Instance != null)
                {
                    moneyText.text = $"현재 자금: {GameManager.Instance.money.ToString("N0")} WON";
                }
            }

            Debug.Log("돈 주머니 버튼 클릭됨. 정보 패널 토글: " + !isActive);
        }
        else
        {
            Debug.LogError("HUDManager: Money Info Panel이 연결되지 않았습니다.");
        }
    }
}
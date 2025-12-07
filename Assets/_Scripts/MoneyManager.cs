using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("초기 돈 설정")]
    public int startMoney = 5000;     // 게임 시작시 돈
    private int currentMoney = 0;

    [Header("UI 참조")]
    public GameObject moneyPanel;         // Btn_Money 전체 오브젝트
    public TextMeshProUGUI moneyText;     // Btn_Money 안에 있는 Text(TMP)

    private bool isVisible = false;

    private void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentMoney = startMoney;

        // 시작할 때 돈 패널은 안 보이게
        if (moneyPanel != null)
            moneyPanel.SetActive(false);

        UpdateMoneyUI();
    }

    private void Update()
    {
        // Y 키를 눌렀을 때 돈 패널 토글
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ToggleMoneyPanel();
        }
    }

    public int GetMoney()
    {
        return currentMoney;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        if (currentMoney < 0) currentMoney = 0;
        UpdateMoneyUI();
    }

    /// <summary>
    /// 돈을 쓸 때 사용. 충분하면 true, 부족하면 false 리턴.
    /// </summary>
    public bool TrySpendMoney(int amount)
    {
        if (currentMoney < amount)
            return false;

        currentMoney -= amount;
        UpdateMoneyUI();
        return true;
    }

    private void ToggleMoneyPanel()
    {
        if (moneyPanel == null) return;

        isVisible = !isVisible;
        moneyPanel.SetActive(isVisible);
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            // "5,000" 이런 식으로 보이게 하고 싶으면 "N0"
            moneyText.text = currentMoney.ToString("N0");
        }
    }
}

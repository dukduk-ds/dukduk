using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("최대값")]
    public float maxHP = 100f;
    public float maxHunger = 100f;
    public float maxMental = 100f;   // = 신뢰도 게이지

    [Header("현재값 (Inspector 확인용)")]
    public float hp;
    public float hunger;
    public float mental;             // 정신력 = 신뢰도

    [Header("돈")]
    public int money = 0;

    [Header("배고픔 감소 설정")]
    [Tooltip("몇 초마다 한 번씩 배고픔을 줄일지 (예: 10이면 10초마다)")]
    public float hungerTickInterval = 10f;   // ★ 여기 숫자 키우면 더 느려짐
    [Tooltip("한 번에 몇씩 줄일지 (예: 1이면 1포인트)")]
    public float hungerPerTick = 1f;         // 1이면 100 → 0 가는데 100번 필요
    private float hungerTimer = 0f;

    [Header("굶주렸을 때 체력 감소 설정")]
    public float hungerCriticalThreshold = 10f; // 이 값 이하면 '굶주림 상태'
    [Tooltip("배고픔 감소량의 몇 배로 체력을 깎을지 (0.5 = 절반 속도)")]
    public float hpDecreaseFactor = 0.5f;       // 배고픔보다 느리게 HP 감소

    void Awake()
    {
        // 시작 시 풀로 채우기
        hp = maxHP;
        hunger = maxHunger;
        mental = maxMental;
    }

    void Update()
    {
        // --- 1. 배고픔 타이머 ---
        hungerTimer += Time.deltaTime;

        if (hungerTimer >= hungerTickInterval)
        {
            hungerTimer -= hungerTickInterval; // 타이머 리셋

            // 배고픔 감소
            hunger = Mathf.Max(0f, hunger - hungerPerTick);

            // 굶주림 상태면 HP도 같이 감소 (배고픔 속도의 hpDecreaseFactor배)
            if (hunger <= hungerCriticalThreshold)
            {
                float hpDamage = hungerPerTick * hpDecreaseFactor;
                hp = Mathf.Max(0f, hp - hpDamage);
            }
        }

        // 🔸 정신력(mental)은 여기서 건드리지 않음!
        //     → 미션 스크립트에서만 AddMental()로 조절
    }

    // ----- 외부에서 쓰는 함수들 -----
    public void AddHP(float amount)
    {
        hp = Mathf.Clamp(hp + amount, 0f, maxHP);
    }

    public void AddHunger(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
    }

    public void AddMental(float amount)
    {
        mental = Mathf.Clamp(mental + amount, 0f, maxMental);
    }

    public bool TrySpendMoney(int amount)
    {
        if (money < amount) return false;
        money -= amount;
        return true;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }
}

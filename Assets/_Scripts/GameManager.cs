using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수!

public class GameManager : MonoBehaviour
{
    // 싱글톤(Singleton) 패턴: 게임 내내 딱 하나만 존재하도록 만듦
    public static GameManager Instance { get; private set; }

    // =========================================================
    // <<<<<<<<<<<< 생존 시스템 데이터 >>>>>>>>>>>>

    [Header("Survival Stats")]
    // 0~100 사이의 값으로 관리
    [Range(0f, 100f)] public float hunger = 100f;   // 굶주림
    [Range(0f, 100f)] public float sanity = 100f;   // 정신력
    public int money = 0;                           // 자금 (동전 등)

    [Header("Decay Settings")]
    [Tooltip("1초당 굶주림 감소량")]
    public float hungerDecayRate = 0.1f;
    [Tooltip("1초당 정신력 감소량")]
    public float sanityDecayRate = 0.05f;

    [Header("Stat Limits")]
    public float minStatValue = 0f;
    public float maxStatValue = 100f;
    // 굶주림 페널티 기준 값
    public float lowHungerThreshold = 30f;

    [Header("Status Effects")]
    [Tooltip("플레이어가 현재 감기에 걸렸는지 여부 (QTE 발동 조건)")]
    public bool isSick = false;

    // =========================================================


    void Awake()
    {
        // --- 싱글톤 설정 ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 다른 씬으로 넘어가도 이 GameManager는 파괴되지 않음!
        }
        else
        {
            Destroy(gameObject); // 이미 GameManager가 있다면 이 오브젝트는 파괴
        }
        // ---------------------
    }


    // =========================================================
    // <<<<<<<<<<<< 자동 감소 및 페널티 로직 >>>>>>>>>>>>

    void Update()
    {
        // 굶주림 감소 (시간에 따라 자동 감소)
        hunger -= hungerDecayRate * Time.deltaTime;

        // 정신력 감소
        sanity -= sanityDecayRate * Time.deltaTime;

        // Stat 값 제한 (0 ~ 100 사이로 유지)
        hunger = Mathf.Clamp(hunger, minStatValue, maxStatValue);
        sanity = Mathf.Clamp(sanity, minStatValue, maxStatValue);

        // 굶주림 페널티 체크 함수 호출
        CheckLowHungerPenalty();
    }

    // 굶주림 페널티 체크 함수 (TODO: PlayerController와 연동 필요)
    private void CheckLowHungerPenalty()
    {
        if (hunger <= lowHungerThreshold)
        {
            // Debug.Log("경고: 굶주림이 낮아 이동 속도 페널티가 적용되어야 함!");
            // (TODO: PlayerController.cs에서 이동 속도를 제어하는 함수를 호출합니다.)
        }
        else
        {
            // (TODO: 굶주림 페널티를 해제하는 함수를 호출합니다.)
        }
    }

    // =========================================================


    // --- 씬 전환 기능 ---
    // 씬의 '빌드 인덱스(숫자)'를 받아서 해당 씬을 불러오는 함수
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // (예시) 씬 로드 함수들
    public void LoadHub()
    {
        LoadScene(0);
    }

    public void LoadStage1()
    {
        LoadScene(1);
    }

    public void LoadStage2()
    {
        LoadScene(2);
    }

    public void LoadStage3()
    {
        LoadScene(3);
    }


    // --- 세이브/로드 기초 ---

    public void SaveGame()
    {
        Debug.Log("게임 저장 시도 (아직 구현 안 됨)");
    }

    public void LoadGame()
    {
        Debug.Log("게임 불러오기 시도 (아직 구현 안 됨)");
    }
}
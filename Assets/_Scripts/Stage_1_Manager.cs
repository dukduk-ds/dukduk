using UnityEngine;

public class Stage_1_Manager : MonoBehaviour
{
    // 👈 Singleton 패턴을 적용하여 씬 내에서 어디서든 접근 가능하게 합니다.
    public static Stage_1_Manager Instance { get; private set; }

    // --- 스테이지 고유 퍼즐 변수 ---
    // GameManager에서 가져온 정적 변수들을 여기에 정의합니다.
    public bool hasCodePiece1 = false;
    public bool hasCodePiece2 = false;

    public bool AllPiecesCollected
    {
        get { return hasCodePiece1 && hasCodePiece2; }
    }
    // ---------------------------------

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad는 필요 없음 (씬이 끝나면 같이 파괴)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // (선택 사항: 나중에 AI 감지 레벨, 씬 초기화 등을 여기서 관리)
}
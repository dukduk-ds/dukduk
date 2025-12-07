using UnityEngine;

public class ObjectiveZone : MonoBehaviour
{
    [Header("미션 성공 UI")]
    [Tooltip("미션 성공 시 활성화할 UI 패널을 연결하세요.")]
    public GameObject successUIPanel;

    private bool isMissionComplete = false;

    // 플레이어가 Trigger 영역에 들어왔을 때 호출됩니다.
    void OnTriggerEnter(Collider other)
    {
        // 미션이 이미 완료된 상태라면 중복 처리 방지
        if (isMissionComplete) return;

        // 플레이어 태그 확인
        if (other.CompareTag("Player"))
        {
            CompleteMission();
        }
    }

    void CompleteMission()
    {
        isMissionComplete = true; // 중복 호출 방지

        Debug.Log("🎉 최종 미션 성공! Good Ending!");

        // GameManager를 통해 미션 성공 처리 및 시간 정지 명령
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MissionSuccess(successUIPanel);
        }
        else
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다.");
        }
    }
}
using UnityEngine;

public class MissionSuccess : MonoBehaviour
{
    // Inspector에서 성공 UI 패널을 드래그하여 연결합니다.
    public GameObject successUIPanel;

    // Unity에서 제공하는 표준 함수: 다른 Collider가 Trigger 영역에 들어왔을 때 호출됨
    void OnTriggerEnter(Collider other)
    {
        // 태그를 확인하여 플레이어인지 체크 (플레이어 오브젝트에 "Player" 태그가 있어야 함)
        if (other.CompareTag("Player"))
        {
            TriggerGoodEnding();
        }
    }

    void TriggerGoodEnding()
    {
        Debug.Log("🎉 임무 성공! (굿 엔딩)");

        // 1. 성공 UI 활성화
        if (successUIPanel != null)
        {
            successUIPanel.SetActive(true);
        }

        // 2. 게임 시간 멈춤
        Time.timeScale = 0f;

        // 3. AI 및 플레이어 이동 스크립트 비활성화 (선택 사항)
        // 예를 들어, 플레이어의 PlayerController 스크립트를 찾아서 비활성화할 수 있습니다.
    }
}
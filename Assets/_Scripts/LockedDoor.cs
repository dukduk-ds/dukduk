using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [Header("해제 후 비활성화할 문")]
    [Tooltip("실제로 잠겨있는 문 3D 모델 오브젝트를 연결하세요.")]
    public GameObject doorObject;

    private bool isUnlocked = false;

    // 플레이어가 잠금 장치 영역에 들어오면 체크
    void OnTriggerEnter(Collider other)
    {
        // 이미 해제되었거나 플레이어가 아니면 무시
        if (other.CompareTag("Player") && !isUnlocked)
        {
            CheckForUnlock();
        }
    }

    void CheckForUnlock()
    {
        if (Stage_1_Manager.Instance == null)
        {
            Debug.LogError("Stage_1_Manager가 씬에 없습니다! 잠금 해제 체크 실패.");
            return;
        }

        // Stage Manager의 AllPiecesCollected 프로퍼티를 확인
        if (Stage_1_Manager.Instance.AllPiecesCollected)
        {
            UnlockDoor();
        }
        else
        {
            Debug.Log("문이 잠겨 있습니다. 코드 조각을 마저 모아야 합니다.");
            // (선택 사항) 여기에 잠금 해제 실패 UI/사운드 피드백 추가
        }
    }

    void UnlockDoor()
    {
        isUnlocked = true;
        Debug.Log("✅ 잠금 해제 성공! 문이 열립니다.");

        // 문 오브젝트 비활성화 (문을 '열린' 상태로 만들어 플레이어가 통과할 수 있게 됨)
        if (doorObject != null)
        {
            doorObject.SetActive(false);
        }

        // (선택 사항) 여기에 문 열리는 사운드 재생 코드를 추가합니다.
    }
}
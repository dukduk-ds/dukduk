using UnityEngine;

public class CodePiece : MonoBehaviour
{
    [Header("조각 번호 (1 또는 2)")]
    [Tooltip("이 조각이 Stage Manager의 어떤 변수를 True로 바꿀지 지정합니다.")]
    public int pieceNumber = 1;

    // Unity에서 제공하는 표준 함수: 다른 Collider가 Trigger 영역에 들어왔을 때 호출됨
    void OnTriggerEnter(Collider other)
    {
        // 태그를 확인하여 플레이어인지 체크 (플레이어 오브젝트에 "Player" 태그가 있어야 함)
        if (other.CompareTag("Player"))
        {
            CollectPiece();
        }
    }

    void CollectPiece()
    {
        // Stage_1_Manager 인스턴스가 있는지 확인
        if (Stage_1_Manager.Instance == null)
        {
            Debug.LogError("Stage_1_Manager가 씬에 없습니다! 퍼즐 로직 실패.");
            return;
        }

        // 1. 상태 업데이트 (Stage Manager의 변수를 직접 조작)
        if (pieceNumber == 1)
        {
            Stage_1_Manager.Instance.hasCodePiece1 = true;
            Debug.Log("코드 조각 1 획득!");
        }
        else if (pieceNumber == 2)
        {
            Stage_1_Manager.Instance.hasCodePiece2 = true;
            Debug.Log("코드 조각 2 획득!");
        }
        else
        {
            Debug.LogError("잘못된 조각 번호가 설정되었습니다. 1 또는 2로 설정해야 합니다.");
            return;
        }

        // (선택 사항) 여기에 획득 효과음 재생 코드를 추가할 수 있습니다.

        // 2. 오브젝트 비활성화 (맵에서 사라지게 함)
        gameObject.SetActive(false);
    }
}
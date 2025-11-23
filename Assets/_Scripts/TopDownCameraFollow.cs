using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Follow")]
    public float followLerp = 15f;   // 위치 부드럽게 따라가기

    [Header("Flick Rotation")]
    [Tooltip("이 값보다 강하게 움직여야 회전으로 인식됩니다.")]
    public float flickThreshold = 0.4f;  // 0.3~0.6 정도 추천

    [Tooltip("한 번 Flick할 때 최대 회전량(도 단위)")]
    public float maxYawStep = 3f;        // 2~5 사이 추천

    private Vector3 baseOffset;
    private float currentYaw = 0f;       // 누적 회전각 (제한 없음, 360도 이상도 OK)

    void Start()
    {
        if (!target) return;

        // 시작 시점의 player-카메라 간 거리/높이 저장
        baseOffset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (!target) return;

        // 1) 마우스 X 입력
        float mouseX = Input.GetAxis("Mouse X");
        float absX = Mathf.Abs(mouseX);

        // 2) Flick 판단: threshold보다 큰 입력만 회전으로 처리
        if (absX > flickThreshold)
        {
            // threshold를 얼마나 넘었는지 비율(0~1) 계산
            float strength01 = Mathf.Clamp01((absX - flickThreshold) / (1f - flickThreshold));

            // 한 번 flick당 회전량 계산 (왼쪽/오른쪽 방향 포함)
            float deltaYaw = Mathf.Sign(mouseX) * strength01 * maxYawStep;

            // 🔥 회전각 누적 (여기서는 범위 제한 없음 → 360도, 720도 다 가능)
            currentYaw += deltaYaw;
        }

        // 3) 현재 yaw 각도로 offset 회전
        Quaternion rot = Quaternion.Euler(0f, currentYaw, 0f);
        Vector3 rotatedOffset = rot * baseOffset;

        // 4) 카메라 위치를 플레이어 주변으로 이동
        Vector3 desiredPos = target.position + rotatedOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followLerp * Time.deltaTime);

        // 5) 카메라가 항상 플레이어를 바라보도록
        Vector3 lookTarget = target.position + Vector3.up * 2f;
        transform.rotation = Quaternion.LookRotation(lookTarget - transform.position);
    }
}

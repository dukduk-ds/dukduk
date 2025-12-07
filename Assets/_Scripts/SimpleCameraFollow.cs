using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("따라갈 대상 (플레이어)")]
    public Transform target;

    [Header("부드럽게 따라오기 정도 (0이면 바로 붙음)")]
    public float followLerp = 10f;

    private Vector3 offset;           // 카메라와 플레이어 사이 거리
    private Quaternion initialRot;    // 시작할 때의 카메라 각도

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("SimpleCameraFollow: target이 비어있어요!");
            enabled = false;
            return;
        }

        // ★ 지금 씬에서 보이는 '현재 위치/각도' 그대로 저장
        offset = transform.position - target.position;
        initialRot = transform.rotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 원하는 위치 = 플레이어 위치 + 처음 저장한 offset
        Vector3 desiredPos = target.position + offset;

        // 부드럽게 이동 (followLerp가 0이면 바로 이동)
        if (followLerp > 0f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPos,
                followLerp * Time.deltaTime
            );
        }
        else
        {
            transform.position = desiredPos;
        }

        // 각도는 처음 봤던 각도 그대로 유지
        transform.rotation = initialRot;
    }
}

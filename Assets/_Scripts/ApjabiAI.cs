using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ApjabiAI : MonoBehaviour
{
    [Header("플레이어 참조")]
    public Transform player;

    [Header("이동/추격 설정")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("시야 설정")]
    public float detectionRange = 8f;
    public float detectionAngle = 70f;

    [Header("순찰 경로")]
    public Transform[] waypoints;

    private CharacterController controller;
    private int currentWaypoint = 0;

    // AI 상태머신
    private enum State { Patrol, Chase }
    private State state = State.Patrol;

    // 🔥 FuseBox에서 조절하기 위한 기본값 저장
    private float baseDetectionRange;
    private float baseDetectionAngle;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // 🔥 기본값 저장 (FuseBox가 조절할 수 있게)
        baseDetectionRange = detectionRange;
        baseDetectionAngle = detectionAngle;
    }

    void Update()
    {
        if (player == null)
            return;

        // 1. 플레이어 보이나?
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            state = State.Chase;
        }
        else if (state == State.Chase && !canSeePlayer)
        {
            state = State.Patrol;
        }

        // 2. 상태에 따라 행동
        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float dist = toPlayer.magnitude;
        if (dist > detectionRange)
            return false;

        // 각도 체크
        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
        if (angle > detectionAngle * 0.5f)
            return false;

        return true;    // Raycast 체크는 생략 (네 프로젝트가 간단 구조여서)
    }

    // --- 순찰 행동 ---
    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Transform target = waypoints[currentWaypoint];
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.magnitude < 0.2f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            return;
        }

        MoveAndRotate(dir.normalized, patrolSpeed);
    }

    // --- 추격 행동 ---
    void Chase()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.1f)
            return;

        MoveAndRotate(dir.normalized, chaseSpeed);
    }

    // 이동 + 회전
    void MoveAndRotate(Vector3 dir, float speed)
    {
        controller.SimpleMove(dir * speed);

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 8f
            );
        }
    }

    // --- Gizmo: 에디터에서 시야 표시 ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 leftDir = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, detectionAngle * 0.5f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftDir * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * detectionRange);
    }

    // --- 🔥 FuseBox가 시야 조절할 때 호출 ---
    public void SetVisionMultiplier(float multiplier)
    {
        detectionRange = baseDetectionRange * multiplier;
        detectionAngle = baseDetectionAngle * multiplier;
    }
}

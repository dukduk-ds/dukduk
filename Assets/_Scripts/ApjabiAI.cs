using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ApjabiAI : MonoBehaviour
{
    public Transform player;          // 플레이어 Transform
    public float patrolSpeed = 2f;    // 순찰 속도
    public float chaseSpeed = 4f;     // 추격 속도
    public float detectionRange = 8f; // 플레이어 감지 거리
    public float detectionAngle = 70f; // 시야각 (도 단위)
    public Transform[] waypoints;     // 순찰 지점들

    private CharacterController controller;
    private int currentWaypoint = 0;
    private enum State { Patrol, Chase }
    private State state = State.Patrol;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (player == null)
            return;

        // 1. 플레이어를 감지하는지 먼저 체크
        bool canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            state = State.Chase;
        }
        else if (state == State.Chase && !canSeePlayer)
        {
            // 한동안 못 보면 다시 순찰로
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

        // (선택) Raycast로 시야 막힌 것 체크하고 싶으면 여기서 추가 가능
        return true;
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Transform target = waypoints[currentWaypoint];
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.magnitude < 0.2f)
        {
            // 다음 웨이포인트로
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            return;
        }

        MoveAndRotate(dir.normalized, patrolSpeed);
    }

    void Chase()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.1f)
            return;

        MoveAndRotate(dir.normalized, chaseSpeed);
    }

    void MoveAndRotate(Vector3 dir, float speed)
    {
        // 이동
        controller.SimpleMove(dir * speed);

        // 몸 회전
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

    // Scene 뷰에서 시야각 / 거리 보이게 Gizmo 그리기 (디버그용)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 시야각 선
        Vector3 leftDir = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, detectionAngle * 0.5f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftDir * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * detectionRange);
    }
}

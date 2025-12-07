using UnityEngine;

// 이 스크립트를 사용하려면 Sunsa 오브젝트에 CharacterController 컴포넌트가 필요합니다.
[RequireComponent(typeof(CharacterController))]
public class SunsaMovement : MonoBehaviour
{
    [Header("AI 사운드")]
    public AudioClip detectionSound; // 인스펙터에서 경고음 WAV/MP3 파일 연결
    private AudioSource audioSource;

    [Header("타겟 설정")]
    public Transform player; // 플레이어(덕새) 오브젝트
    public Transform[] waypoints; // 순찰 지점 (Inspector에서 연결)

    [Header("AI 설정")]
    public float patrolSpeed = 2f; // 순찰 속도
    public float detectionRange = 12f; // 시야 거리 (Inspector에서 8f 등으로 조정 권장)
    public float detectionAngle = 70f; // 시야각 (Inspector에서 35f 등으로 조정 권장)

    [Header("UI 연결")]
    public GameObject gameOverUIPanel; // 발각 시 활성화할 게임 오버 UI 패널

    private CharacterController controller;
    private int currentWaypoint = 0;

    // AI의 현재 상태 정의
    private enum State { Patrol, Alerted }
    private State state = State.Patrol;

    // 감지 시간 관련 변수
    private float detectionTimer = 0f;
    public float timeToCapture = 0.5f; // 발각 후 게임 오버까지 걸리는 시간 (0.1f로 설정 권장)

    // Awake는 Start보다 먼저 호출되어 시간 복구를 보장합니다.
    void Awake()
    {
        // 🚨🚨🚨 시간 복구 핵심 🚨🚨🚨
        // 게임 시작 시, 멈춰있는 TimeScale을 1로 강제 복구하여 순찰 및 이동을 활성화합니다.
        if (Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
            Debug.Log("게임 시간 복구 완료!");
        }

        // CharacterController 컴포넌트 가져오기
        controller = GetComponent<CharacterController>();

        // CharacterController가 없는 경우 에러 발생
        if (controller == null)
        {
            Debug.LogError("SunsaMovement 스크립트는 CharacterController 컴포넌트가 필요합니다.");
            this.enabled = false;
        }
    }
    void Start()
    {
        // 👈 추가: AudioSource 컴포넌트 가져오기 또는 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // AI 오브젝트에 AudioSource가 없으면 자동으로 추가
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // 시작 시 자동 재생 방지
    }


    void Update()
    {
        if (player == null || waypoints.Length == 0)
        {
            state = State.Patrol;
            return;
        }

        // 1. 플레이어 감지 로직
        bool playerInSight = CanSeePlayer();

        if (playerInSight)
        {
            detectionTimer += Time.deltaTime;
            state = State.Alerted;

            // 2. 김마리아 모드: 즉시 체포 (발각 시간 초과 시)
            if (detectionTimer >= timeToCapture)
            {
                TriggerBadEnding(); // Bad Ending 함수 호출
                return; // 게임 오버 후 Update 중지
            }
        }
        else
        {
            // 시야에서 벗어나면 타이머 초기화 및 순찰 복귀
            detectionTimer = 0f;
            state = State.Patrol;
        }

        // 3. 상태에 따른 행동 실행
        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Alerted:
                AlertedAction();
                break;
        }
    }

    // ========== 감지 체크 로직 ==========

    bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        // 1. 거리 체크 
        if (distance > detectionRange)
            return false;

        // 2. 각도 체크
        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
        if (angle > detectionAngle * 0.5f)
            return false;

        return true;
    }

    // ========== 순찰 및 이동 로직 ==========

    void Patrol()
    {
        Transform target = waypoints[currentWaypoint];
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        // 목표 지점에 거의 도달했을 때
        if (dir.magnitude < 0.2f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            return;
        }

        MoveAndRotate(dir.normalized, patrolSpeed);
    }

    void AlertedAction()
    {
        // 정지하고 플레이어를 바라보게 합니다.
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        MoveAndRotate(Vector3.zero, 0f); // 이동은 멈춤
        RotateTowards(dir); // 플레이어를 바라보게만 함
    }

    // 캐릭터의 실제 이동 처리 (CharacterController 사용)
    void MoveAndRotate(Vector3 dir, float speed)
    {
        controller.SimpleMove(dir * speed);

        if (dir.sqrMagnitude > 0.001f)
        {
            RotateTowards(dir);
        }
    }

    // 특정 방향으로 몸을 회전시키는 함수
    void RotateTowards(Vector3 dir)
    {
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * 8f // 회전 속도
        );
    }


    // ========== 게임 오버 처리 (Bad Ending 통합 함수) ==========

    void TriggerBadEnding()
    {
        if (this.enabled == false) return;

        // 1. 📢 경고음 재생 (GameOver 직전에 실행!)
        if (audioSource != null && detectionSound != null)
        {
            // AudioSource가 이미 씬에 있으므로, PlayOneShot으로 재생
            audioSource.PlayOneShot(detectionSound);
        }

        // 2. 게임 오버 처리
        if (GameManager.Instance != null)
        {
            // GameManager로 게임 오버 UI 활성화 및 시간 정지 명령
            GameManager.Instance.GameOver(gameOverUIPanel);
        }

        this.enabled = false;
    }


    // ========== 디버그 시각화 (Scene 뷰에서 시야각 확인) ==========

    void OnDrawGizmos()
    {
        // 헌병의 감지 범위 (빨간색 원)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 헌병의 시야각 (빨간색 원뿔 선)
        Vector3 forward = transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * forward;
        Vector3 rightDir = Quaternion.Euler(0, detectionAngle * 0.5f, 0) * forward;

        Gizmos.DrawLine(transform.position, transform.position + leftDir * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * detectionRange);
    }
}
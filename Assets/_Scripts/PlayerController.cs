using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // === 1. 이동 및 애니메이터 관련 변수 ===
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float stealthSpeed = 2.5f; // 숨기/수그리기 속도
    public float flySpeed = 8f;       // 날기 속도
    private float currentSpeed;
    private CharacterController controller;
    private Camera playerCamera;
    private Vector2 inputVector;

    // TODO: (나중에) 애니메이터 컴포넌트 연결
    // private Animator anim; 

    // === 2. 생존 및 정신력 관련 지표 (기획서 반영) ===
    // NOTE: 이 지표들은 이제 GameManager.cs가 관리하는 것을 권장합니다.
    [Header("Survival Stats (TEMP)")]
    public float health = 100f;
    public float trust = 50f;

    [Header("Status Effects")]
    public bool isStealthing = false;
    // public bool hasSickness = false; // GameManager.isSick을 사용하도록 변경

    // === 3. 질병 QTE 설정 (통합) ===
    [Header("Sickness QTE Settings")]
    public float coughTimeInterval = 10f; // 기침이 터지는 주기 (초)
    public float qteDuration = 1.0f;      // QTE 입력 성공 시간 (초)
    public GameObject qteIndicatorUI;     // QTE가 발동되었음을 알려줄 임시 UI (Inspector 연결 필요)

    private float nextCoughTime;
    private bool isQTEActive = false;

    // 캐릭터의 기본 높이 (수그리기 로직에 사용)
    private float originalControllerHeight;

    // --- 생존 및 정신력 관련 지표 (기획서 반영) ---
    [Header("Survival Stats")]
    [Tooltip("물리적 피해에 대한 지표")]
    public float health = 100f; // 체력 (하트 아이콘)
    [Tooltip("시간 경과에 따라 감소하며, 굶주림 디버프를 유발")]
    public float hunger = 100f; // 굶주림 (주먹밥 아이콘)
    [Tooltip("임무 성공 및 대의적 선택 시 상승")]
    public float trust = 50f;   // 신뢰도 (촛불 아이콘)
    [Tooltip("임무 실패, 굶주림 등으로 감소하며, 배드 엔딩 분기 유발")]
    public float sanity = 100f; // 정신력 (뇌/태극 문양)
    public float money = 0f;    // 소지 금액 (¥)

    // --- 상태 이상 (디버프) ---
    [Header("Status Effects")]
    public bool isStealthing = false;
    public bool hasSickness = false; // 질병 (감기)

    // 이전에 Awake에 있던 초기화 로직을 Start()로 옮겨 초기화 순서의 안정성을 높입니다.
    void Start()
    {
        // CharacterController 컴포넌트를 찾습니다.
        controller = GetComponent<CharacterController>();
        // Tag가 'MainCamera'인 카메라를 찾습니다.
        playerCamera = Camera.main;
        currentSpeed = walkSpeed;

        // QTE UI 초기 비활성화
        if (qteIndicatorUI != null)
        {
            qteIndicatorUI.SetActive(false);
        }

        // 첫 기침 시간 설정
        nextCoughTime = Time.time + coughTimeInterval;

        // CharacterController 높이 저장
        if (controller != null)
        {
            originalControllerHeight = controller.height;
        }

        // GameManager 인스턴스 체크 (QTE 발동 조건 연동을 위해)
        if (GameManager.Instance == null)
        {
            Debug.LogError("FATAL ERROR: GameManager 인스턴스를 찾을 수 없습니다! 생존/QTE 로직이 작동하지 않습니다.");
        }
    }

    void Update()
    {
        // 1. 이동 및 회전 로직
        HandleMovement();

        // 2. 생존 지표 감소 및 상태 관리
        HandleSurvivalDecay(); // GameManager가 관리하므로 이 함수는 연동 역할만 함

        // 3. 기침 소음 발생 로직 (QTE)
        HandleSicknessCough();

        // 4. Ctrl 키 QTE 입력 처리
        // QTE 발동 중이고 (isQTEActive) Ctrl 키가 눌렸을 때
        if (isQTEActive && Input.GetKeyDown(KeyCode.LeftControl))
        {
            HandleCoughSuppressQTE();
        }
    }

    // --- 이동 및 회전 로직 ---
    void HandleMovement()
    {
        if (controller == null || playerCamera == null) return;

        // 탑다운 시점의 이동 방향 계산
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * inputVector.y + right * inputVector.x).normalized;

        // CharacterController를 사용하여 이동
        // Note: CharacterController는 Gravity를 적용해야 자연스럽게 바닥에 붙어있음
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        // 회전 로직
        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // TODO: (나중에) 애니메이션 파라미터 업데이트 로직 추가
    }

    // --- Input System 이벤트 함수 ---

    public void OnMove(InputAction.CallbackContext context)
    {
        // W, A, S, D 입력 값 (Vector2)을 inputVector에 할당
        inputVector = context.ReadValue<Vector2>();

        // TODO: (나중에) 애니메이션 파라미터 설정 (moveDirection.magnitude > 0.1f 일 때 IsWalking = true)
    }

    public void OnStealth(InputAction.CallbackContext context) // Ctrl 키
    {
        if (controller == null) return;

        if (context.performed)
        {
            isStealthing = true;
            currentSpeed = stealthSpeed;
            // CharacterController의 Height를 낮춥니다.
            controller.height = originalControllerHeight * 0.5f;
            controller.center = new Vector3(0, originalControllerHeight * 0.25f, 0); // 센터도 조정
        }
        else if (context.canceled)
        {
            isStealthing = false;
            currentSpeed = walkSpeed;
            // CharacterController의 Height를 원래대로 복원합니다.
            controller.height = originalControllerHeight;
            controller.center = new Vector3(0, originalControllerHeight * 0.5f, 0); // 센터 복원
        }
    }

    public void OnFly(InputAction.CallbackContext context) // F 키
    {
        if (context.performed)
        {
            if (health > 10f)
            {
                currentSpeed = flySpeed;
                health -= 10f;
                // TODO: 날기 애니메이션 및 효과음 재생 로직 추가
            }
        }
        else if (context.canceled)
        {
            currentSpeed = isStealthing ? stealthSpeed : walkSpeed;
        }
    }

    // --- 생존 지표 및 상태 이상 처리 함수 (Placeholder) ---

    void HandleSurvivalDecay()
    {
        // GameManager에서 감소 로직이 처리되므로, 여기서는 페널티 적용 로직만 받아서 처리
        if (GameManager.Instance != null && GameManager.Instance.hunger <= GameManager.Instance.lowHungerThreshold)
        {
            // 굶주림 페널티 (이동 속도 저하) 적용
            currentSpeed = isStealthing ? stealthSpeed * 0.5f : walkSpeed * 0.5f;
        }
        else
        {
            // 페널티 해제 (원래 속도로 복귀)
            currentSpeed = isStealthing ? stealthSpeed : walkSpeed;
        }
    }

    // --- 질병(감기) QTE 로직 (통합) ---

    void HandleSicknessCough()
    {
        // 감기에 걸렸고 (isSick) QTE가 활성화되어 있지 않으며 (isQTEActive) 시간이 되었을 때 발동
        if (GameManager.Instance != null && GameManager.Instance.isSick && !isQTEActive && Time.time >= nextCoughTime)
        {
            StartCoughQTE();
        }
    }

    private void StartCoughQTE()
    {
        isQTEActive = true;

        // UI 표시 (플레이어에게 Ctrl 키를 누르라고 알림)
        if (qteIndicatorUI != null)
        {
            qteIndicatorUI.SetActive(true);
        }

        // qteDuration 시간 후 자동 실패 처리
        Invoke(nameof(QTEFailed), qteDuration);

        Debug.Log("QTE 발동! Ctrl 키를 눌러 기침을 참으세요.");
    }

    private void HandleCoughSuppressQTE()
    {
        if (!isQTEActive) return;

        // 자동 실패 타이머 취소
        CancelInvoke(nameof(QTEFailed));

        // QTE 성공!
        QTESuccess();
    }

    private void QTESuccess()
    {
        isQTEActive = false;
        if (qteIndicatorUI != null) qteIndicatorUI.SetActive(false);

        // 성공 보상: 다음 기침 시간을 늦춥니다.
        nextCoughTime = Time.time + coughTimeInterval;

        Debug.Log("QTE 성공: 기침 참기에 성공했습니다.");
    }

    private void QTEFailed()
    {
        isQTEActive = false;
        if (qteIndicatorUI != null) qteIndicatorUI.SetActive(false);

        // 실패 페널티: 정신력/굶주림 감소 등 페널티 추가
        // GameManager.Instance.sanity -= 5f; // 예시

        Debug.Log("QTE 실패: 기침 소리가 났습니다. 페널티 적용!");

        // 다음 기침 시간을 즉시 설정 (실패했으므로 더 빨리 재발 가능)
        nextCoughTime = Time.time + coughTimeInterval * 0.5f;
    }

    // 이외의 Input System 함수 (OnInteract, OnInventory, OnItem1/2/3 등)는 필요에 따라 추가됩니다.
}
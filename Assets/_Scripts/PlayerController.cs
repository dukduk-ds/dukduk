using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Public Variables (인스펙터에서 수정) ---
    [Header("플레이어 기본 설정")]
    public float moveSpeed = 5.0f;
    public float crouchSpeed = 2.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.0f;
    public float rotationSpeed = 500f; // 회전 속도

    [Header("숨기(Crouch) 설정")]
    public float originalHeight = 2.0f; // 캐릭터 컨트롤러의 원래 높이
    public float crouchHeight = 1.0f;  // 숨었을 때의 높이

    // --- Private Variables (내부 사용) ---
    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator; // 애니메이터 컴포넌트
    private Transform cam;     // 메인 카메라 Transform
    private bool isGrounded;
    private bool isCrouching = false;
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;

        // 👈 수정: 카메라 태그가 없을 경우 대비
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        else
        {
            Debug.LogError("씬에서 'MainCamera' 태그를 가진 카메라를 찾을 수 없습니다! 회전이 작동하지 않습니다.");
        }
        // 👈 메인 카메라의 Transform을 참조
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }

        // 👈 Animator 컴포넌트 초기화 (3D 모델 자식 오브젝트에 붙어있다고 가정)
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator 컴포넌트를 자식에서 찾을 수 없어 애니메이션이 작동하지 않습니다.");
        }
    }

    void Update()
    {
        if (controller == null) return;
        // 👈 추가: cam이 null일 경우 Update 로직을 바로 종료
        if (cam == null) return;

        // --- 1. 땅 감지 및 중력 리셋 ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // --- 2. 숨기 (Crouch) 로직 및 컨트롤러 높이 조정 ---
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            currentSpeed = crouchSpeed;
            // 컨트롤러 높이를 낮추고 센터 조정
            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchHeight / 2, 0);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            currentSpeed = moveSpeed;
            // 컨트롤러 높이를 복구하고 센터 조정
            controller.height = originalHeight;
            controller.center = new Vector3(0, originalHeight / 2, 0);
        }

        // --- 3. 이동 입력 및 회전 (Rotation & Horizontal Movement) ---
        float x = Input.GetAxis("Horizontal"); // A, D
        float z = Input.GetAxis("Vertical");   // W, S

        // 입력 벡터를 만듭니다. (Y축은 0)
        Vector3 inputDirection = new Vector3(x, 0f, z).normalized;
        float moveMagnitude = inputDirection.magnitude; // 이동량 (애니메이션 Speed에 사용)

        // 플레이어 회전 및 이동 처리
        if (moveMagnitude >= 0.1f)
        {
            // A. 회전 각도 계산: 카메라 Y축 각도 + 입력 방향 각도
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            // B. 부드러운 회전 적용
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // C. 이동 방향 설정: 회전된 방향으로 캐릭터를 이동시킵니다.
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
        }

        // --- 4. 점프/날기 ---
        if (Input.GetKeyDown(KeyCode.F) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- 5. 중력 적용 (Vertical Movement) ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ----------------------------------------------------
        // 6. 애니메이션 업데이트
        if (animator != null)
        {
            // Walk와 Idle 전환을 위해 Speed 파라미터에 이동량 전달
            // Speed 파라미터 값이 0이면 Idle, 0.1 이상이면 Walk로 전환됩니다.
            animator.SetFloat("Speed", moveMagnitude);
        }
        // ----------------------------------------------------
    }
}
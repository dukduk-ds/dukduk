using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Public Variables (인스펙터에서 조절 가능) ---
    [Header("플레이어 이동 설정")]
    public float moveSpeed = 5.0f;
    public float crouchSpeed = 2.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.0f;

    // --- Private Variables ---
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching = false;
    private float currentSpeed;

    // 🔧 자물쇠 해제용 (일단 true로 테스트 - 인벤토리 연결 가능)
    [HideInInspector]
    public bool hasLockPick = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        // --- 1. 바닥 체크 (CharacterController 내장) ---
        isGrounded = controller.isGrounded;

        // 착지 후 y속도 초기화
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // --- 2. 웅크리기 (Crouch) ---
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            currentSpeed = moveSpeed;
        }

        // --- 3. 좌우/앞뒤 이동 ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- 4. 점프 ---
        if (Input.GetKeyDown(KeyCode.F) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- 5. 중력 처리 ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // --- 🔥 추가 기능: 외부 스크립트에서 읽도록 Getter 제공 ---
    public bool IsCrouching
    {
        get { return isCrouching; }
    }

    public bool IsMoving
    {
        get
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            return Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
        }
    }
}

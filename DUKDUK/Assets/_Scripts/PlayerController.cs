using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- Public Variables (인스펙터에서 수정) ---
    [Header("플레이어 설정")]
    public float moveSpeed = 5.0f;
    public float crouchSpeed = 2.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.0f; // 이 값이 0이면 점프가 안 됩니다! 인스펙터에서 1 이상인지 확인하세요.

    // --- Private Variables (내부 사용) ---
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded; // 땅에 닿았는지 매 프레임 저장할 변수
    private bool isCrouching = false;
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        // --- 1. 땅 감지 (가장 먼저!) ---
        // Character Controller가 땅에 닿았는지 매 프레임 확인
        isGrounded = controller.isGrounded;

        // --- 2. 중력 리셋 ---
        // 땅에 있고, 떨어지는 중(velocity.y < 0)이 아닐 때
        if (isGrounded && velocity.y < 0)
        {
            // 속도를 0이 아닌 -2f 정도로 살짝 눌러줘서, 땅에서 뜨지 않게 합니다.
            velocity.y = -2f;
        }

        // --- 3. 숨기 (Crouch) ---
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            currentSpeed = crouchSpeed;
            // (나중에 여기에 '숨기' 애니메이션 실행 코드를 넣으면 됩니다)
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            currentSpeed = moveSpeed;
            // (나중에 여기에 '걷기' 애니메이션 실행 코드를 넣으면 됩니다)
        }

        // --- 4. 이동 (Horizontal Movement) ---
        float x = Input.GetAxis("Horizontal"); // A, D
        float z = Input.GetAxis("Vertical");   // W, S
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- 5. 점프/날기 (Jump/Fly) ---
        // F키를 누르고, (아까 저장해둔) isGrounded가 true일 때
        if (Input.GetKeyDown(KeyCode.F) && isGrounded)
        {
            // 중력을 이기고 점프! (물리 공식)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- 6. 중력 적용 (Vertical Movement) ---
        // 매 프레임 중력을 더하고, 그 값으로 플레이어를 수직 이동시킵니다.
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
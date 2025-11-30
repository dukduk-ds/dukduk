using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [Header("기본 설정")]
    public string playerTag = "Player";
    public KeyCode interactKey = KeyCode.E;

    [Header("문 & 애니메이션")]
    public Animator doorAnimator;   // "Open" 트리거가 있다고 가정
    public Collider doorCollider;   // 실제로 막고 있는 콜라이더

    [Header("사운드")]
    public AudioSource unlockSfx;
    public AudioSource failSfx;

    private bool playerInRange = false;
    private bool isUnlocked = false;
    private PlayerController playerController;

    private void Reset()
    {
        if (doorCollider == null)
            doorCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerController = other.GetComponent<PlayerController>();
            // TODO: "E키로 자물쇠 따기" UI
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            playerController = null;
            // TODO: UI 숨기기
        }
    }

    private void Update()
    {
        if (!playerInRange || isUnlocked) return;

        if (Input.GetKeyDown(interactKey))
        {
            TryUnlock();
        }
    }

    private void TryUnlock()
    {
        if (playerController == null) return;

        if (playerController.hasLockPick)   // 에어팟 플라스틱 있다고 가정
        {
            UnlockDoor();
        }
        else
        {
            if (failSfx != null) failSfx.Play();
            Debug.Log("[DoorLock] 잠금 해제 도구가 없습니다. (에어팟 플라스틱 필요)");
        }
    }

    private void UnlockDoor()
    {
        isUnlocked = true;

        if (unlockSfx != null) unlockSfx.Play();

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (doorCollider != null)
            doorCollider.enabled = false;

        Debug.Log("[DoorLock] 문 열림!");
        // TODO: UI 숨기기
    }
}

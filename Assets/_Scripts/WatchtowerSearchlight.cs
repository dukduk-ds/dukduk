using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WatchtowerSearchlight : MonoBehaviour
{
    [Header("회전 설정")]
    public Transform pivot;           // 회전축(없으면 자기 자신)
    public float rotationSpeed = 40f;

    [Header("탐지 설정")]
    public string playerTag = "Player";
    public float detectTime = 0.5f;

    [Header("사운드")]
    public AudioSource alarmSfx;

    private bool playerInside = false;
    private float stayTimer = 0f;
    private bool alarmTriggered = false;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        // 회전
        Transform t = pivot != null ? pivot : transform;
        t.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        if (!playerInside || alarmTriggered) return;

        stayTimer += Time.deltaTime;
        if (stayTimer >= detectTime)
        {
            TriggerAlarm();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            stayTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            stayTimer = 0f;
        }
    }

    private void TriggerAlarm()
    {
        if (alarmTriggered) return;
        alarmTriggered = true;

        if (alarmSfx != null)
            alarmSfx.Play();

        Debug.Log("[Watchtower] 플레이어 탐지! 경보 레벨 최고, 헌병 소환!");

        // TODO:
        // 여기서 씬 전체 적들에게 "경보 모드" 전환시키는 코드 넣으면 됨.
        // 예시)
        // foreach (var enemy in FindObjectsOfType<ApjabiAI>())
        // {
        //     enemy.ForceChase(playerTransform);  ← 이런 함수 만들 수도 있고
        // }
    }
}

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CreakyFloor : MonoBehaviour
{
    public string playerTag = "Player";

    [Header("소음 설정")]
    public float noiseRadius = 8f;
    public float noiseCooldown = 1.5f;

    [Header("사운드")]
    public AudioSource creakSfx;

    private float lastNoiseTime = -999f;
    private PlayerController playerOnFloor;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerOnFloor = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerOnFloor = null;
        }
    }

    private void Update()
    {
        if (playerOnFloor == null) return;

        // 웅크리지 않고 움직이는 중일 때만 소음
        if (!playerOnFloor.IsCrouching && playerOnFloor.IsMoving)
        {
            if (Time.time - lastNoiseTime >= noiseCooldown)
            {
                MakeNoise();
                lastNoiseTime = Time.time;
            }
        }
    }

    private void MakeNoise()
    {
        if (creakSfx != null)
            creakSfx.Play();

        Vector3 pos = transform.position;

        Debug.Log("[CreakyFloor] 삐걱! 소음 발생, 반경 " + noiseRadius);

        // TODO:
        // 1) 미니맵에 소음 원 표시
        // MiniMapManager.Instance.CreateNoisePing(pos, noiseRadius);

        // 2) 주변 ApjabiAI에게 "소리 들음" 알리기
        // EnemyNoiseSystem.Instance.EmitNoise(pos, noiseRadius);
    }
}

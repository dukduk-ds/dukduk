using System.Collections.Generic;
using UnityEngine;

public class FuseBox : MonoBehaviour
{
    [Header("상호작용 설정")]
    public string playerTag = "Player";
    public KeyCode interactKey = KeyCode.E;

    [Header("제어할 라이트들")]
    public List<Light> controlledLights = new List<Light>();

    [Header("제어할 적(ApjabiAI)들")]
    public List<ApjabiAI> controlledEnemies = new List<ApjabiAI>();

    [Header("밝을 때 / 어두울 때 시야 배수")]
    public float normalVisionMultiplier = 1.0f;
    public float darkVisionMultiplier = 0.3f;   // 70% 감소 느낌

    [Header("사운드")]
    public AudioSource switchSfx;

    private bool isPowerOn = true;
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            // TODO: "E키로 전등 스위치" 같은 UI 띄우기
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            // TODO: UI 숨기기
        }
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            TogglePower();
        }
    }

    private void TogglePower()
    {
        isPowerOn = !isPowerOn;

        // 라이트 켜고/끄기
        foreach (var light in controlledLights)
        {
            if (light == null) continue;
            light.enabled = isPowerOn;
        }

        // 적 시야 조절
        float multiplier = isPowerOn ? normalVisionMultiplier : darkVisionMultiplier;

        foreach (var enemy in controlledEnemies)
        {
            if (enemy == null) continue;
            enemy.SetVisionMultiplier(multiplier);
        }

        if (switchSfx != null)
            switchSfx.Play();

        Debug.Log($"[FuseBox] PowerOn = {isPowerOn}");
        // TODO: 전기 꺼지면 "라이터를 사용하세요" 같은 힌트 UI
    }
}

using UnityEngine;

public class MinimapBoundsHelper : MonoBehaviour
{
    public static MinimapBoundsHelper Instance { get; private set; }

    [Header("맵 전체를 덮는 Ground 오브젝트")]
    public Transform ground;

    [HideInInspector] public float minX;
    [HideInInspector] public float maxX;
    [HideInInspector] public float minZ;
    [HideInInspector] public float maxZ;

    void Awake()
    {
        Instance = this;

        if (ground != null)
        {
            // 큐브 + Scale 기준
            minX = ground.position.x - ground.localScale.x * 0.5f;
            maxX = ground.position.x + ground.localScale.x * 0.5f;
            minZ = ground.position.z - ground.localScale.z * 0.5f;
            maxZ = ground.position.z + ground.localScale.z * 0.5f;
        }
    }
}

using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public Transform target;          // 따라갈 월드 오브젝트
    public RectTransform minimapRect; // 미니맵 RawImage의 RectTransform

    private RectTransform iconRect;

    void Awake()
    {
        iconRect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (target == null || minimapRect == null || MinimapBoundsHelper.Instance == null) return;

        var b = MinimapBoundsHelper.Instance;
        Vector3 pos = target.position;

        float nx = Mathf.InverseLerp(b.minX, b.maxX, pos.x);
        float nz = Mathf.InverseLerp(b.minZ, b.maxZ, pos.z);

        Vector2 size = minimapRect.rect.size;
        float x = (nx - 0.5f) * size.x;
        float y = (nz - 0.5f) * size.y;

        iconRect.anchoredPosition = new Vector2(x, y);
    }
}

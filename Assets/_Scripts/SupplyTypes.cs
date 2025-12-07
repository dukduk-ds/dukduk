// SupplyTypes.cs
// ───────────────────────────────
// 인벤토리 / 아이템에서 공통으로 쓰는 열거형

public enum SupplyType
{
    Coin,        // 동전 (처음부터 보유)
    Lighter,     // 라이터 (처음부터 보유)
    BrokenPhone, // 깨진 유리폰 (처음부터 보유)
    Earbuds,     // 에어팟 (처음부터 보유)

    Flare,       // 조명탄 (줍거나 구매)
    Gun,         // 총 (줍거나 구매)
    Medicine,    // 약 (줍거나 구매)
    Bread        // 빵 (줍거나 구매)
}

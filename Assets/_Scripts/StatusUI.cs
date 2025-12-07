using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    public PlayerStatus playerStatus;

    [Header("UI Fill")]
    public Image hpFill;
    public Image hungerFill;
    public Image mentalFill;

    void Update()
    {
        if (playerStatus == null) return;

        if (hpFill != null)
            hpFill.fillAmount = playerStatus.hp / Mathf.Max(1f, playerStatus.maxHP);

        if (hungerFill != null)
            hungerFill.fillAmount = playerStatus.hunger / Mathf.Max(1f, playerStatus.maxHunger);

        if (mentalFill != null)
            mentalFill.fillAmount = playerStatus.mental / Mathf.Max(1f, playerStatus.maxMental);
    }
}

using UnityEngine;

public class EarnBadge : MonoBehaviour 
{
    [Range(1, 2)]
    [SerializeField] private int player;
    [SerializeField] private int badgeNum = 0;
    [SerializeField] private BadgeManager badgeManager;

    public void AwardBadge()
    {
        badgeManager.EarnBadge(badgeNum, player);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Badge", menuName = "Scriptable Objects/Badge")]
public class Badge : ScriptableObject
{
    [SerializeField] public string badgeName = "Null";
    [SerializeField] public string badgeDescription = "Description";
    [SerializeField] public Sprite badgeSprite;
    [SerializeField] public int badgeReward = 50;
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

//an in level script that keeps track of what badges were earned in this level specifically
//can be used to display ui, reward the player with money or other things
//this will reset upon reloading a level unlike the BadgeTracker which is a permanent script that remembers what was earned or not
public class BadgeManager : MonoBehaviour
{
    [SerializeField] public int levelID; //what index in the array of all levels badges should this level contain
    private BadgeTracker badgeTracker; //the badges earnable in this level

    public List<Badge> badges;
    public List<Badge> player1Badges; //the badges earned by each player
    public List<Badge> player2Badges;
    LevelBadgeList levelBadgeList;

    private void Start()
    {
        badgeTracker = BadgeTracker.instance;

        //set the earnable badges for this level
        levelBadgeList = badgeTracker.badgeLists[levelID];
    }
    public void EarnBadge(int badgeNum, int player)
    {
        //target badge is the info containing a badge and if its completed or not
        BadgeList targetBadge = levelBadgeList.badgeList[badgeNum];

        //check if that badge number exists 
        if (targetBadge != null )
        {
            //add the badge to the player and tick it as earned in the badge tracker
            switch (player)
            {
                case 1:
                    player1Badges.Add(targetBadge.badge);
                    break;
                case 2:
                    player2Badges.Add(targetBadge.badge);
                    break;
            }

           
        }
        levelBadgeList.badgeList[badgeNum].earned = true;
    }
}

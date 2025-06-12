using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

//player will complete a task and then add a badge to their count of badges
//2 lists, 1 is per level and 1 is per entire game to keep track of all earned badges
//each level will have a set of badges to earn, when a badge is earned regardless of who earned it
//we need to keep track of that in a counter outside of the level

//badges won't have a numbered placement
//will just be added as list
//can only be earned once by each player

//need a database somewhere that keeps track of all earned badges
//a level by level badge manager can keep track of who has earned what badges
//a static list 1 for each player is fine

//need a way to check all earnable badges and keep track of if they were earned (for completionists)
//could manually fill a list on starting a level
//levels will need to have the information before the player even plays them

//could have a list of earnable badges for each level (they would be numbered) and
//whenever we need to trigger the earning of a badge it could say earn badge (level, badgeNum, player) 
//need a static list of badges per level which can be held in a scriptable object
public class BadgeManager : MonoBehaviour
{
    [SerializeField] public BadgeList badgeList; //the badges earnable in this level

    public List<Badge> player1Badges; //the badges earned by each player
    public List<Badge> player2Badges;

    public void EarnBadge(int badgeNum, int player)
    {
        BadgeInfo targetBadge = badgeList.badgeList[badgeNum];
        switch (player)
        {
            case 1:
                player1Badges.Add(targetBadge.badge);
                break;
            case 2:
                player2Badges.Add(targetBadge.badge);
                break;

        }

        targetBadge.earned = true;
    }
}

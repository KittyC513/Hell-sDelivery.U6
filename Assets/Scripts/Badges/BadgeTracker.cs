using UnityEngine;
using System.Collections.Generic;

//a single object that remembers and keeps track of what badges were earned in each level
//also provides a list of earnable badges for each level
public class BadgeTracker : MonoBehaviour
{
    //update and keep track of what badges were earned
    public static BadgeTracker instance;
    public LevelBadgeList[] badgeLists;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

}

[System.Serializable]
public class BadgeList
{
    public Badge badge;
    public bool earned = false;
}

[System.Serializable]
public class LevelBadgeList
{
    public string levelName;
    public BadgeList[] badgeList;
}

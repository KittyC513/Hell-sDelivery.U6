using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "BadgeList", menuName = "Scriptable Objects/BadgeList")]
public class BadgeList : ScriptableObject
{
    public BadgeInfo[] badgeList;
}

[System.Serializable]
public class BadgeInfo
{
    public Badge badge;
    public bool earned = false;
}


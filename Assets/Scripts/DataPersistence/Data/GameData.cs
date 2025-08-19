using UnityEngine;

[System.Serializable]
public class GameData
{
    //keep track what badges have been earned
    public int p1CoinCount;
    public int p2CoinCount;

    public bool p1Invert;
    public bool p2Invert;

    public float p1Sens;
    public float p2Sens;

    public LevelBadgeList[] levelBadgeList;


    //anything in this constructor will be the default values
    //these values are used when a new game is created
    public GameData()
    {
        this.p1CoinCount = 0;
        this.p2CoinCount = 0;
        this.p1Sens = 160;
        this.p2Sens = 160;
        this.p1Invert = false;
        this.p2Invert = false;
    }
}

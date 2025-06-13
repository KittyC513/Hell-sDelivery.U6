using UnityEngine;

[System.Serializable]
public class GameData
{
    //keep track what badges have been earned
    public int p1CoinCount;
    public int p2CoinCount;

    //anything in this constructor will be the default values
    //these values are used when a new game is created
    public GameData()
    {
        this.p1CoinCount = 0;
        this.p2CoinCount = 0;
    }
}

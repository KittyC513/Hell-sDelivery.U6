using UnityEngine;

public class EventData
{
    [Header("NPC Event")]
    public static bool isAcceptedMission_lalah = false;

    [Header("Level1")]
    public static bool craneIsActivated = false;

    //[Header("Initial setting")]
    //public static bool firstTimeEnterScene = true;

    [Header("Scene Info")]
    public static string curSceneName = "";

    public static bool isInverseScreen = false;

    public static bool isSceneChanged = false;

    [Header("Dev Setting")]
    public static bool DevModeIsOn = false;

    [Header("Post Office")]
    public bool cutscene_PO_01 = false;

    [Header("StartScene")]
    public bool cutscene_SC_01 = false;
    public static bool gameStart = false;

}

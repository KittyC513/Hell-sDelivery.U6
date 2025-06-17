using UnityEngine;

public class PlayerSettings : MonoBehaviour, IDataPersistence
{
    //this script contains all the players settings and saves them to the game data

    public static PlayerSettings instance;
    [SerializeField] public float p1Sensitivity;
    [SerializeField] public float p2Sensitivity;

    public delegate void OnSettingsChange();
    public OnSettingsChange onSettingsChange;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    //change the sensitivity 
    //uses the playerNum int from the playerInput script
    public void SetSensitivity(int player, float value)
    {
        if (player == 1)
        {
            p1Sensitivity = value;
        }
        else
        {
            p2Sensitivity = value;
        }

        onSettingsChange?.Invoke();
    }

    //returns sensitivity based on the playerNum int from the playerInput script
    public float GetSensitivity(int player)
    {
        if (player == 1)
        {
            return p1Sensitivity;
        }
        else
        {
            return p2Sensitivity;
        }
    }

    public void LoadData(GameData data)
    {
        p1Sensitivity = data.p1Sens;
        p2Sensitivity = data.p2Sens;

        onSettingsChange?.Invoke();
    }

    public void SaveData(GameData data)
    {
        data.p1Sens = p1Sensitivity;
        data.p2Sens = p2Sensitivity;
    }
}

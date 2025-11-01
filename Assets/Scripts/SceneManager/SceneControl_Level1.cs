using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneControl_Level1 : SceneControlBase<SceneControl_Level1>
{
    public Transform[] spawnpoints;


    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ResetPlayerPos();
        EventData.curSceneName = "Level1";

    }

    // Update is called once per frame
    void Update()
    {
        ResetPlayerPos();
    }

    public void ResetPlayerPos()
    {
        if (!isResetPos)
        {
            GameManager.instance.ResetPlayersPosition(spawnpoints[0], spawnpoints[1]);
            isResetPos = true;
        }
    }

}


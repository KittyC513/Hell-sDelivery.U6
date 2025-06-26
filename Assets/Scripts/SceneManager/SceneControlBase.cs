using UnityEngine;

public class SceneControlBase : MonoBehaviour
{
    private static SceneControlBase instance;
    public static SceneControlBase Instance => instance;

    public bool sceneChanged = false;

    public bool isResetPos = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        sceneChanged = true;
        isResetPos = false;
    }



}

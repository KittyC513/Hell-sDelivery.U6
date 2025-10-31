using UnityEngine;

public class SceneControlBase<T> : MonoBehaviour where T: class
{
    private static T instance;
    public static T Instance => instance;

    public bool sceneChanged = false;

    public bool isResetPos = false;

    private void Awake()
    {
        instance = this as T;
    }

    private void Start()
    {
        EventData.isSceneChanged = true;
        isResetPos = false;
    }



}

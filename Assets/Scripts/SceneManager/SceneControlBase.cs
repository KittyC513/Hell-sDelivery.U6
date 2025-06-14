using UnityEngine;

public class SceneControlBase : MonoBehaviour
{
    private static SceneControlBase instance;
    public static SceneControlBase Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }



}

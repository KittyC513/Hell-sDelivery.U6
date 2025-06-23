using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class SceneControl_Base : MonoBehaviour
{


    private void Awake()
    {

    }

    private void Start()
    {
        GameManager.instance.sceneChanged = true;
    }

}

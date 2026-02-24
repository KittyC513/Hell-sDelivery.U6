using UnityEngine;
using UnityEngine.SceneManagement;

public class SetSelfParent : MonoBehaviour
{
    private void OnEnable()
    {
        transform.parent = SceneManager.GetActiveScene().GetRootGameObjects()[0].transform;
    }
}

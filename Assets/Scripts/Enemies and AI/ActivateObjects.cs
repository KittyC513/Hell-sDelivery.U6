using UnityEngine;

public class ActivateObjects : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToActivate;

    public void ToggleAcitvateObjects(bool toggle)
    {
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].SetActive(toggle);
        }
    }

    
}

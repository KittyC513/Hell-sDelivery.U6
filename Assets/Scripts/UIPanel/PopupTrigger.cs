using UnityEngine;
using UnityEngine.UI;

public class PopupTrigger : MonoBehaviour
{
    [SerializeField] private panel popupImage;

    void OnTriggerEnter(Collider other)
    {
        // UI image appears when player 1 or 2 enters collision box
        if (other.CompareTag("P1") || other.CompareTag("P2"))
        {
            popupImage.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // UI image disappears when player 1 or 2 exits collision box
        if (other.CompareTag("P1") || other.CompareTag("P2"))
        {
            popupImage.enabled = false;
        }
    }
}

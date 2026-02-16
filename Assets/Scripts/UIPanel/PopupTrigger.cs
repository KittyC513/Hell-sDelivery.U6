using UnityEngine;
using UnityEngine.UI;

public class PopupTrigger : MonoBehaviour
{
    [SerializeField] private GameObject popupImage;

    private void Start()
    {
        popupImage.SetActive(false); // Ensure the popup image is initially hidden
        //popupImage.enabled = false; // Ensure the popup image is initially hidden

        print("Image component reference: " + popupImage); // Debug log to check if the reference is assigned

    }
    void OnTriggerEnter(Collider other)
    {
        // UI image appears when player 1 or 2 enters collision box
        if (other.CompareTag("Player"))
        {
            //popupImage.enabled = true;
            popupImage.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // UI image disappears when player 1 or 2 exits collision box
        if (other.CompareTag("Player"))
        {
            //popupImage.enabled = false;
            popupImage.SetActive(false);
        }
    }
}

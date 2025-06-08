using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class BarkUIControl_FlavourNPC : MonoBehaviour
{
    public GameObject textUI;
    public GameObject barkUI;
    public BarkOnIdle barkOnIdle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            TurnOnProximityUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            TurnOffProximityUI();
        }
    }
    void TurnOnProximityUI() 
    {
        textUI.SetActive(true);
        barkUI.SetActive(false);
        barkOnIdle.enabled = true;
    }

    void TurnOffProximityUI()
    {
        textUI.SetActive(false);
        barkUI.SetActive(true);
        barkOnIdle.enabled = false;
    }
}

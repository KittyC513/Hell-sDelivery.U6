using Unity.VisualScripting;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public Material newMaterial; // Assign your desired material in the Inspector
    public bool isTriggered;
    public GameObject targetObject;
    public Collider triggerCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (targetObject != null && triggerCollider != null)
            {
                // Option 1: Check if target's position is within the trigger's bounds
                if (triggerCollider.bounds.Contains(targetObject.transform.position))
                {
                    // Get the Renderer component of the object that entered the trigger
                    Renderer otherRenderer = GetComponent<Renderer>();

                    // If a Renderer exists, change its material
                    if (otherRenderer != null)
                    {
                        isTriggered = true;
                        otherRenderer.material = newMaterial;
                    }

                    Debug.Log("Target is inside the trigger!");
                    // Perform actions when target is in trigger
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object has a specific tag (optional, but good practice)
        if ( Input.GetKeyDown(KeyCode.E))
        {
            // Get the Renderer component of the object that entered the trigger
            Renderer otherRenderer = other.GetComponent<Renderer>();

            // If a Renderer exists, change its material
            if (otherRenderer != null)
            {
                isTriggered = true;
                otherRenderer.material = newMaterial;
            }
        }
    }
}

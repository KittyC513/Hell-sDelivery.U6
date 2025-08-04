using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UIPointerControl : MonoBehaviour
{
    public RectTransform canvasRect;
    public GameObject pointerPrefab;
    public float borderSize = 50f;
    public List<Transform> targets;

    public Transform targetPlayer;
    public Image playerPointer;

    private Dictionary<Transform, RectTransform> pointers = new Dictionary<Transform, RectTransform>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //for multiple pointer, such as enemies
        foreach (Transform target in targets)
        {
            GameObject pointer = Instantiate(pointerPrefab, canvasRect);
            pointers[target] = pointer.GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    #region For player pointer
    public void DisplayPlayerPointer(Camera cam)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(targetPlayer.position);
        bool isOffScreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width / 2 ||
                                              screenPos.y < 0 || screenPos.y > Screen.height;

        playerPointer.gameObject.SetActive(isOffScreen);

        if (isOffScreen)
        {
            // Clamp to screen bounds
            screenPos.x = Mathf.Clamp(screenPos.x, borderSize, Screen.width / 2- borderSize);
            screenPos.y = Mathf.Clamp(screenPos.y, borderSize, Screen.height - borderSize);

            playerPointer.transform.position = screenPos;

            // Direction to target from camera
            Vector3 dir = (targetPlayer.position - Camera.main.transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            targetPlayer.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }
    #endregion


    #region For multiple pointers
    public void DisplayMultipleTargetPointer(Camera cam)
    {
        foreach (Transform target in targets)
        {
            RectTransform pointer = pointers[target];

            Vector3 screenPos = cam.WorldToScreenPoint(target.position);
            bool isOffScreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width ||
                                                  screenPos.y < 0 || screenPos.y > Screen.height;

            pointer.gameObject.SetActive(isOffScreen);

            if (isOffScreen)
            {
                // Clamp position to screen edges
                screenPos.x = Mathf.Clamp(screenPos.x, borderSize, Screen.width - borderSize);
                screenPos.y = Mathf.Clamp(screenPos.y, borderSize, Screen.height - borderSize);

                pointer.position = screenPos;

                //Rotate pointer to face the target direction
                Vector3 dir = (target.position - cam.transform.position).normalized;
                Vector3 flatDir = new Vector3(dir.x, dir.y, 0); //for 2D UI rotation
                float angle = Mathf.Atan2(flatDir.y, flatDir.x) * Mathf.Rad2Deg;
                pointer.rotation = Quaternion.Euler(0, 0, angle - 90);
            }

        }
    }
    #endregion

}

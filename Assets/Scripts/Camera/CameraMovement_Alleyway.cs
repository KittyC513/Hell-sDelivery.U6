using System.Collections;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CameraMovement_Alleyway : MonoBehaviour
{
    public float lerpSpeed = 5f;
    public Transform playerPos;
    public Vector3 desiredPos;
    public bool isTransitioning = false;

    public GameObject transition_p1;
    public GameObject transition_p2;

    private bool isP1 = false;
    private bool isP2 = false;

    private void Start()
    {
        if(playerPos.gameObject.layer == LayerMask.NameToLayer("Player1"))
        {
            isP1 = true;
        }
        else if (playerPos.gameObject.layer == LayerMask.NameToLayer("Player2"))
        {
            isP2 = true;

        }

    }
    void LateUpdate()
    {
        desiredPos = new Vector3(playerPos.position.x, this.transform.position.y, this.transform.position.z);
        transform.position = Vector3.Lerp(this.transform.position, desiredPos, Time.deltaTime * lerpSpeed);
    }

    private void Update()
    {
        if(isTransitioning)
            StartCoroutine(CamTransition());
    }
    IEnumerator CamTransition()
    {
        if(isP1)
            transition_p1.SetActive(true);
        else if (isP2)
            transition_p2.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        if (isP1)
            transition_p1.SetActive(false);
        else if (isP2)
            transition_p2.SetActive(false);

        isTransitioning = false;
    }
}

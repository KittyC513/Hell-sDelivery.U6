using NUnit.Framework.Constraints;
using PixelCrushers.DialogueSystem;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarkUIControl_FlavourNPC : MonoBehaviour
{
    public GameObject textUI_P1;
    public GameObject barkUI_P1;
    public GameObject textUI_P2;
    public GameObject barkUI_P2;
    public BarkOnIdle barkOnIdle;

    public Vector3 halfSize = new Vector3(5,5,5);
    public float maxDistance = 5f;

    public bool isInRange_P1 = false;
    public bool isInRange_P2 = false;
    
    private RaycastHit[] hits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckRange();
        UIControl();
    }

    private void CheckRange()
    {
        //when players are in bark range, the text UI will trigger, otherwise, it will stay bark UI
        hits = Physics.BoxCastAll(this.transform.position, halfSize, this.transform.forward,Quaternion.identity, maxDistance, 
                                                1 << LayerMask.NameToLayer("Player1") | 1 << LayerMask.NameToLayer("Player2"));
        // group the hits when it comes from the same object with multiple colliders
        hits = hits.GroupBy(h => h.collider.gameObject).Select(g => g.First()).ToArray();
        //print(hits.Length);

        switch (hits.Length) 
        {
            case 0:
                hits = null;
                barkOnIdle.enabled = false;
                isInRange_P1 = false;
                isInRange_P2 = false;
                break;
            case 1:
                if (hits[0].transform.gameObject.layer == LayerMask.NameToLayer("Player1"))
                {
                    isInRange_P1 = true;
                    print("FindPlayer1");
                }
                else
                {
                    isInRange_P1 = false;
                }

                if (hits[0].transform.gameObject.layer == LayerMask.NameToLayer("Player2"))
                {
                    isInRange_P2 = true;
                    print("FindPlayer2");
                }
                else
                {
                    isInRange_P2 = false;
                }
                    break;
            case 2:
                isInRange_P1 = true;
                isInRange_P2 = true;
                break;
       
        }

        //activate the bark
        if(hits != null && hits.Length > 0)
            barkOnIdle.enabled = true;


        //if (hits.Length > 0 )
        //{
        //    print(hits.Length);
        //    barkOnIdle.enabled = true;
        //    for (int i = 0; i < hits.Length - 1; i++)
        //    {
        //        if(hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Player1"))
        //        {
        //            isInRange_P1 = true;
        //            print("FindPlayer1");
        //        }

        //        if (hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Player2"))
        //        {
        //            isInRange_P2 = true;
        //            print("FindPlayer2");
        //        }
        //    }
        //}
        //else
        //{
        //    hits = null;
        //    barkOnIdle.enabled = false;
        //    isInRange_P1 = false;
        //    isInRange_P2 = false;
        //}
    }

    private void UIControl()
    {
        if (isInRange_P1)
        {
            TurnOnProximityUIP1();
        }
        else
        {
            TurnOffProximityUIP1();
        }

        if (isInRange_P2)
        {
            TurnOnProximityUIP2();
        }
        else
        {
            TurnOffProximityUIP2();
        }
    }
    void TurnOnProximityUIP1() 
    {
        textUI_P1.SetActive(true);
        barkUI_P1.SetActive(false);
    }

    void TurnOffProximityUIP1()
    {
        textUI_P1.SetActive(false);
        barkUI_P1.SetActive(true);
    }

    void TurnOnProximityUIP2()
    {
        textUI_P2.SetActive(true);
        barkUI_P2.SetActive(false);
    }

    void TurnOffProximityUIP2()
    {
        textUI_P2.SetActive(false);
        barkUI_P2.SetActive(true);
    }

    //provide visual support
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, 2 * halfSize);
    }
}

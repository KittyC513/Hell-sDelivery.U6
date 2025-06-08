using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//slot in a cosmetic
//place it in the correct place and give it a gameobject to display it
public class PlayerCosmeticHandler : MonoBehaviour
{
    public enum CosmeticPlace { head, torso, leftArm, rightArm, leftLeg, RightLeg }
    [HideInInspector]public CosmeticPlace cosmeticPlace;

    private Dictionary<CosmeticPlace, Transform> cosmeticToTransform;

    [SerializeField] private CosmeticPiece cosmeticPiece;

    [SerializeField] private GameObject cosmeticHolder;
    private List<GameObject> currentCosmetic;

    //the transforms for each piece, where should the cosmetic be attached to
    [Space, Header("Parent Transforms")]
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform torsoTransform;
    [SerializeField] private Transform leftArmTransform;
    [SerializeField] private Transform rightArmTransform;
    [SerializeField] private Transform leftLegTransform;
    [SerializeField] private Transform rightLegTransform;


    private void Awake()
    {
        cosmeticToTransform = new Dictionary<CosmeticPlace, Transform>();

        //fill the dictionary so you can put in the enum and get the correct transform
        cosmeticToTransform.Add(CosmeticPlace.head, headTransform);
        cosmeticToTransform.Add(CosmeticPlace.torso, torsoTransform);
        cosmeticToTransform.Add(CosmeticPlace.leftArm, leftArmTransform);
        cosmeticToTransform.Add(CosmeticPlace.rightArm, rightArmTransform);
        cosmeticToTransform.Add(CosmeticPlace.leftLeg, leftLegTransform);
        cosmeticToTransform.Add(CosmeticPlace.RightLeg, rightLegTransform);

        currentCosmetic = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ChangeCosmetic();
        }
    }

    private void ChangeCosmetic()
    {
        foreach (GameObject obj in currentCosmetic)
        {
            //destroy the old cosmetic
            Destroy(obj);
        }

        //clear the list of cosmetic parts
        currentCosmetic.Clear();
      
        foreach (CosmeticPart part in cosmeticPiece.cosmeticParts)
        {
            //get the target transform to be the parents
            Transform targetTransform = cosmeticToTransform[part.cosmeticPlace];

            //create an empty object to nest the cosmetic into
            GameObject emptyObj = Instantiate(cosmeticHolder, targetTransform.position, targetTransform.rotation, targetTransform);
            
            //add the empty object to be destroyed and cleared later
            currentCosmetic.Add(emptyObj);
            
            //create the actual cosmetic piece and parent it to the empty object
            GameObject cos = Instantiate(part.modelObj, emptyObj.transform.position, emptyObj.transform.rotation ,emptyObj.transform);
            
        }


    }

}

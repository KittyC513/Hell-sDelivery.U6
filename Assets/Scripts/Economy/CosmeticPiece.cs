using UnityEngine;

[CreateAssetMenu(fileName = "CosmeticPiece", menuName = "Scriptable Objects/CosmeticPiece")]
public class CosmeticPiece : ScriptableObject
{
    [SerializeField] public string pieceName;

    [SerializeField] public CosmeticPart[] cosmeticParts;
}

[System.Serializable]
public struct CosmeticPart
{
    //this is to be used if multiple cosmetic parts are in a piece, for example 2 boots each need their own leg and model
    [SerializeField] public PlayerCosmeticHandler.CosmeticPlace cosmeticPlace;
    [SerializeField] public GameObject modelObj;
}

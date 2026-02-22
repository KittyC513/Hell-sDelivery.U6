using System.Collections.Generic;
using UnityEngine;

public class CarjackCar : MonoBehaviour
{
    private CarjackMinigameManager minigameManager;
    [HideInInspector] public CarUnlockSlider p1Slider;
    [HideInInspector] public CarUnlockSlider p2Slider;
    private CarUnlockSlider activeSlider;
    private bool active = false;


    private bool canInput = true;
    private CarjackMinigameManager.HeatCheck heat;
    [SerializeField] private MeshRenderer mesh;

    [SerializeField] private Color hotColour;
    [SerializeField] private Color warmColour;
    [SerializeField] private Color coolColour;
    [SerializeField] private Color coldColour;
   
    public bool unlocked = false;

    public virtual void Start()
    {
        minigameManager = FindFirstObjectByType<CarjackMinigameManager>();
        SingleCamBillboardUI singleBillboard = GetComponent<SingleCamBillboardUI>();
        singleBillboard.cameraToBillboard = Camera.main;
        p1Slider = minigameManager.p1Slider;
        p2Slider = minigameManager.p2Slider;
        
    }

    public void StartUnlockMinigame(PlayerInputDetection playerInput, InteractableObject interactable)
    {
        if (!unlocked)
        {
            CarUnlockSlider slider;
            active = true;

            //freeze the inputs of the player who started the minigame
            //and grab the correct slider to activate
            if (playerInput.playerNum == 1) 
            {
                slider = p1Slider;
                GameManager.instance.FreezePlayer1();
            }
            else 
            {
                slider = p2Slider;
                GameManager.instance.FreezePlayer2();
            }
            
            //reset the slider minigame
            slider.ResetMinigame();
            slider.Activate(this.gameObject, playerInput.gameObject.transform.position, playerInput);

            //set the variables
            activeSlider = slider;
            //currentPlayer = playerInput;
        }
       
    }


    public virtual void UnlockCar(PlayerInputDetection player)
    {
        heat = minigameManager.CheckDistance(this.transform.position, player);
        Debug.Log(heat);
        unlocked = true;
        switch (heat)
        {
            case CarjackMinigameManager.HeatCheck.target:
                GetComponentInChildren<MeshRenderer>().material = null;
                break;
            case CarjackMinigameManager.HeatCheck.hot:
                mesh.material.SetColor("_NewColour", hotColour);
                break;
            case CarjackMinigameManager.HeatCheck.warm:
                mesh.material.SetColor("_NewColour", warmColour);
                break;
            case CarjackMinigameManager.HeatCheck.cool:
                mesh.material.SetColor("_NewColour", coolColour);
                break;
            case CarjackMinigameManager.HeatCheck.cold:
                mesh.material.SetColor("_NewColour", coldColour);
                break;
        }
    }
    
}

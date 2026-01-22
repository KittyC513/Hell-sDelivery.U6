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

    [SerializeField] private Material hotMaterial;
    [SerializeField] private Material warmMaterial;
    [SerializeField] private Material coolMaterial;
    [SerializeField] private Material coldMaterial;

    public bool unlocked = false;

    public virtual void Start()
    {
        minigameManager = FindFirstObjectByType<CarjackMinigameManager>();
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
                GetComponentInChildren<MeshRenderer>().material = hotMaterial;
                break;
            case CarjackMinigameManager.HeatCheck.warm:
                GetComponentInChildren<MeshRenderer>().material = warmMaterial;
                break;
            case CarjackMinigameManager.HeatCheck.cool:
                GetComponentInChildren<MeshRenderer>().material = coolMaterial;
                break;
            case CarjackMinigameManager.HeatCheck.cold:
                GetComponentInChildren<MeshRenderer>().material = coldMaterial;
                break;
        }
    }
    
}

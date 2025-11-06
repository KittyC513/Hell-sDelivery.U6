using UnityEngine;

public class CarjackCar : MonoBehaviour
{
    private CarjackMinigameManager minigameManager;
    private CarUnlockSlider p1Slider;
    private CarUnlockSlider p2Slider;
    private CarUnlockSlider activeSlider;
    private bool active = false;
    private PlayerInputDetection currentPlayer;

    private bool canInput = true;
    private CarjackMinigameManager.HeatCheck heat;

    [SerializeField] private Material hotMaterial;
    [SerializeField] private Material warmMaterial;
    [SerializeField] private Material coolMaterial;
    [SerializeField] private Material coldMaterial;

    private void Start()
    {
        minigameManager = FindFirstObjectByType<CarjackMinigameManager>();
        p1Slider = minigameManager.p1Slider;
        p2Slider = minigameManager.p2Slider;
        
    }

    public void StartUnlockMinigame(PlayerInputDetection playerInput)
    {
        if (!active)
        {
            CarUnlockSlider slider;
            active = true;

            if (playerInput.playerNum == 1) slider = p1Slider;
            else slider = p2Slider;

            //reset the slider minigame
            slider.ResetMinigame();
            slider.Activate(this.gameObject);

            //set the variables
            activeSlider = slider;
            currentPlayer = playerInput;
        }
       
    }

    private void Update()
    {
        if (active)
        {
            //read inputs for the active slider
            if (currentPlayer?.jumpPressed == true && canInput)
            {
                canInput = false;
                activeSlider?.CallInput();
            }


            //make sure they cant hold down the button
            if (currentPlayer?.jumpPressed != true)
            {
                if (canInput == false)
                {
                    canInput = true;
                }
            }

            if (!activeSlider.active)
            {
                //check if the game was completed
                if (activeSlider.completed == true)
                {
                    active = false;
                    currentPlayer = null;
                    activeSlider = null;
                }
                else
                {
                    //the game was failed
                    active = false;
                    currentPlayer = null;
                    activeSlider = null;
                }
            }

        }
        
    
    }
    
    public void UnlockCar()
    {
        heat = minigameManager.CheckDistance(this.transform.position);
        Debug.Log(heat);
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

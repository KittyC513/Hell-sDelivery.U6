using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectedControl : MonoBehaviour
{
    public Text text_leftSelected;
    public Text text_rightSelected;
    public GameObject canvas_p1;
    public GameObject canvas_p2;

    private float inputCd_p1 = 0.2f;
    private float lastInputTime_p1 = 0;

    private float inputCd_p2 = 0.2f;
    private float lastInputTime_p2 = 0;

    public bool leftIsSelected_p1 = false;
    public bool leftIsSelected_p2 = false;
    public bool rightIsSelected_p1 = false; 
    public bool rightIsSelected_p2 = false;

    public bool onReady = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        text_leftSelected.text = "";
        text_rightSelected.text = "";
        canvas_p1.SetActive(false);
        canvas_p2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isOnCharacterSelection)
        {
            //OnCharacterSelected();
            //TextDemonstrate();
            //OnReady();

            //testing 
            TestingScene();

        }
    }

    public void OnCharacterSelected()
    {
        canvas_p1.SetActive(true);
        canvas_p2.SetActive(true);
        //p1
        if (Time.time - lastInputTime_p1 < inputCd_p1) return;
        if(GameManager.instance.player1 != null)
        {
            PlayerInputDetection playerInput = GameManager.instance.player1.GetComponent<PlayerInputDetection>();
            if (playerInput.GetHorizontalMovement().x < -0.5f)
            {
                rightIsSelected_p1 = false;
                leftIsSelected_p1 = true;
                lastInputTime_p1 = Time.time;
            }

            if (playerInput.GetHorizontalMovement().x > 0.5)
            {
                rightIsSelected_p1 = true;
                leftIsSelected_p1 = false;
                lastInputTime_p1 = Time.time;
            }
        }
        

        //p2
        if (Time.time - lastInputTime_p2 < inputCd_p2) return;
        if(GameManager.instance.player2 != null)
        {
            PlayerInputDetection playerInput = GameManager.instance.player2.GetComponent<PlayerInputDetection>();

            if (playerInput.GetHorizontalMovement().x < -0.5f)
            {
                rightIsSelected_p2 = false;
                leftIsSelected_p2 = true;
                lastInputTime_p2 = Time.time;
            }

            if (playerInput.GetHorizontalMovement().x > 0.5f)
            {
                leftIsSelected_p2 = false;
                rightIsSelected_p2 = true;
                lastInputTime_p2 = Time.time;
            }
        }

    }

    public void TextDemonstrate() 
    {
        if(leftIsSelected_p1 && !leftIsSelected_p2) 
        {
            text_leftSelected.text = "P1";
        }
        else if(!leftIsSelected_p1 && leftIsSelected_p2)
        {
            text_leftSelected.text = "P2";
        }
        else if(leftIsSelected_p1 && leftIsSelected_p2)
        {
            text_leftSelected.color = Color.grey;
            text_leftSelected.text = "P1 & P2";
        }
        else
        {
            text_leftSelected.text = "";
        }

        if (rightIsSelected_p1 && !rightIsSelected_p2)
        {
            text_rightSelected.text = "P1";
        }
        else if (!rightIsSelected_p1 && rightIsSelected_p2)
        {
            text_rightSelected.text = "P2";
        }
        else if (rightIsSelected_p1 && rightIsSelected_p2)
        {
            text_rightSelected.text = "P1 & P2";
        }
        else
        {
            text_rightSelected.text = "";
        }
    }

    public void OnReady()
    {
        //When players select their respective characters, a 3-second countdown will begin before the game starts
        if (leftIsSelected_p1 && rightIsSelected_p2 ||
            rightIsSelected_p1 && leftIsSelected_p2)
            StartCoroutine(StartGameCountDown());
        else
        {
            onReady = false;
            StopCoroutine(StartGameCountDown());
        }



        if (onReady)
        {
            if (leftIsSelected_p1)
            {
                GameManager.instance.cam_p1.rect = new Rect(0, 0, 0.5f, 1);
                GameManager.instance.cam_p2.rect = new Rect(0.5f, 0, 0.5f, 1);
                SelectPlayer1Model_p1("Models/Characters/Shmink");
                SelectPlayer1Model_p2("Models/Characters/Shmonk");

            }

            if (leftIsSelected_p2)
            {
                EventData.isInverseScreen = true;
                GameManager.instance.cam_p2.rect = new Rect(0, 0, 0.5f, 1);
                GameManager.instance.cam_p1.rect = new Rect(0.5f, 0, 0.5f, 1);
                SelectPlayer1Model_p1("Models/Characters/Shmonk");
                SelectPlayer1Model_p2("Models/Characters/Shmink");
            }
            GameManager.instance.isOnCharacterSelection = false;
            //SceneManager.LoadScene("Alleyway_tutorial_testing");
            //SceneManager.LoadScene("Alleyway_tutorial_testing");
            canvas_p1.SetActive(false);
            canvas_p2.SetActive(false);
            SceneControl_MainMenu.Instance.cutscene_sockThief.SetActive(true);
        }
    }

    IEnumerator StartGameCountDown()
    {
        print("Game starts in 3 seconds");
        yield return new WaitForSeconds(3f);
        onReady = true;
    }


    #region Character Model
    // Shmink
    public GameObject SelectPlayer1Model_p1(string location)
    {
        // 1. Load prefab from Resources
        GameObject prefab = Resources.Load<GameObject>(location);

        if (prefab == null)
        {
            Debug.LogError("Failed to load player model from location: " + location);
            return null;
        }

        // 2. Instantiate prefab into scene
        GameObject playerInstance = Instantiate(prefab);
        playerInstance.transform.parent = GameManager.instance.playerModel_p1.transform;
        Transform t = playerInstance.transform;
        t.localPosition = new Vector3(0f, -1.2f, 0f);
        t.localRotation = Quaternion.Euler(0f, 180f, 0f);

        //GameManager.instance.InputDetection_p1.characterModelObj = playerInstance;
        GameManager.instance.playerStateMachine_p1.anim = playerInstance.GetComponent<Animator>();
        GameManager.instance.PlayerController_p1.anim = playerInstance.GetComponent<Animator>();

        return playerInstance;
    }
    // Shmonk
    public GameObject SelectPlayer1Model_p2(string location)
    {
        // 1. Load prefab from Resources
        GameObject prefab = Resources.Load<GameObject>(location);

        if (prefab == null)
        {
            Debug.LogError("Failed to load player model from location: " + location);
            return null;
        }

        // 2. Instantiate prefab into scene
        GameObject playerInstance = Instantiate(prefab);
        playerInstance.transform.parent = GameManager.instance.playerModel_p2.transform;
        Transform t = playerInstance.transform;
        t.localPosition = new Vector3(0f, -1.2f, 0f);
        t.localRotation = Quaternion.Euler(0f, 180f, 0f);

        //GameManager.instance.InputDetection_p2.characterModelObj = playerInstance;
        GameManager.instance.playerStateMachine_p2.anim = playerInstance.GetComponent<Animator>();
        GameManager.instance.PlayerController_p2.anim = playerInstance.GetComponent<Animator>();
        return playerInstance;
    }

    public void TestingScene()
    {
        GameManager.instance.cam_p1.rect = new Rect(0, 0, 0.5f, 1);
        GameManager.instance.cam_p2.rect = new Rect(0.5f, 0, 0.5f, 1);
        SelectPlayer1Model_p1("Models/Characters/Shmink");
        SelectPlayer1Model_p2("Models/Characters/Shmonk");
        SceneControl_MainMenu.Instance.cutscene_sockThief.SetActive(true);
        GameManager.instance.isOnCharacterSelection = false;
    }
    #endregion
}

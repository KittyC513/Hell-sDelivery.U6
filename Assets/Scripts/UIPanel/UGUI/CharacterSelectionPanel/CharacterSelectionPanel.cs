using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionPanel : MonoBehaviour
{
    public Image img_left;
    public Image img_right;

    private float inputCd_p1 = 0.2f;
    private float lastInputTime_p1 = 0;

    private float inputCd_p2 = 0.2f;
    private float lastInputTime_p2 = 0;

    public bool leftIsSelected_p1 = true;
    public bool leftIsSelected_p2 = false;

    public bool onReady = false;
    public Image img_start;

    public GameObject imgLeft_p1;
    public GameObject imgLeft_p2;

    public GameObject imgRight_p1;
    public GameObject imgRight_p2;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        img_start.gameObject.SetActive(false);

        leftIsSelected_p1 = false;
        leftIsSelected_p2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        PanelControlP1();
        PanelControlP2();
        ButtonStatus();
        OnReady();
    }

    public void PanelControlP1()
    {
        if (Time.time - lastInputTime_p1 < inputCd_p1) return;
        if (GameManager.instance.player1.GetComponent<PlayerInputDetection>().GetHorizontalMovement().x < -0.5f)
        {
            leftIsSelected_p1 = true;
            lastInputTime_p1 = Time.time;
            print("Left is selected for Player 1");
            print("right is selected for Player 2");


        }

        if (GameManager.instance.player1.GetComponent<PlayerInputDetection>().GetHorizontalMovement().x > 0.5)
        {
            leftIsSelected_p1 = false;
            lastInputTime_p1 = Time.time;
        }


    }

    public void PanelControlP2()
    {
        if (Time.time - lastInputTime_p2 < inputCd_p2) return;
        if (GameManager.instance.player2.GetComponent<PlayerInputDetection>().GetHorizontalMovement().x < -0.5f)
        {
            leftIsSelected_p2 = true;
            lastInputTime_p2 = Time.time;
        }

        if (GameManager.instance.player2.GetComponent<PlayerInputDetection>().GetHorizontalMovement().x > 0.5f)
        {
            leftIsSelected_p2 = false;
            lastInputTime_p2 = Time.time;
        }
    }

    public void ButtonStatus()
    {
        if (leftIsSelected_p1 && !leftIsSelected_p2 || !leftIsSelected_p1 && leftIsSelected_p2)
        {
            img_left.color = Color.black;
            img_right.color = Color.black;

            onReady = true;
        }

        if(leftIsSelected_p1 && leftIsSelected_p2)
        {
            img_left.color = Color.red;
            img_right.color = Color.white;

            onReady = false;
        }


        if (!leftIsSelected_p1 && !leftIsSelected_p2)
        {
            img_left.color = Color.white;
            img_right.color = Color.red;

            onReady = false;
        }

        SelectedIconControl();

    }

    public void OnReady()
    {
        if (onReady)
        {
            img_start.gameObject.SetActive(true);

            if (GameManager.instance.player1.GetComponent<PlayerInputDetection>().crouchPressed ||
                GameManager.instance.player2.GetComponent<PlayerInputDetection>().crouchPressed)
            {
                if (leftIsSelected_p1)
                {
                    GameManager.instance.cam_p1.rect = new Rect(0, 0, 0.5f, 1);
                    GameManager.instance.SelectPlayer1Model("Models/Character/Shmink");
                    GameManager.instance.SelectPlayer2Model("Models/Character/Shmonk");
                    GameManager.instance.cam_p2.rect = new Rect(0.5f, 0, 0.5f, 1);
                }

                if (leftIsSelected_p2)
                {
                    EventData.isInverseScreen = true;
                    GameManager.instance.cam_p2.rect = new Rect(0, 0, 0.5f, 1);
                    GameManager.instance.SelectPlayer2Model("Models/Character/Shmink");
                    GameManager.instance.SelectPlayer1Model("Models/Character/Shmonk");
                    GameManager.instance.cam_p1.rect = new Rect(0.5f, 0, 0.5f, 1);
                }

                GameManager.instance.isOnCharacterSelection = false;
                SceneManager.LoadScene("Alleyway_tutorial_testing");
            }
        }
        else
        {
            img_start.gameObject.SetActive(false);
        }
    }

    public void SelectedIconControl()
    {
        imgLeft_p1.SetActive(leftIsSelected_p1);
        imgRight_p1.SetActive(!leftIsSelected_p1);

        imgLeft_p2.SetActive(leftIsSelected_p2);
        imgRight_p2.SetActive(!leftIsSelected_p2);
    }
}

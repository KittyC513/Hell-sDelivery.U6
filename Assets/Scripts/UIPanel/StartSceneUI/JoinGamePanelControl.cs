using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JoinGamePanelControl : MonoBehaviour
{
    [SerializeField]
    private Text text_joinP1;
    [SerializeField]
    private Image image_p1;
    [SerializeField]
    private Text text_joinP2;
    [SerializeField]
    private Image image_p2;

    public PlayerManager playerManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text_joinP1.text = "Join";
        text_joinP2.text = "Join";
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.isOnJoinGamePanel)
            OnPlayerJoined();
    }

    public void OnPlayerJoined()
    {
        if (playerManager.players.Count == 1 && text_joinP1.text != "Ready")
        {
            text_joinP1.text = "Ready";
            image_p1.gameObject.SetActive(false);
        }

        if (playerManager.players.Count == 2)
        {
            text_joinP2.text = "Ready";
            image_p2.gameObject.SetActive(false);
            SceneControl_MainMenu.Instance.cutscene_characterSelected.SetActive(true);
            GameManager.instance.isOnCharacterSelection = true;
            this.gameObject.SetActive(false);
            GameManager.instance.isOnJoinGamePanel = false;
        }
    }
}

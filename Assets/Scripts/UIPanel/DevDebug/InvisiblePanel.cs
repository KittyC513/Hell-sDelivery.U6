using System.Linq;
using UnityEngine;

public class InvisiblePanel : BasePanel<InvisiblePanel>
{
    public CustomGUIToggle toggle_on;
    public CustomGUIToggle toggle_off;
    public PlayerManager playerManager;
    public CustomGUIButton btn_exit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggle_on.toggleEvent += (isOn) =>
        {
            if (playerManager.players[0] != null)
                playerManager.players[0].gameObject.layer = LayerMask.NameToLayer("Player1");
            if (playerManager.players[1] != null)
                playerManager.players[1].gameObject.layer = LayerMask.NameToLayer("Player2");
        };

        toggle_off.toggleEvent += (isOn) =>
        {
            if(playerManager.players[0] != null)
                playerManager.players[0].gameObject.layer = LayerMask.NameToLayer("Invisible_Player1");
            if (playerManager.players[1] != null)
                playerManager.players[1].gameObject.layer = LayerMask.NameToLayer("Invisible_Player2");
        };

        btn_exit.clickEvent += () =>
        {
            HideMe();
        };

        HideMe();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

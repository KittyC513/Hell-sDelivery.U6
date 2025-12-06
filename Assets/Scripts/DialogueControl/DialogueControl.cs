using UnityEngine;

public class DialogueControl : MonoBehaviour
{
    public Transform[] enterPoints;
    private bool isResetPos = false;

    public void OnConversation()
    {
        GameManager.instance.FreezeBothPlayers();
        GameManager.instance.DisableBothPlayersCam();
    }
    public void EndConversation()
    {
        GameManager.instance.UnFreezeBothPlayers();
        GameManager.instance.EnableBothPlayersCam();
        isResetPos = false;
    }

    public void ResetPlayerPos()
    {
        if (!isResetPos)
        {
            GameManager.instance.player1.transform.position = enterPoints[0].position;
            GameManager.instance.player2.transform.position = enterPoints[1].position;
            
            /***********rotation related to camera direction****************/

            
            isResetPos = true;
        }
    }
}

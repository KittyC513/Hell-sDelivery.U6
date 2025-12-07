using UnityEngine;

public class DialogueControl : MonoBehaviour
{
    public Transform[] spawnPoints;
    private bool isResetPos = false;
    public Transform dialogueCam;

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
            print("resetting pos");
            Transform rotTransform = dialogueCam;
            //GameManager.Instance.player1.transform.position = spawnPoints[0].position;
            //GameManager.Instance.player2.transform.position = spawnPoints[1].position;
            GameManager.Instance.ResetPlayersPosition(spawnPoints[0], spawnPoints[1]);

            /***********rotation related to camera direction****************/
            //GameManager.Instance.player1.transform.rotation = Quaternion.Euler(0, rotTransform.eulerAngles.y, 0);
            //GameManager.Instance.player2.transform.rotation = Quaternion.Euler(0, rotTransform.eulerAngles.y, 0);
            GameManager.instance.RotatePlayersTo(dialogueCam);
            isResetPos = true;
        }
    }
}

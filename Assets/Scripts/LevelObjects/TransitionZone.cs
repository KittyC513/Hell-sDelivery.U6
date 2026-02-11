using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TransitionZone : MonoBehaviour
{
    private int activePlayers = 0;
    [SerializeField] private int requiredPlayers = 2;
    [SerializeField] private TransitionScenes transitionScenes;
    [SerializeField] private bool canTransition = true;
    [SerializeField] private BillboardUI billboardUI;

    private TextMeshProUGUI p1Counter;
    private TextMeshProUGUI p2Counter;

    private bool init = false;

    public void Awake()
    {
        if (billboardUI != null)
        {
            billboardUI.onInitialize += SetupPlayerCounter;
        }
        
    }

    public void SetupPlayerCounter()
    {
        p1Counter = billboardUI.images[0].GetComponentInChildren<TextMeshProUGUI>();
        p2Counter = billboardUI.images[1].GetComponentInChildren<TextMeshProUGUI>();

        init = true;

        UpdatePlayerCounter();
    }

    public void UpdatePlayerCounter()
    {
        if (init && activePlayers > 0)
        {
            p1Counter.text = activePlayers / 2 + "/2";
            p2Counter.text = activePlayers / 2 + "/2";
        }
        else if (init)
        {
            p1Counter.text = 0 + "/2";
            p2Counter.text = 0 + "/2";
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activePlayers += 1;
            UpdatePlayerCounter();
        }
        
        if (activePlayers >= requiredPlayers && canTransition)
        {
            canTransition = false;
            transitionScenes.StartTransition();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activePlayers -= 1;
            UpdatePlayerCounter();
        }
    }

  
}

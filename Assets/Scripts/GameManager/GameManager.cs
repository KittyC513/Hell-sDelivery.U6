using UnityEngine;
using UnityEngine.InputSystem.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Character Selection")]
    public GameObject characterSelectionUI;
    public bool isOnCharacterSelection = false;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isOnCharacterSelection)
        {
            OnCharacterSelectionPanel();
        }
    }

    #region Character Selection 

    private void OnCharacterSelectionPanel()
    {
        characterSelectionUI.SetActive(true);
    }
    #endregion
}

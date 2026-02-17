using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TransitionScenes : MonoBehaviour
{
    private Vector3 player1Pos;
    private Vector3 players2Pos;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject sceneRoot;

    [SerializeField] private bool unloadStartScene = false;
    [SerializeField] private bool shouldSavePosition = false;
    private Scene loadedScene;
    private Scene startingScene;

    private bool transitionStarted = false;

    private bool savedPos = false;
    private bool waitingOnReturn = false;

    [SerializeField] public UnityEvent OnReturnToScene; //calls when the object is re-enabled after calling a transition to start

    private void Start()
    {
        startingScene = this.gameObject.scene;
    }

    private void Update()
    {
        //DEBUG ONLY REMOVE AFTER TESTING
        if (Input.GetKeyDown(KeyCode.Z) && !transitionStarted)
        {
            //StartTransition();
        }
    }

    private void OnEnable()
    {
        if (savedPos && shouldSavePosition)
        {
            GameManager.instance.player1.transform.position = player1Pos;
            GameManager.instance.player2.transform.position = players2Pos;
        }

        if (waitingOnReturn)
        {
            Debug.Log("On Return");
            OnReturnToScene.Invoke();
            waitingOnReturn = false;
        }
    }


    public void StartTransition()
    {
        transitionStarted = true;
        
        //reference the player positions to use later
        player1Pos = GameManager.instance.player1.transform.position;
        players2Pos = GameManager.instance.player2.transform.position;

        GameManager.instance.DropPlayerItems();
        waitingOnReturn = true;
        
        savedPos = true;

        //check the scene root to see if we should enable it
        Scene _temp = SceneManager.GetSceneByName(sceneToLoad);

        //this checks if the scene to load is loaded already
        if (_temp.IsValid())
        {
            //enable the root gameobject(s)
            GameObject[] _root = _temp.GetRootGameObjects();

            if(unloadStartScene)
            {
                //set this scenes root as disabled
                sceneRoot.SetActive(false);
                //unload this scene
                SceneManager.UnloadSceneAsync(startingScene);
            }
            else
            {
                //set this scenes root as disabled
                sceneRoot.SetActive(false);
            }

            
            for (int i = 0; i < _root.Length; i++)
            {
                _root[i].SetActive(true);
            }


            EventData.curSceneName = sceneToLoad;
            transitionStarted = false;
        }
        else
        {
            //start loading the new scene
            StartCoroutine(WaitOnLoadScene());
        }
        
    }

    private IEnumerator WaitOnLoadScene()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        //wait for the scene to be loaded
        while (!loading.isDone)
        {
            loadedScene = SceneManager.GetSceneByName(sceneToLoad);

            if (unloadStartScene)
            {
                SceneManager.UnloadSceneAsync(startingScene);
            }
            else
            {
                //disable the old scene for reactivation later
                //this will also disable this script for the time being
                sceneRoot.SetActive(false);
            }
            transitionStarted = false;
            yield return null;
        }
    }

   
}

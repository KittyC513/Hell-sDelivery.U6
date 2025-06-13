using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool createDataIfNull = false; //create a new game data if there is nothing to load, testing only, disable on build
    [Header("File Storage Settings")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;
  
    public static DataPersistenceManager Instance { get; private set; }

    private GameData gameData;

    private List<IDataPersistence> persitenceObjs;
    private FileDataHandler fileDataHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager, destroying the newest one");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

        //create the file data handler which can save and access our data
        //application.persistentDataPath points to a standard directory for saving persistent data
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void NewGame()
    {
        //create a new game data which is our save data
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //get the game data from the file handler
        this.gameData = fileDataHandler.Load();

        //if there is no data to load create a new game instead
        if (this.gameData == null)
        {
            if (createDataIfNull)
            {
                //this will fail to save if there is no game data, rather than creating new data
                //this is just in case some error causes easy ways to wipe someones file
                Debug.Log("No game data found, creating new data");
                NewGame();
            }
            else
            {
                //this will fail to save if there is no game data, rather than creating new data
                //this is just in case some error causes easy ways to wipe someones file
                Debug.Log("No game data found, A new game needs to be started in order to load");
                return;
            }
            
        }

        foreach (IDataPersistence objs in persitenceObjs)
        {
            objs.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        //if there is no game data to save to just return in order to stop further errors
        if (gameData == null)
        {
            Debug.Log("No game data found a new game needs to be created in order to save");
            return;
        }

        foreach (IDataPersistence objs in persitenceObjs)
        {
            objs.SaveData(gameData);
        }

        //save the game data to the file handler
        fileDataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        //save the game if the game is closed
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        //find all data persistence objects in the game on load
        IEnumerable<IDataPersistence> persitenceObjs = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDataPersistence>();   
        return new List<IDataPersistence>(persitenceObjs);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        //when loading a scene get all the new data persistence objects and load the data to them
        this.persitenceObjs = FindAllDataPersistenceObjects();
        LoadGame();
    }


    public bool HasGameData()
    {
        return gameData != null;
    }
}

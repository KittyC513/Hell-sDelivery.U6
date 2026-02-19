using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SoundBankManager : MonoBehaviour
{
    public static SoundBankManager soundBankManager;

    [SerializeField] private SoundBank[] soundBanks;
    private Dictionary<string, SoundBank> soundBankIndex;
    public Dictionary<string, SoundBank> loadedSoundBanks;

    //need a reference to loaded sound banks

    private void Awake()
    {
        if (soundBankManager != null && soundBankManager != this)
        {
            Destroy(this);
        }
        
        soundBankManager = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        InitalizeSoundBanks();
    }

    private void InitalizeSoundBanks()
    {
        soundBankIndex = new Dictionary<string, SoundBank>();
        loadedSoundBanks = new Dictionary<string, SoundBank>();

        //run the initialize function on the sound banks
        for (int i = 0; i < soundBanks.Length; i++)
        {
            soundBankIndex.Add(soundBanks[i].bankName, soundBanks[i]);
            soundBanks[i].InitializeBank();
        }
    }

    public void LoadSoundBank(string bankName)
    {
        //check if the bank exists
        if (soundBankIndex.ContainsKey(bankName))
        {
            //check if the sound bank is already loaded
            if (!loadedSoundBanks.ContainsKey(bankName))
            {
                //get the bank from the dictionary using the name
                SoundBank bank = soundBankIndex[bankName];

                //add the bank to the list of loaded sound banks
                loadedSoundBanks.Add(bankName, bank);

                //for each sound in the bank load it into memory so it can be played instantly
                for (int i = 0; i < bank.sounds.Count; i++)
                {
                    for (int j = 0; j < bank.sounds[i].audioClips.Count; j++)
                    {
                        bank.sounds[i].audioClips[j].LoadAudioData();
                    }
                }
            }
            else
            {
                Debug.Log("(Soundbank already loaded) A Sound Bank named: " + bankName + " is already loaded");
            }
          
        }
        else
        {
            Debug.LogError("(LoadSoundBankError) A Sound Bank Named: " + bankName + " doesn't exist");
        }
    }


    public void UnloadSoundBank(string bankName)
    {
        //check if the bank is loaded
        if (loadedSoundBanks.ContainsKey(bankName))
        {
            //get the bank from the dictionary using the name
            SoundBank bank = loadedSoundBanks[bankName];

            //remove the sound bank from the loaded list
            loadedSoundBanks.Remove(bankName);

            //for each sound in the bank unload it from memory
            for (int i = 0; i < bank.sounds.Count; i++)
            {
                for (int j = 0; j < bank.sounds[i].audioClips.Count; j++)
                {
                    bank.sounds[i].audioClips[j].UnloadAudioData();
                }
            }
        }
        else
        {
            Debug.LogError("(UnloadSoundBankError) A Sound Bank Named: " + bankName + " was not loaded in the first place");
        }
    }


    public AudioClip FetchAudioClip(string bankName, string soundName)
    {
        if (loadedSoundBanks.ContainsKey(bankName))
        {
            //check if the sound bank contains the sound
            if (loadedSoundBanks[bankName].soundIndex.ContainsKey(soundName))
            {
                SoundEffect sfx = loadedSoundBanks[bankName].soundIndex[soundName];

                //if the sound contains more than one audio clip choose between them at random
                if (sfx.audioClips.Count > 1)
                {
                    int num = Random.Range(0, sfx.audioClips.Count);
                    return sfx.audioClips[num];
                }
                else
                {
                    return sfx.audioClips[0];
                }
                
            }
            else
            {
                Debug.LogError("Audio clip is null, naming might be incorrect");
                return null;
            }
        }
        Debug.LogError("SoundBank is null, it may not be loaded");
        return null;
    }

    public SoundEffect FetchSoundEffect(string bankName, string soundName)
    {
          if (loadedSoundBanks.ContainsKey(bankName))
        {
            //check if the sound bank contains the sound
            if (loadedSoundBanks[bankName].soundIndex.ContainsKey(soundName))
            {
                //grab the sound effect
                SoundEffect sfx = loadedSoundBanks[bankName].soundIndex[soundName];

                //if the sound contains more than one audio clip choose between them at random
                if (sfx.audioClips.Count > 1)
                {
                    int num = Random.Range(0, sfx.audioClips.Count);
                    sfx.chosenSound = num;
                }
                else
                {
                    sfx.chosenSound = 0;
                }

                //return the sound effect
                return sfx;
            }
            else
            {
                Debug.LogError("Audio clip is null, naming might be incorrect");
                return null;
            }
        }
        Debug.LogError("SoundBank is null, it may not be loaded");
        return null;
    }
}



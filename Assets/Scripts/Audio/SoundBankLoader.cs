using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundBankLoader : MonoBehaviour
{
    [SerializeField] private SoundBank[] banksToLoad;

    public void Start()
    {
        //load all the sound banks
        for (int i = 0; i < banksToLoad.Length; i++)
        {
            SoundBankManager.soundBankManager.LoadSoundBank(banksToLoad[i].bankName);
        }
        
    }

    public void OnDisable()
    {
        for (int i = 0; i < banksToLoad.Length; i++)
        {
            SoundBankManager.soundBankManager.UnloadSoundBank(banksToLoad[i].bankName);
        }
        
    }
}

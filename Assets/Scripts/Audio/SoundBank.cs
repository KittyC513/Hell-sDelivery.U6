using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundBank", menuName = "Scriptable Objects/SoundBank")]
public class SoundBank : ScriptableObject
{
    [SerializeField] public string bankName;
    [SerializeField] public List<SoundEffect> sounds;

    public Dictionary<string, SoundEffect> soundIndex;

    //called to setup a dictionary that makes it easier to fetch sound effects
    public void InitializeBank()
    {
        soundIndex = new Dictionary<string, SoundEffect>();

        for (int i = 0; i < sounds.Count; i++)
        {
            soundIndex.Add(sounds[i].clipName, sounds[i]);
        }
    }
}

[System.Serializable]
public class SoundEffect
{
    [SerializeField] public string clipName;
    [SerializeField] public List<AudioClip> audioClips;
}

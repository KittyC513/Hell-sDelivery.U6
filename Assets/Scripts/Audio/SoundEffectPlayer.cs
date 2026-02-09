using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    private SoundBankManager soundBankManager;
    //[SerializeField] public AudioSource audioSource;

    [SerializeField] public GameObject audioSourcePrefab;
    [SerializeField] private int sourceStartSize;
    private List<AudioSource> audioSources;

    private void Start()
    {
        audioSources = new List<AudioSource>();

        for (int i = 0; i < sourceStartSize; i++)
        {
            audioSources.Add(Instantiate(audioSourcePrefab, this.transform.position, Quaternion.identity, this.transform).GetComponent<AudioSource>());
        }
    }

    private AudioSource GetOrAddAudioSource()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            //audio source is not playing and can be grabbed
            if (audioSources[i].isPlaying == false)
            {
                return audioSources[i];
            }
        }

        //we made it through the entire loop and did not find a free audio source, we need to create a new one
        GameObject temp = Instantiate(audioSourcePrefab, this.transform.position, Quaternion.identity, this.transform);
        audioSources.Add(temp.GetComponent<AudioSource>());
        return temp.GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(string bankName, string soundEffectName)
    {
        SoundEffect soundEffect = SoundBankManager.soundBankManager.FetchSoundEffect(bankName, soundEffectName);

        if (soundEffect != null)
        {
            ApplySoundEffectParameters(soundEffect, GetOrAddAudioSource());
        }
    }

    public void StopAllSoundEffects()
    {
        StopAllCoroutines();
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].Stop();
        }
    }

    public void StopSoundEffect()
    {
        //audioSource.Stop();
    }

    public void StopAudioDelayed(float delay)
    {
        StartCoroutine(DelayedSoundStop(delay));
    }

    public void ApplySoundEffectParameters(SoundEffect soundEffect, AudioSource audioSource)
    {
        float rndPitch = Random.Range(1 - soundEffect.randomPitchBend, 1 + soundEffect.randomPitchBend);

        audioSource.pitch = rndPitch;

        audioSource.loop = soundEffect.loop;

        audioSource.volume = soundEffect.volumeAdjust;

        //Debug.Log(audioSource);

        if (audioSource.loop)
        {
            audioSource.clip = soundEffect.audioClips[soundEffect.chosenSound];
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(soundEffect.audioClips[soundEffect.chosenSound]);
        }

        
        //audioSource.pitch = 1;
    }

    public void QueueSoundEffect(string bankName, string soundEffectName, float delay)
    {
        SoundEffect soundEffect = SoundBankManager.soundBankManager.FetchSoundEffect(bankName, soundEffectName);

        if (soundEffect != null)
        {
            StartCoroutine(DelayedSoundEffect(delay, soundEffect));
        }
    }
    private IEnumerator DelayedSoundEffect(float delayTime, SoundEffect soundEffect)
    {
        yield return new WaitForSeconds(delayTime);

        //yield return new WaitUntil(() => audioSource.isPlaying == false);

        ApplySoundEffectParameters(soundEffect, GetOrAddAudioSource());
    }

    private IEnumerator DelayedSoundStop(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}

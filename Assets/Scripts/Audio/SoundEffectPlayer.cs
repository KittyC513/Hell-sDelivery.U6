using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    private SoundBankManager soundBankManager;
    [SerializeField] public AudioSource audioSource;

    public void PlaySoundEffect(string bankName, string soundEffectName)
    {
        SoundEffect soundEffect = SoundBankManager.soundBankManager.FetchSoundEffect(bankName, soundEffectName);

        if (soundEffect != null)
        {
            ApplySoundEffectParameters(soundEffect);
        }
    }

    public void StopAllSoundEffects()
    {
        StopAllCoroutines();
        audioSource.Stop();
    }

    public void StopSoundEffect()
    {
        audioSource.Stop();
    }

    public void StopAudioDelayed(float delay)
    {
        StartCoroutine(DelayedSoundStop(delay));
    }

    public void ApplySoundEffectParameters(SoundEffect soundEffect)
    {
        float rndPitch = Random.Range(1 - soundEffect.randomPitchBend, 1 + soundEffect.randomPitchBend);

        audioSource.pitch = rndPitch;

        audioSource.loop = soundEffect.loop;

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

        ApplySoundEffectParameters(soundEffect);
    }

    private IEnumerator DelayedSoundStop(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}

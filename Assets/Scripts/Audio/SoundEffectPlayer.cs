using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    private SoundBankManager soundBankManager;
    [SerializeField] public AudioSource audioSource;

    public void PlaySoundEffect(string bankName, string soundEffectName)
    {
        AudioClip clip = SoundBankManager.soundBankManager.FetchAudioClip(bankName, soundEffectName);

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void Update()
    {
        //debug only
        if (Input.GetMouseButtonDown(0))
        {
            //PlaySoundEffect("Player", "Jump");
        }
    }
}

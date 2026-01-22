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
            float rndPitch = Random.Range(1 - soundEffect.randomPitchBend, 1 + soundEffect.randomPitchBend);

            audioSource.pitch = rndPitch;

            audioSource.PlayOneShot(soundEffect.audioClips[soundEffect.chosenSound]);
            //audioSource.pitch = 1;
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

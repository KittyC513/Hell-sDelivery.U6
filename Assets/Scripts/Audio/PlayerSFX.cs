using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public SoundEffectPlayer sfxPlayer;

    public void PlayShminkJump()
    {
        sfxPlayer.PlaySoundEffect("Player", "ShminkJump");
    }

    public void PlayShmonkJump()
    {
        sfxPlayer.PlaySoundEffect("Player", "ShmonkJump");
    }

    public void PlayStep()
    {
        sfxPlayer.PlaySoundEffect("Player", "Step");
    }

    public void PlayLand()
    {
        sfxPlayer.PlaySoundEffect("Player", "Land");
    }

    public void PlayHurt()
    {
        
    }
}

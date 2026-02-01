using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public SoundEffectPlayer sfxPlayer;

    public void PlayShimnkDeath()
    {
        sfxPlayer.PlaySoundEffect("Player", "ShminkDeath");
    }

    public void PlayShmonkDeath()
    {
        sfxPlayer.PlaySoundEffect("Player", "ShmonkDeath");
    }
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
        Debug.Log("Play Step");
        sfxPlayer.PlaySoundEffect("Player", "Step");
    }

    public void PlayLand()
    {
        sfxPlayer.PlaySoundEffect("Player", "Land");
    }

    public void PlaySpinAttack()
    {
        sfxPlayer.PlaySoundEffect("Player", "SpinAttack");
    }

    public void PlayHurt()
    {
        
    }

    public void PlayItemPickup()
    {
        sfxPlayer.PlaySoundEffect("Player", "ItemPickup");
    }

    public void PlayItemThrow()
    {
        sfxPlayer.PlaySoundEffect("Player", "ItemThrow");
    }
}

using UnityEngine;

public class PlayFootstep : MonoBehaviour
{
    private PlayerSFX playerSFX;
    [SerializeField] private ParticleSystem pSystem;

    private void Start()
    {
        //temporary easy way to find the player sfx object
        playerSFX = transform.parent.transform.parent.GetComponentInChildren<PlayerSFX>();
    }

    public void PlayStep()
    {
        if (playerSFX != null) playerSFX.PlayStep();
        else 
        {
            //attempt to grab the player sfx object again
            playerSFX = transform.parent.transform.parent.GetComponentInChildren<PlayerSFX>();
            if (playerSFX != null) playerSFX.PlayStep();
        }

    }

    public void SpawnStepParticle()
    {
        if (pSystem != null) pSystem.Play();
    }
}

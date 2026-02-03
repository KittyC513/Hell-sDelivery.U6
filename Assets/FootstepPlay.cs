using UnityEngine;

public class FootstepPlay : MonoBehaviour
{
    [SerializeField] private PlayerSFX playerSFX;
    public void PlayFootstep()
    {
        playerSFX.PlayStep();
    }
}

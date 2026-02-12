using UnityEngine;
using UnityEngine.SceneManagement;

public class ParticleSystemSelfDestroy : MonoBehaviour
{
    private ParticleSystem self;
    private bool started = false;

    private void Awake()
    {
        self = GetComponent<ParticleSystem>();
        
    }

    private void Update()
    {
        if (started)
        {
            if (!self.isPlaying) Destroy(this);
        }
        else
        {
            if (self.isPlaying) started = true;
        }
    }


}

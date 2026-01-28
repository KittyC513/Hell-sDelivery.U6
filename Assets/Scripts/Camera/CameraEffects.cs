using UnityEditor;
using UnityEngine;
using Febucci.UI;

public class CameraEffects : MonoBehaviour
{
    private Animator _cameraAnimator;
    private static readonly int _shakeEffect = Animator.StringToHash("ShakeEffect");
    [SerializeField] TextAnimator_TMP textAnimator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShakeCamera()
    {
        _cameraAnimator.enabled = true;
        _cameraAnimator.SetTrigger(_shakeEffect);
    }
}

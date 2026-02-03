using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class UnlockableCar : CarjackCar
{
    private UnlockSliderManager sliderManager;
    public UnityEvent onCarUnlock;
    public override void Start()
    {
        sliderManager = FindFirstObjectByType<UnlockSliderManager>();
        p1Slider = sliderManager.p1Slider;
        p2Slider = sliderManager.p2Slider;
    }

    //called when the car is unlocked
    public override void UnlockCar(PlayerInputDetection player)
    {
        unlocked = true;
        //do something here
        onCarUnlock.Invoke();
    }
}

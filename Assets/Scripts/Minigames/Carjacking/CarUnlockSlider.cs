using UnityEngine;
using UnityEngine.UI;

public class CarUnlockSlider : MonoBehaviour
{
    [SerializeField] public RectTransform slider;
    [SerializeField] public RectTransform handle;
    [SerializeField] public RectTransform hitArea;

    private float sliderProgression = 0;
    [SerializeField] private float sliderSpeed = 1;


    private float startValue;
    private float endValue; 
    private int dir;

    private void Start()
    {
        slider.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void Update()
    {
        //find the start and end of the slider 
        startValue = slider.position.x - (slider.rect.width / 2);
        endValue = slider.position.x + (slider.rect.width / 2);
        
        if (Input.GetKey(KeyCode.N))
        {
            PickNewSection(Random.Range(0.75f, 2.5f));
        }

        //set the handle position to somewhere between the start and end
        handle.position = new Vector3(Mathf.Lerp(startValue, endValue, sliderProgression / 10), handle.position.y, handle.position.z);

        //progress the slider
        sliderProgression += (sliderSpeed * dir) * Time.deltaTime;

        //reverse the slider if it goes to the end and start
        if (sliderProgression >= 10)
        {
            sliderProgression = 10;
            dir = -1;
        }
        else if (sliderProgression <= 0)
        {
            sliderProgression = 0;
            dir = 1;
        }
    }

    private void PickNewSection(float xSize)
    {
        //pick a section on the slider between start and end value but with a buffer of half the Xsize
        //pick a random percentage
        hitArea.sizeDelta = new Vector2(xSize, hitArea.rect.height);

        float randomPercent = Random.Range(0, 1f);
        Vector3 startPos = new Vector3(Mathf.Lerp(startValue, endValue, randomPercent), slider.position.y, slider.position.z);
        
        //check if the target area will go over the slider values
        if (startPos.x + (xSize / 2) > endValue)
        {
            startPos.x -= (startPos.x + (xSize / 2)) - endValue;
        }
        else if (startPos.x - (xSize / 2) < startValue)
        {
            startPos.x += startValue - (startPos.x - (xSize / 2));
        }
        
        hitArea.position = startPos;
       
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarjackMinigameManager : MonoBehaviour
{
    
    [SerializeField] private GameObject carPrefab;
    //the space where cars should be placed
    [SerializeField] private Vector2 screenSize;
    //the amount of cars to be placed x is horizontal and y is vertical
    [SerializeField] private Vector2 carCount;

    [SerializeField] private Transform startPos;

    [SerializeField] public CarUnlockSlider p1Slider;
    [SerializeField] public CarUnlockSlider p2Slider;
    private List<GameObject> cars;
    [SerializeField] private float hotDist = 12;
    [SerializeField] private float warmDist = 22;
    [SerializeField] private float coolDist = 29;
    private GameObject targetCar;
    [SerializeField] private TextMeshProUGUI text;
    

    public enum HeatCheck { cold, cool, warm, hot, target }

    private void Start()
    {
        cars = new List<GameObject>();

        //spawn cars across the x
        for (int i = 0; i < carCount.x; i++)
        {
            float percent = (screenSize.x / (carCount.x - 1));
            Vector3 position = new Vector3(startPos.position.x + (i * percent), startPos.position.y, startPos.position.z);
            cars.Add(Instantiate(carPrefab, position, Quaternion.identity));


            //for each x space spawn cars up and down
            for (int y = 1; y < carCount.y; y++)
            {
                float percentY = (screenSize.y / (carCount.y - 1));
                Vector3 positionY = new Vector3(startPos.position.x + (i * percent), startPos.position.y, (startPos.position.z + y * percentY));

                cars.Add(Instantiate(carPrefab, positionY, Quaternion.identity));
            }
        }

        SelectCar(cars.Count, cars);

        StartCoroutine(MinigameStartSequence());
        //GameManager.Instance.DisableBothPlayersCam();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(startPos.position + new Vector3(screenSize.x / 2, 0, screenSize.y / 2), new Vector3(screenSize.x, 1, screenSize.y));
    }

    private void SelectCar(int listLength, List<GameObject> carList)
    {
        int num = Random.Range(0, listLength);

        GameObject chosenCar = carList[num];
        targetCar = chosenCar;
        chosenCar.GetComponentInChildren<MeshRenderer>().material = null;
    }

    public HeatCheck CheckDistance(Vector3 position, PlayerInputDetection player)
    {
        float distance = Vector3.Distance(position, targetCar.transform.position);
        Debug.Log(distance);
        if (distance <= 0)
        {
            //target car has been opened
            int winningPlayer = player.playerNum;
            StartCoroutine(MinigameEndSequence(winningPlayer));

            return HeatCheck.target;
        }
        if (distance < hotDist)
        {
            return HeatCheck.hot;
        }
        else if (distance < warmDist)
        {
            return HeatCheck.warm;
        }
        else if (distance < coolDist)
        {
            return HeatCheck.cool;
        }
        else
        {
            return HeatCheck.cold;
        }
    }

    private IEnumerator MinigameEndSequence(int p)
    {
        GameManager.instance.FreezeBothPlayers();

        yield return new WaitForSeconds(0.15f);
        text.alpha = 255;
        text.text = "Player " + p + " Wins!";
          text.gameObject.SetActive(true);
        text.GetComponent<Animator>().Play("Default");
        yield return new WaitForSeconds(1);
        GameManager.instance.UnFreezeBothPlayers();
        SceneManager.LoadScene("Playtest-11-2025");

        //do something to end the minigame
    }

    private IEnumerator MinigameStartSequence()
    {

        GameManager.instance.FreezeBothPlayers();
        yield return new WaitForSeconds(0.15f);
        text.text = "Find The Item";
        text.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1);
        text.color = Color.yellow;
        text.text = "3";

        yield return new WaitForSeconds(1);
        text.color = Color.blue;
        text.text = "2";

        yield return new WaitForSeconds(1);
        text.color = Color.red;
        text.text = "1";

        yield return new WaitForSeconds(1);
        text.color = Color.green;
        text.text = "Go!";
        text.GetComponent<Animator>().Play("MinigameTextGo");

        yield return new WaitForSeconds(0.35f);
        text.gameObject.SetActive(false);
        text.GetComponent<Animator>().Play("Default");
        GameManager.instance.UnFreezeBothPlayers();
    }
}

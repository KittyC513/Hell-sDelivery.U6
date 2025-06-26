using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public int curHealth;
    public int maxHealth;

    public Image[] hearts;
    public Color red;
    public Color white;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        red = Color.red;
        white = Color.white;
        

        if (this.CompareTag("P1"))
        {
            maxHealth = GameManager.instance.health_p1.maxHealth;
        }

        if (this.CompareTag("P2"))
        {
            maxHealth = GameManager.instance.health_p2.maxHealth;
        }




        curHealth = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {

        if (EventData.isInverseScreen)
        {
            if (this.CompareTag("P2"))
            {
                curHealth = GameManager.instance.health_p1.currentHealth;
            }

            if (this.CompareTag("P1"))
            {
                curHealth = GameManager.instance.health_p2.currentHealth;
            }
        }
        else
        {
            if (this.CompareTag("P1"))
            {
                curHealth = GameManager.instance.health_p1.currentHealth;
            }

            if (this.CompareTag("P2"))
            {
                curHealth = GameManager.instance.health_p2.currentHealth;
            }
        }

        switch (curHealth)
        {
            case 0:
                hearts[0].color = white;
                hearts[1].color = white;
                hearts[2].color = white;
                break;
            case 1:
                hearts[0].color = red;
                hearts[1].color = white;
                hearts[2].color = white;
                break;
            case 2:
                hearts[0].color = red;
                hearts[1].color = red;
                hearts[2].color = white;
                break;
            case 3:
                hearts[0].color = red;
                hearts[1].color = red;
                hearts[2].color = red;
                break;

        }

    }
}

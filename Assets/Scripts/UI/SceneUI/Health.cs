using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int health;
    public int numOfHearts;

    public Image[] hearts;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowHealthHearts();
    }

    private void ShowHealthHearts()
    {
        health = GetComponent<Player>().charCurrentHealth;

        numOfHearts = health;

        if (health > numOfHearts)
        {
            health = numOfHearts;
        }
        for(int i = 0; i < hearts.Length; i++)
        {
            if(i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}

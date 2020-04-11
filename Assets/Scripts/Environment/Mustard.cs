using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mustard : Collectables
{
    [SerializeField] protected UnityEngine.UI.Text myText;
    int score;

    void Start()
    {
        collectableSprite = GetComponent<SpriteRenderer>(); 
        score = System.Int32.Parse(myText.text);
    }

    // Update is called once per frame
    void Update()
    {
        if (Collected())
        {
            UpdateScore();
            Suicide();
        }
    } 

    void UpdateScore()
    {
        ++score;
        myText.text = score.ToString();
    }
}

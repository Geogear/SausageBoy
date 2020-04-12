using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mustard : Collectables
{
    protected UnityEngine.UI.Text myText;
    int score;

    void Start()
    {
        myText = FindObjectOfType<UnityEngine.UI.Text>();
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
        score = System.Int32.Parse(myText.text);
        ++score;
        myText.text = score.ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(collectionPoint.position, collectionPointRange);
    }

}

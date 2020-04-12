using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Collectables 
{
    [SerializeField] float oscillationDistance = 0.5f;
    [SerializeField] float moveSpeed = 1.0f;
    float baseYPos;
    bool directionUp = true;

    void Start()
    {
        collectableSprite = GetComponent<SpriteRenderer>();
        baseYPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Collected())
        {
            Suicide();
        }
        Oscillate();
        ChangeDirection();
    }

    new protected bool Collected()
    {
        Collider2D[] hitPlayer;
        // create a circle in enemyAttackPoint position which has a radius size is equal to enemyAttackRange and last parameter represents what kind of layer is touched
        hitPlayer = Physics2D.OverlapCircleAll(collectionPoint.position, collectionPointRange, playerLayer);
        foreach (Collider2D player in hitPlayer)
        { 
            if(player.GetComponent<Player>().charCurrentHealth < 3 && player.GetComponent<Player>().charCurrentHealth > 0)
                ++player.GetComponent<Player>().charCurrentHealth;
            return true;
        }
        return false;
    } 

    public void Oscillate()
    {
        if (directionUp)
        {
            transform.position = new Vector3(transform.position.x, moveSpeed * Time.deltaTime + transform.position.y, transform.position.z);
        }else if(!directionUp)
        {
            transform.position = new Vector3(transform.position.x, -1 * moveSpeed * Time.deltaTime + transform.position.y, transform.position.z);
        }
    } 

    public void ChangeDirection()
    {
        if (transform.position.y >= baseYPos + oscillationDistance)
        {
            directionUp = false;
        }else if(transform.position.y <= baseYPos - oscillationDistance)
        {
            directionUp = true;
        }
    }
}

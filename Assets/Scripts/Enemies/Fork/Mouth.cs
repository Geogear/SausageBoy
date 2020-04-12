using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouth : EnemyRenderer2D
{
    private BoxCollider2D boxCollider;
    [SerializeField] float accelerationModifier = 0.25f;
    void Start()
    {
        enemyState = EnemyState.Chasing;
        enemyIsFacingRight = true;
        enemySprite = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Catched();
        SpeedUp();
        
    } 

    new public void Move()
    {
        transform.position = new Vector3(enemyMoveSpeed * Time.deltaTime + transform.position.x, transform.position.y, transform.position.z);
    } 

    public void Catched()
    {
        Collider2D[] hitPlayer;
        hitPlayer = Physics2D.OverlapCircleAll(enemyAttackPoint.position, enemyAttackRange, playerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<Player>().TakeDamage(100); 
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyAttackPoint.position, enemyAttackRange);
    } 

    protected void SpeedUp()
    { 
        if(enemyMoveSpeed < player.GetComponent<Player>().GetMoveSpeed())
        {
            enemyMoveSpeed += enemyMoveSpeed * Time.deltaTime * accelerationModifier; 
            if(enemyMoveSpeed > player.GetComponent<Player>().GetMoveSpeed())
            {
                enemyMoveSpeed = player.GetComponent<Player>().GetMoveSpeed();
            }
        }

    }
}

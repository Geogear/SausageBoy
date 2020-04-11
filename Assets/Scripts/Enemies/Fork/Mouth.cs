using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouth : EnemyRenderer2D
{
    private BoxCollider2D boxCollider;
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
}

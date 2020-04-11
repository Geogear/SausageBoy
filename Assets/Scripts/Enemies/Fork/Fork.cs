using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fork : EnemyRenderer2D
{
    // Start is called before the first frame update
    void Start()
    {
        enemyAnimationController = GetComponent<Animator>();
        enemyState = EnemyState.Idling;
        enemyIsFacingRight = false;
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemySprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GiveDamage();
        Chase();
        UpdateAnimations();
    }
    private void FixedUpdate()
    {
        SetEnemyState();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyAttackPoint.position, enemyAttackRange);
    }
}

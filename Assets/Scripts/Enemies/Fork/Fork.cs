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
        enemyTimer = GetComponent<Timer>(); 
        player = FindObjectOfType<Player>();
        enemyTimer.addTimer("Chase", 1, 4);
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
        enemyTimer.DecreaseCurrentFrame();
        SetEnemyState(); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyAttackPoint.position, enemyAttackRange);
    }

    new protected void SetEnemyState()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= playerNoticeRange && !IsDead() && !player.IsDead() && enemyTimer.isOnCooldown("Chase") == false)
        {
            ChangeState(EnemyState.Chasing);
            enemyTimer.ResetCooldownFrame("Chase");
            CheckMovementDirection();
        }
        else if (!IsDead() && enemyTimer.isOnCooldown("Chase") == false)
        {
            ChangeState(EnemyState.Idling);
        }
    }

    override protected void Move()
    {
        transform.position = new Vector3(playerDirection * enemyMoveSpeed * Time.deltaTime + transform.position.x, transform.position.y, transform.position.z);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRenderer2D : MonoBehaviour
{
    #region Enums
    public enum EnemyState
    {
        Idling,
        Notice,
        Chasing,
        Dead
    }
    #endregion

    #region Variables
    #region Public Variables
    #endregion
    #region Protected Variables
    [Header("General")]
    [Range(0, 7)]
    [SerializeField] [Tooltip("enemyacter Movement Speed")] protected float enemyMoveSpeed = 0;
    [SerializeField] [Tooltip("enemyacter Maximum Health")] protected float enemyMaxHealth = 0;
    protected SpriteRenderer enemySprite;
    protected Rigidbody2D enemyRigidbody;
    protected Animator enemyAnimationController;
    protected Timer enemyTimer;

    [Space(5)]
    ///STATE CONTROLS
    [Header("State Control Variables")]
    [SerializeField] [Tooltip("enemyacter Ground Check Radius")] protected float enemyCheckRadius = 0;
    [SerializeField] [Tooltip("enemyacter Ground Check Point")] protected Transform enemyGroundCheckPoint = null;
    protected float enemyCurrentHealth;


    [Space(5)]
    ///ATTACK
    [Header("Attack")]
    [SerializeField] [Tooltip("enemyacter Attack Range")] protected float enemyAttackRange = 0;
    [SerializeField] [Tooltip("enemyacter Attack Damage")] protected int enemyAttackDamage = 0;
    [SerializeField] [Tooltip("enemyacter Attack Point")] protected Transform enemyAttackPoint = null;
    [SerializeField] [Tooltip("enemyacter Attack Point")] protected float playerNoticeRange = 0;

    ///STATES
    protected bool enemyIsGrounded;
    protected bool enemyIsFacingRight;

    [Space(5)]
    //Layers
    [Header("Layers")]
    [SerializeField] [Tooltip("Player Layer")] protected LayerMask playerLayer = 0;
    [SerializeField] [Tooltip("Ground Layer")] protected LayerMask groundLayer = 0;

    //Variables of object that can be interacted
    [SerializeField] protected Player player;
    Vector2 targetPos;
    float playerDirection;

    //State Variables
    protected EnemyState enemyState;
    protected EnemyState prevState;


    #endregion
    #endregion

    #region Functions
    #region Public Functions
    #endregion
    #region Protected Functions
    /// <summary>
    /// Changing enemies states
    /// </summary>
    protected virtual void ChangeState(EnemyState enemyState)
    {
        if(this.enemyState != enemyState)
        {
            prevState = this.enemyState;
            this.enemyState = enemyState;
        }
    }

    /// <summary>
    /// Chasing player
    /// </summary>
    protected virtual void Chase()
    {
        if(IsChasing())
        Move();
    }

    protected virtual void Move()
    {
        CheckMovementDirection();
        Vector2 newPos = Vector2.MoveTowards(enemyRigidbody.position, targetPos, enemyMoveSpeed * Time.deltaTime * 2);
        enemyRigidbody.MovePosition(newPos);
    }

    protected virtual void SetEnemyState()
    {
        if(Vector2.Distance(transform.position,player.transform.position) <= playerNoticeRange)
        {
            ChangeState(EnemyState.Chasing);
        }
        else
        {
            ChangeState(EnemyState.Idling);
        }
    }
    protected virtual void Flip()
    {
        enemyIsFacingRight = !enemyIsFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    protected void UpdateAnimations()
    {
        enemyAnimationController.SetBool("isChasing", IsChasing());
        enemyAnimationController.SetBool("isIdling", IsIdling());
        enemyAnimationController.SetBool("isDead", IsDead());
    }

    protected virtual void CheckMovementDirection()
    {
        GetTarget();
        if (playerDirection < 0 && enemyIsFacingRight)
        {
            Flip();
        }
        else if (playerDirection > 0 && !enemyIsFacingRight)
        {
            Flip();
        }
    }
    protected virtual void GetTarget()
    {
        targetPos = new Vector2(player.transform.position.x, enemyRigidbody.position.y);
        playerDirection = (targetPos - enemyRigidbody.position).normalized.x;
    }
    ///<summary>
    /// enemyacter Damage Function 
    ///</summary>
    public virtual void GiveDamage()
    {
        Collider2D[] hitPlayer;
        // create a circle in enemyAttackPoint position which has a radius size is equal to enemyAttackRange and last parameter represents what kind of layer is touched
        hitPlayer = Physics2D.OverlapCircleAll(enemyAttackPoint.position, enemyAttackRange, playerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<Player>().TakeDamage(enemyAttackDamage);
        }
    }

    ///<summary>
    ///enemyacter Taking Damage Function
    ///<param name="damage">Damage Value </param>
    ///</summary>
    public virtual void TakeDamage(int damage)
    {
        //enemyAnimationController.SetTrigger("Hit");
        enemyCurrentHealth -= damage;
        Debug.Log(enemyCurrentHealth);
        if (enemyCurrentHealth <= 0)
        {
            ChangeState(EnemyState.Dead);
            enemyAnimationController.SetTrigger("Die");
            GetComponent<Collider2D>().enabled = false;
        }
    }

    ///<summary>
    /// enemyacter Die Function 
    ///</summary>
    public virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual bool IsChasing()
    {
        return enemyState == EnemyState.Chasing;
    }
    protected virtual bool IsIdling()
    {
        return enemyState == EnemyState.Idling;
    }
    protected virtual bool IsDead()
    {
        return enemyState == EnemyState.Dead;
    }
    protected virtual bool IsNoticed()
    {
        return enemyState == EnemyState.Notice;
    }
    #endregion
    #endregion
}

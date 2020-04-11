using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRenderer2D : MonoBehaviour
{
    #region Enums
    public enum ObstacleState
    {
        Run,
        Stop
    }
    #endregion
    #region Variables
    #region Protected Variables
    protected SpriteRenderer obstacleSprite;
    protected Rigidbody2D obstacleRigidbody;
    protected Animator obstacleAnimationController;
    protected Timer obstacleTimer;

    [Space(5)]
    ///ATTACK
    [Header("Attack")]
    [SerializeField] [Tooltip("enemyacter Attack Range")] protected float obstacleAttackRange = 0;
    [SerializeField] [Tooltip("enemyacter Attack Damage")] protected int obstacleAttackDamage = 0;
    [SerializeField] [Tooltip("enemyacter Attack Point")] protected Transform obstacleAttackPoint = null;

    [Space(5)]
    //Layers
    [Header("Layers")]
    [SerializeField] [Tooltip("Player Layer")] protected LayerMask playerLayer = 0;

    //State Variables
    protected ObstacleState obstacleState;
    protected ObstacleState prevState;
    #endregion
    #endregion

    #region Functions
    #region Public Functions
    #endregion
    #region Protected Functions
    /// <summary>
    /// Changing enemies states
    /// </summary>
    protected virtual void ChangeState(ObstacleState obstacleState)
    {
        if (this.obstacleState != obstacleState)
        {
            prevState = this.obstacleState;
            this.obstacleState = obstacleState;
        }
    }

    protected void UpdateAnimations()
    {
        obstacleAnimationController.SetBool("isRunning", IsRunning());
        obstacleAnimationController.SetBool("isStopped", IsStopped());
    }

    public virtual void GiveDamage()
    {
        if (IsRunning())
        {
            Collider2D[] hitPlayer;
            // create a circle in enemyAttackPoint position which has a radius size is equal to enemyAttackRange and last parameter represents what kind of layer is touched
            hitPlayer = Physics2D.OverlapCircleAll(obstacleAttackPoint.position, obstacleAttackRange, playerLayer);
            foreach (Collider2D player in hitPlayer)
            {
                player.GetComponent<Player>().TakeDamage(obstacleAttackDamage);
            }
            obstacleTimer.ResetCooldownFrame("Electric");
        }
    }

    protected virtual void SetObstacleState()
    {

        if (obstacleTimer.isOnCooldown("Electric") == false )
        {
            obstacleState = ObstacleState.Run;    
        }
    }

    protected virtual bool IsRunning()
    {
        return obstacleState == ObstacleState.Run;
    }
    protected virtual bool IsStopped()
    {
        return obstacleState == ObstacleState.Stop;
    }
    #endregion
    #endregion
}

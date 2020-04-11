using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint2D))]
public class KitchenKnife : ObstacleRenderer2D
{
    #region SerializeField Variables
    [SerializeField] float leftPushRange = 0;
    [SerializeField] float rightPushRange = 0;
    [SerializeField] float velocityThreshold = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        obstacleRigidbody = GetComponent<Rigidbody2D>();
        obstacleRigidbody.angularVelocity = velocityThreshold;
        obstacleState = ObstacleState.Run;
    }

    // Update is called once per frame
    void Update()
    {
        GiveDamage();
        Push();
    }

    public void Push()
    {
        if(transform.rotation.z > 0 && transform.rotation.z < rightPushRange && (obstacleRigidbody.angularVelocity > 0) && obstacleRigidbody.angularVelocity < velocityThreshold)
        {
            obstacleRigidbody.angularVelocity = velocityThreshold;
        }
        else if(transform.rotation.z <  0 && transform.rotation.z > leftPushRange && (obstacleRigidbody.angularVelocity < 0) && obstacleRigidbody.angularVelocity > velocityThreshold * -1)
        {
            obstacleRigidbody.angularVelocity = velocityThreshold * -1;
        }
    }


    public override void GiveDamage()
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
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(obstacleAttackPoint.position, obstacleAttackRange);
    }
}

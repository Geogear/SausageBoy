using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricSocket : ObstacleRenderer2D
{
    // Start is called before the first frame update
    void Start()
    {
        obstacleSprite = GetComponent<SpriteRenderer>();
        obstacleRigidbody = GetComponent<Rigidbody2D>();
        obstacleAnimationController = GetComponent<Animator>();
        obstacleState = ObstacleState.Run;
        obstacleTimer = GetComponent<Timer>();
        obstacleTimer.addTimer("Electric", 5, 1);
}

    // Update is called once per frame
    void Update()
    {
        GiveDamage();
        UpdateAnimations();
    }
    private void FixedUpdate()
    {
        obstacleTimer.DecreaseCurrentFrame();
        SetObstacleState();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(obstacleAttackPoint.position, obstacleAttackRange);
    }
}

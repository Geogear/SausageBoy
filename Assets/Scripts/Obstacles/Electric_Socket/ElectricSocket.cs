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
}

    // Update is called once per frame
    void Update()
    {
        GiveDamage();
        UpdateAnimations();
    }
    private void FixedUpdate()
    {
        SetObstacleState();
    }
}

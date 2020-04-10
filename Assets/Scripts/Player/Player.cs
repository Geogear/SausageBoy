using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CharacterController2D))]
public class Player : CharacterRenderer2D
{
    [Space(10)]
    [Header("Player Variables")]
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float timeToJumpApex = .4f;
    Animator myAnimator; // animator component
    private float move; // Movement input variable range in [-1,1]

    //CharacterController2D
    CharacterController2D charController;
    Vector3 velocity;
    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    private int extraJumps; // Amount of jump
    [SerializeField] private int extraJumpsValue = 0;
    [SerializeField] private float fallMultiplier = 2.5f; // to make an advanced jump

    //TIMER
    float nextAttackTime = 0f;
    public int noOfClicks = 1; // amount of clicks ranged in [0,3] since player has 3 attack animations
    float lastClickedTime = 0;
    public float maxComboDelay = 0.9f; // checking step by step click not spawning
    public float maxComboDelayAnimation = 0.53f; // animation delay

    public CharacterState PlayerState
    {
        get
        {
            return charState;
        }
        set
        {
            charState = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController2D>();
        collidingAgainst = CollidedAreas.Ground;
        ChangeState(CharacterState.inIdling);
        charCurrentHealth = charMaxHealth;
        charIsFacingRight = true;
        extraJumps = extraJumpsValue;
        myAnimator = GetComponent<Animator>();
        charSprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        gravity = -(2 * jumpHeight) / (Mathf.Pow(timeToJumpApex, 2));
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        if (charController.collisions.above || charController.collisions.below)
        {
            velocity.y = 0;
        }
        Move();
        SetCharacterState();
        CheckMovementDirection();
        Jump();
        Attack();
        RunAnimations();
    }
    private void FixedUpdate()
    {
        if (IsIdling())
        {
            velocity.x = 0;
        }
        IsOnGround();
        CheckSurroundings();
        AdvancedJump();
    }
    protected override void Move()
    {
        velocity.y += gravity * Time.deltaTime;
        move = Input.GetAxisRaw("Horizontal");
        float targetVelocityX = move * charMoveSpeed;
        if (IsRunning() || IsJumping() || IsFalling())
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (charController.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        }

        charController.Move(velocity * Time.deltaTime);
    }
    void Jump()
    {
        // check if is grounded or wall sliding, if so reset extrajump value
        if (charIsGrounded)
        {
            extraJumps = extraJumpsValue;
        }
        if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {
            velocity = Vector2.up * jumpVelocity;
            charController.Move(velocity * Time.deltaTime);
            myAnimator.SetTrigger("Jump");
            extraJumps--;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && charIsGrounded == true)
        {
            velocity = Vector2.up * jumpVelocity;
            charController.Move(velocity * Time.deltaTime);
            myAnimator.SetTrigger("Jump");
        }
    }
    void RunAnimations()
    {
        myAnimator.SetBool("isAttacking", IsAttacking());
        myAnimator.SetBool("isFalling", IsFalling());
        myAnimator.SetBool("isIdling", IsIdling());
        myAnimator.SetBool("isRunning", IsRunning());
        myAnimator.SetBool("isGrounded", charIsGrounded);
    }
    protected override void Flip()
    {
         // flip player
        charIsFacingRight = !charIsFacingRight;
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }
    public override void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            // when click time pass, attack animation resets for example when you click the mouse after that you will see Attack animation 1 and so on
            if (Time.time - lastClickedTime > maxComboDelay && (IsAttacking()))
            {
                if (charIsGrounded)
                {
                    ChangeState(CharacterState.inIdling);
                }
                else
                {
                    ChangeState(CharacterState.onFalling);
                }

                //canMove = true;
            }
            if (Time.time - lastClickedTime > maxComboDelay * 6)
            {
                noOfClicks = 1;
            }
            // check if left mouse button is clicked (0) for left (1) for right

            if (IsAttacking())
            {
                // get current time
                // increase amount of clicks           
                // check if player touchs ground, if so rigidbody velocity is made zero for stopping 
                if (IsAttacking())
                {
                    //Normal Attack
                    if (IsAttacking())
                    {
                        if (noOfClicks == 1)
                        {
                            myAnimator.SetTrigger("Attack1");
                        }
                        else if (noOfClicks == 2)
                        {
                            myAnimator.SetTrigger("Attack2");
                        }
                        else if (noOfClicks == 3)
                        {
                            myAnimator.SetTrigger("Attack3");
                        }
                    }
                }
                // next click time should be that attack rate which is specified by ourselves.
                nextAttackTime = Time.time + 1f / charAttackRate;

            }
        }
    }

    public override void TakeDamage(int damage)
    {
        myAnimator.SetTrigger("Hit");
        charCurrentHealth -= damage;
        if (charCurrentHealth <= 0)
        {
            myAnimator.SetTrigger("Die");
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
        }
    }

    // Attack animation event to check whether enemy take damage or not.
    public override void GiveDamage()
    {
        Collider2D[] hitEnemies;
        // create a circle in charAttackPoint position which has a radius size is equal to charAttackRange and last parameter represents what kind of layer is touched
        hitEnemies = Physics2D.OverlapCircleAll(charAttackPoint.position, charAttackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            //enemy.GetComponent<Enemy>().TakeDamage(charAttackDamage);
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    // check movement direction , move range in [-1,1]
    protected override void CheckMovementDirection()
    {
        if (charIsFacingRight && move < 0)
        {
            Flip();
        }
        else if (!charIsFacingRight && move > 0)
        {
            Flip();
        }
    }

    private void IsOnGround()
    {
        if (charIsGrounded && !IsGrounded())
        {
            collidingAgainst = CollidedAreas.Ground;
        }
    }
    protected override void CheckSurroundings()
    {
        charIsGrounded = Physics2D.OverlapCircle(charGroundCheckPoint.position, charCheckRadius, groundLayer);
        // creating a raycast form wallcheck position to right in wallcheckdistance size and last parameter represents what kind of layer is touched

    }


    void AdvancedJump()
    {
        //if going down then speed up the falling
        if (velocity.y < 0  && !charIsGrounded)
        {
            velocity += (Vector3)(Vector2.up * gravity * (fallMultiplier - 1) * Time.deltaTime);
        }
    }

    protected override void ChangeState(CharacterState charState)
    {
        this.charState = charState;
    }

    protected override void SetCharacterState()
    {
        //Setting whether character is running or is idling
        if (!IsAttacking())
        {
            if (move != 0 && charIsGrounded && !IsJumping()) //&& canMove)
            {
                ChangeState(CharacterState.onRunning);
            }
            else if (charIsGrounded && !IsJumping() & !IsIdling())
            {
                ChangeState(CharacterState.inIdling);
                velocity.x = 0;
            }
        }
        //
        if (!charIsGrounded && velocity.y > 0)
        {
            ChangeState(CharacterState.onJumping);
        }
        //
        if (velocity.y < 0 && !charIsGrounded)
        {
            ChangeState(CharacterState.onFalling);
        }
        if (Input.GetMouseButtonDown(0))
        {
            //noOfClicks++;
            lastClickedTime = Time.time;
            noOfClicks = Mathf.Clamp(noOfClicks, 1, 3);
            if (charIsGrounded)
            {
                ChangeState(CharacterState.inAttacking);
            }
        }
    }

}


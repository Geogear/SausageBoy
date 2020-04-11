using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterRenderer2D
{
    [Space(10)]
    [Header("Player Variables")]
    [SerializeField] private float jumpHeight = 4f;
    private Vector2 jumpVector;
    Animator myAnimator; // animator component
    private float move; // Movement input variable range in [-1,1]
    CharacterState prevState; 


    private int extraJumps; // Amount of jump
    [SerializeField] private int extraJumpsValue = 0;
    [SerializeField] private float fallMultiplier = 2.5f; // to make an advanced jump

    //TIMER
    float nextAttackTime = 0f;
    float lastClickedTime = 0;
    public float maxComboDelay = 0.9f; // checking step by step click not spawning
    public float maxComboDelayAnimation = 0.53f; // animation delay  

    //Materials For Flashing When Taken Damage 
    [SerializeField] private Material matWhite = null;
    private Material matDefault;

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
        charRigidbody = GetComponent<Rigidbody2D>();
        collidingAgainst = CollidedAreas.Ground;
        ChangeState(CharacterState.inIdling);
        charCurrentHealth = charMaxHealth;
        charIsFacingRight = true;
        extraJumps = extraJumpsValue;
        myAnimator = GetComponent<Animator>();
        charSprite = GetComponent<SpriteRenderer>();
        charTimer = GetComponent<Timer>();
        charTimer.addTimer("NoHit" , 10, 3);
        matDefault = charSprite.material;
    }

    private void Update()
    {
        Move();
        CheckMovementDirection();
        Jump();
        Attack();
        RunAnimations();
        SetCharacterState();
        PlayEffects();
    }
    private void FixedUpdate()
    {
        AdvancedJump();
        IsOnGround();
        CheckSurroundings();
        charTimer.DecreaseCurrentFrame();
    }
    protected override void Move()
    {
        if (!IsDead())
        {
            move = Input.GetAxisRaw("Horizontal");
            charRigidbody.velocity = new Vector2(move * charMoveSpeed, charRigidbody.velocity.y);
        }
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
            jumpVector = Vector2.up * jumpHeight * 2f;
            charRigidbody.velocity = new Vector2(charRigidbody.velocity.x, jumpVector.y);
            extraJumps--;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && charIsGrounded)
        {
            jumpVector = Vector2.up * jumpHeight * 2f;
            charRigidbody.velocity = new Vector2(charRigidbody.velocity.x, jumpVector.y);
        }
    }
    void RunAnimations()
    {
        myAnimator.SetBool("isAttacking", IsAttacking());
        myAnimator.SetBool("isFalling", IsFalling());
        myAnimator.SetBool("isIdling", IsIdling());
        myAnimator.SetBool("isRunning", IsRunning());
        myAnimator.SetBool("isJumping", IsJumping());
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
            }

                // next click time should be that attack rate which is specified by ourselves.
                nextAttackTime = Time.time + 1f / charAttackRate;

            }
     }

    public override void TakeDamage(int damage)
    {
        if (charTimer.isOnCooldown("NoHit") == false)
        {
            charCurrentHealth -= damage;
            if (charCurrentHealth <= 0)
            {
                charRigidbody.velocity = Vector2.zero;
                myAnimator.SetTrigger("Die");
                GetComponent<Collider2D>().enabled = false;
                this.enabled = false;
                FindObjectOfType<GameManager>().EndGame();
            }
            charTimer.ResetCooldownFrame("NoHit");
            charSprite.material = matWhite;
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
            enemy.GetComponent<Fork>().TakeDamage(charAttackDamage);
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
        myAnimator.SetBool("isGrounded", charIsGrounded);
    }


    void AdvancedJump()
    {
        //if going down then speed up the falling
        if (charRigidbody.velocity.y < 0  && !charIsGrounded)
        {
            charRigidbody.velocity += (Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime);
        }
    }

    protected override void ChangeState(CharacterState charState)
    {
        if(this.charState != charState)
        {
            prevState = this.charState;
            this.charState = charState;
        }
        
    }

    protected override void SetCharacterState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastClickedTime = Time.time;
            ChangeState(CharacterState.inAttacking);
        }

        //Setting whether character is running or is idling
        if (!IsAttacking())
        {
            if (move != 0 && charIsGrounded && !IsJumping()) //&& canMove)
            {
                ChangeState(CharacterState.onRunning);
            }
            else if (charIsGrounded & !IsIdling())
            {
                ChangeState(CharacterState.inIdling);
            }
        }
        //
        if (!charIsGrounded && charRigidbody.velocity.y > 0 && !IsAttacking())
        {
            ChangeState(CharacterState.onJumping);
        }
        //
        if (charRigidbody.velocity.y < 0 && !charIsGrounded && !IsAttacking())
        {
            ChangeState(CharacterState.onFalling);
        }

    }

    public void EndAttack()
    {
        if(prevState == CharacterState.inAttacking)
        {
            ChangeState(CharacterState.inIdling);
        }
        else
        {
            ChangeState(prevState);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(charGroundCheckPoint.position, charCheckRadius);
        Gizmos.DrawWireSphere(charAttackPoint.position,charAttackRange);
    } 

    //Plays effects if there is any
    public void PlayEffects()
    {
        if (charTimer.isOnCooldown("NoHit"))
        {
            if(charSprite.material == matWhite)
            {
                charSprite.material = matDefault;
            }
            else if(charSprite.material == matDefault)
            {
                charSprite.material = matWhite;
            }
        }else if(charTimer.isOnCooldown("NoHit") == false && charSprite.material != matDefault)
        {
            charSprite.material = matDefault;
        }
    }
}


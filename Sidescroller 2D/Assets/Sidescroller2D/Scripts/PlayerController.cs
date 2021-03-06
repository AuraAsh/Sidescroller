﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum State
{
    idle, running, jumping, falling, hurt
};
public class PlayerController : MonoBehaviour
{
    //Start() Variables
    private Collider2D coll;
    private Rigidbody2D rb;
    private Animator Anim;
    
   
    //Inspector Variables
    [SerializeField] private State state = State.idle;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private float hurtforce = 10f;
    [SerializeField] private int Score = 0;
    [SerializeField] private Text scoreText;
    [SerializeField] private AudioSource score;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private AudioSource jump;
    [SerializeField] private AudioSource hurt;
    [SerializeField] private int health;
    [SerializeField] private Text healthAmount;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        healthAmount.text = health.ToString();
    }
    private void Update()
    {
        if(state != State.hurt)
        {
            Movement();

        }
        VelocityState();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Collectable")
        {
            scoreText.text = Score.ToString();
            Score += 1;
            score.Play();
            other.GetComponent<Animator>().Play("Item");
            Destroy(other.gameObject,1);   
        }
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (state == State.falling)
            {
                Jump();
                enemy.JumpedOn();
            }
            else
            {
                state = State.hurt;
                HandleHealth();
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    rb.velocity = new Vector2(-hurtforce, rb.velocity.y);
                    hurt.Play();
                }
                else
                {
                    rb.velocity = new Vector2(hurtforce, rb.velocity.y);
                    hurt.Play();
                }
            }
        }
    }

    private void HandleHealth()
    {
        health -= 1;
        healthAmount.text = health.ToString();
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Movement()
    {
        #region SETTINGS MOVEMENT
        float hDirection = Input.GetAxis("Horizontal");

        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            state = State.running;
            transform.localScale = new Vector2(-1, 1);

            Anim.SetBool("isRun", true);
        }
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            state = State.running;
            transform.localScale = new Vector2(1, 1);

            Anim.SetBool("isRun", true);
        }
        else
        {
            Anim.SetBool("isRun", false);
        }
        VelocityState();

        #endregion

        #region SETTINGS JUMP PLAYER
        if (Input.GetButtonDown("Jump") && !coll.IsTouchingLayers())
        {
            Jump();
            jump.Play();
            Anim.SetBool("isJump", true);
        }
        else
        {
            Anim.SetBool("isJump", false);
        }
        #endregion

        #region SETTINGS HURT PLAYER
        if (state == State.hurt)
        {
            rb.velocity = new Vector2(rb.velocity.x, hurtforce);
            Anim.SetBool("isHurt", true);
            state = State.hurt;
        }
        else
        {
            Anim.SetBool("isHurt", false);
        }
        #endregion

    }
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        state = State.jumping;
    }

    private void VelocityState()
    {
        if (state == State.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (coll.IsTouchingLayers())
            {
                state = State.idle;
            }
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
            else
            {
                Anim.SetBool("isHurt", true);
            } 
        }
        else if (Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
    }
    private void Footstep()
    {
        footstep.Play();
    }
}
  
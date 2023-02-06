using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 5;
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttack = false;
    //private bool isDeath = false;
    private float horizontal;
    private int coin = 0;
    [SerializeField] private float jumpForce = 350;
    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    private Vector3 savePoint;
    private void Awake()
    {
        coin = PlayerPrefs.GetInt("coin",0);
    }


    void Update()
    {

        if (isDead)
        {
            return;
        }


        isGrounded = CheckGrounded();
        //Debug.Log(CheckGrounded());
        //horizontal = Input.GetAxisRaw("Horizontal");
        //verticle = Input.GetAxisRaw("Verticle");

        if (isAttack)
        {
            rb.velocity = Vector2.zero;
            return;
        }



        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }
            //Jump 
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }


            //Change anim run
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                changeAnim("run");
            }

            //Attack
            if (Input.GetKeyDown(KeyCode.C) && isGrounded)
            {
                Attack();
            }
            //Throw
            if (Input.GetKeyDown(KeyCode.X) && isGrounded)
            {
                Throw();
            }

        }

        //Check Falling
        if (!isGrounded && rb.velocity.y < 0)
        {
            changeAnim("fall");
            isJumping = false;
        }


        //Run or Idle
        if (Mathf.Abs(horizontal) > 0.1f) //Nh?n phím: l?y h??ng * speed * time
        {
            changeAnim("run");
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            //transform.localScale = new Vector3(horizontal,1,1);
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
        }
        else if (isGrounded)
        {
            changeAnim("idle");
            rb.velocity = Vector2.up * rb.velocity.y; // D?ng tr?c ti?p l?i
        }
    }

    public override void OnInit()
    {
        base.OnInit();

        isAttack = false;
        transform.position = savePoint;
        changeAnim("idle");
        DeactivateAttack();
        SavePoint();
        UIManager.instance.SetCoin(coin);
    }

    protected override void OnDeath()
    {
        base.OnDeath();

    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }
    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);


        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        //if(hit.collider != null)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        return hit.collider != null;
    }
    public void Attack()
    {
        changeAnim("attack");
        isAttack = true;
        Invoke(nameof(resetAttack), 0.5f);
        ActivateAttack();
        Invoke(nameof(DeactivateAttack), 0.5f);
    }
    public void Throw()
    {
        changeAnim("throw");
        isAttack = true;
        Invoke(nameof(resetAttack), 0.5f);
        Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation);
    }

    private void resetAttack()
    {
        isAttack = false;
        changeAnim("idle");
    }

    public void Jump()
    {
        isJumping = true;
        changeAnim("jump");
        rb.AddForce(jumpForce * Vector2.up);
    }


    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void ActivateAttack()
    {
        attackArea.SetActive(true);
    }
    private void DeactivateAttack()
    {
        attackArea.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {

            //Debug.Log("Coin"+collision.gameObject.name);

            coin++;
            PlayerPrefs.SetInt("coin",coin);
            UIManager.instance.SetCoin(coin);
            Destroy(collision.gameObject);
        }

        if (collision.tag == "Deathzone")
        {

            changeAnim("die");
            Invoke(nameof(OnInit), 1f);
        }
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }
}

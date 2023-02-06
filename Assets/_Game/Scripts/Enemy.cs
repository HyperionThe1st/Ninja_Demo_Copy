using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float attackRange;

    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject attackArea;
    private bool isRight = true;
    private Character target;
    public Character Target => target;

    private void Update()
    {
        if (currentState != null && !isDead)
        {
            currentState.OnExecute(this);
        }
    }
    public override void OnInit()
    {
        base.OnInit();
        ChangeState(new IdleState());
        DeactivateAttack();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    protected override void OnDeath()
    {
        ChangeState(null);
        base.OnDeath();
    }

    private IState currentState;
    public void ChangeState(IState newstate)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newstate;
        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }

    public void Moving()
    {
        changeAnim("run");
        rb.velocity = transform.right * moveSpeed;
    }
    public void StopMoving()
    {
        changeAnim("idle");
        rb.velocity = Vector2.zero;
    }
    public void Attack()
    {
        changeAnim("attack");
        ActivateAttack();
        Invoke(nameof(DeactivateAttack), 0.5f);
    }
    public bool isTargetInRange()
    {
        if (target != null && Vector2.Distance(target.transform.position, transform.position) <= attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }

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
        if (collision.tag == "EnemyWall")
        {
            ChangeDirection(!isRight);
        }
    }

    public void ChangeDirection(bool isRight)
    {
        this.isRight = isRight;
        transform.rotation = isRight ? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(Vector3.up * 180);
    }

    internal void SetTarget(Character character)
    {
        this.target = character;
        if (isTargetInRange())
        {
            ChangeState(new AttackState());
        }
        else if (target != null)
        {
            ChangeState(new PatrolState());
        }
        else
        {
            ChangeState(new IdleState());
        }
    }
}


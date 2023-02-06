using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private float hp;
    private string currentAnimName;
    [SerializeField] private Animator anim;
    [SerializeField] protected HealthBar healthBar;

    [SerializeField] private CombatText combatTextPrefabs;
    public bool isDead => hp <= 0;

    private void Start()
    {
        OnInit();
    }
    public virtual void OnInit()
    {
        hp = 100;
        healthBar.OnInit(100,transform);
    }

    protected void changeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            anim.ResetTrigger(animName);
            currentAnimName = animName;
            anim.SetTrigger(currentAnimName);
        }
    }
    public virtual void OnDespawn()
    {

    }

    public void OnHit(float damage)
    {
        if (!isDead)
        {
            hp -= damage;
            if (isDead)
            {
                hp = 0;
                OnDeath();
            }
            healthBar.SetNewHp(hp);
            Instantiate(combatTextPrefabs, transform.position + Vector3.up, Quaternion.identity).OnInit(damage);

        }
    }

    protected virtual void OnDeath()
    {
        changeAnim("die");
        Invoke(nameof(OnDespawn),2f);
    }

}

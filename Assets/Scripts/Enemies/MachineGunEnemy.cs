using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunEnemy : Enemy
{
    [SerializeField] private float attackRange;
    [SerializeField] private float attackTime = 0;

    [SerializeField] private Bullet bulletPrefab;

    private float timer = 0;
    private float setSpeed = 0;

    public void SetMachineGunEnemy(float _attackRange, float _attackTime)
    {
        attackRange = _attackRange;
        attackTime = _attackTime;
    }

    protected override void Start()
    {
        base.Start();
        health = new Health(1, 0, 1);
        setSpeed = speed;
    }

    protected override void Update()
    {
        base.Update();

        if (!target)
            return;

        Attack(attackTime);

        if (Vector2.Distance(transform.position, target.position) < attackRange)
        {
            speed = 0;
        }
        else
        {
            speed = setSpeed;
        }
    }

    public override void Attack(float interval)
    {
        if (timer <= interval)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            weapon.Shoot(bulletPrefab, this, "Player", 20);
        }
    }

    //public override void GetDamage(float damage)
    //{
    //    health.DeductHealth(damage);
    //}
}

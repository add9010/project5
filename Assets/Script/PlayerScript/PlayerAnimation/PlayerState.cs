using UnityEngine;
using System.Collections;

public class PlayerStateController
{
    private PlayerManager pm;
    private enum PlayerState { Idle, Move, Jump, Attack, Roll, Hurt, Dead, Dialog }
    private PlayerState currentState = PlayerState.Idle;

    private float timeSinceAttack;
    private float delayToIdle;
    private int attackCount = 0;
    private bool isAttacking = false;
    private bool grounded = true;

    public PlayerStateController(PlayerManager manager)
    {
        pm = manager;
    }

    public void Update()
    {
        if (currentState == PlayerState.Dead) return;

        HandleInput();
        UpdateStateLogic();
    }

    void FixedUpdate()
    {
        if (currentState == PlayerState.Move)
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            pm.rb.linearVelocity = new Vector2(inputX * pm.data.speed, pm.rb.linearVelocity.y);

            if (inputX != 0)
                pm.spriteRenderer.flipX = inputX < 0;
        }
    }

 private void HandleInput()
{
        if (pm.isAction)
        {
            currentState = PlayerState.Dialog;
            pm.rb.linearVelocity = Vector2.zero;
            pm.animator.SetInteger("AnimState", 0); // Idle 애니메이션 유지
            return;
        }
        timeSinceAttack += Time.deltaTime;

    float inputX = Input.GetAxisRaw("Horizontal");

    if (Input.GetKeyDown(KeyCode.Space) && grounded && !isAttacking)
    {
        currentState = PlayerState.Jump;
        grounded = false;
        pm.animator.SetTrigger("Jump");
        pm.rb.linearVelocity = new Vector2(pm.rb.linearVelocity.x, pm.data.jumpForce);
        return;
    }

    if (Input.GetMouseButtonDown(0) && timeSinceAttack > pm.data.attackDuration && !isAttacking)
    {
        currentState = PlayerState.Attack;
        pm.StartAttackCoroutine(AttackCoroutine());
        return;
    }

    if (Mathf.Abs(inputX) > 0.1f && grounded && !isAttacking)
    {
        currentState = PlayerState.Move;
        return;
    }

    if (grounded && !isAttacking)
    {
        currentState = PlayerState.Idle;
    }
}

    private void UpdateStateLogic()
    {
        pm.animator.SetFloat("AirSpeedY", pm.rb.linearVelocity.y);

        switch (currentState)
        {
            case PlayerState.Idle:
                delayToIdle -= Time.deltaTime;
                if (delayToIdle < 0)
                    pm.animator.SetInteger("AnimState", 0);
                break;

            case PlayerState.Move:
                delayToIdle = 0.05f;
                pm.animator.SetInteger("AnimState", 1);
                break;

            case PlayerState.Dialog:
                pm.animator.SetInteger("AnimState", 0); // 대화 중엔 항상 Idle
                break;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        attackCount++;
        timeSinceAttack = 0f;

        pm.animator.SetTrigger("Attack" + ((attackCount % 3) + 1));
        yield return new WaitForSeconds(pm.data.attackDuration / 2f);
        Attack();

        yield return new WaitForSeconds(pm.data.attackDuration / 2f);
        isAttacking = false;
        if (attackCount >= 3) attackCount = 0;

        currentState = PlayerState.Idle;
    }

    private void Attack()
    {
        Vector3 pos = pm.attackPos.position;
        if (pm.spriteRenderer.flipX)
            pos.x -= pm.data.attackBoxSize.x;

        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, pm.data.attackBoxSize, 0);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                float damage = (attackCount == 3) ? pm.data.attackPower * 1.5f : pm.data.attackPower;
                float knockback = (attackCount == 3) ? pm.data.attackKnockbackThird : pm.data.attackKnockback;

                enemy.TakeDamage(new ParameterPlayerAttack
                {
                    damage = damage,
                    knockback = knockback
                });

                if (attackCount == 3)
                    pm.cameraShake.ShakeCamera();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            grounded = true;
    }

    private void OnDrawGizmos()
    {
        if (pm != null && pm.attackPos != null)
        {
            Gizmos.color = Color.red;
            Vector3 pos = pm.attackPos.position;
            if (pm.spriteRenderer != null && pm.spriteRenderer.flipX)
                pos.x -= pm.data.attackBoxSize.x;
            Gizmos.DrawWireCube(pos, pm.data.attackBoxSize);
        }
    }
}
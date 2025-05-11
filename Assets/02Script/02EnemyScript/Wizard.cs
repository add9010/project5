using System.Collections;
using UnityEngine;

public class Wizard : Enemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    protected override void Start()
    {
        base.Start();
        SetEnemyStatus("마법사", 100, 15, 3);
    }

    public override void PerformAttack()
    {
        if (player == null || projectilePrefab == null || firePoint == null) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.transform.localScale = new Vector3(0.2f, 0.2f, 1f); // 크기 강제 고정

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * 5f;

        SpriteRenderer sr = proj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = dir.x > 0;
    }

}

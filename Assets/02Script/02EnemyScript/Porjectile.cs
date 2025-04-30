using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 3f;
    public float damage = 10f;

    private Animator anim;
    private Rigidbody2D rigid;
    private bool isExploding = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Invoke(nameof(DestroyProjectile), lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploding) return; // 중복 폭발 방지

        if (collision.CompareTag("Player"))
        {
            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player != null && !player.IsDead)
            {
                player.playerHealth.TakeDamage(damage);
            }

            Explode();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            Explode();
        }
    }
    private void Explode()
    {
        if (isExploding) return;

        isExploding = true;
        anim.SetTrigger("explode");     // 폭발 애니메이션 실행
    }

    public void OnExplodeEnd()
    {
        Destroy(gameObject);
    }

    private void DestroyProjectile()
    {
        if (!isExploding)
            Explode(); // Lifetime 다 되어도 터뜨리고 삭제
    }
}

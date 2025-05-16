using UnityEngine;

public class RipleBullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int damage = 10;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;

        // 일정 시간 뒤에 자동 파괴
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // "Enemy" 태그를 가진 적을 맞췄을 때
        if (collision.CompareTag("Enemy"))
        {
            //// Enemy가 Damageable 컴포넌트를 가지고 있다고 가정하고 데미지 전달
            //if (collision.TryGetComponent(out Damageable enemy))
            //{
            //    enemy.TakeDamage(damage);
            //}

            //Destroy(gameObject); // 총알 파괴
        }

        // 벽 같은 다른 오브젝트와 충돌했을 때도 파괴 (옵션)
        if (collision.CompareTag("Wall") || collision.CompareTag("Bubble"))
        {
            Destroy(gameObject);
        }

    }
}

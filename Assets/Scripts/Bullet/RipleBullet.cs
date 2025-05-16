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

        // ���� �ð� �ڿ� �ڵ� �ı�
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // "Enemy" �±׸� ���� ���� ������ ��
        if (collision.CompareTag("Enemy"))
        {
            //// Enemy�� Damageable ������Ʈ�� ������ �ִٰ� �����ϰ� ������ ����
            //if (collision.TryGetComponent(out Damageable enemy))
            //{
            //    enemy.TakeDamage(damage);
            //}

            //Destroy(gameObject); // �Ѿ� �ı�
        }

        // �� ���� �ٸ� ������Ʈ�� �浹���� ���� �ı� (�ɼ�)
        if (collision.CompareTag("Wall") || collision.CompareTag("Bubble"))
        {
            Destroy(gameObject);
        }

    }
}

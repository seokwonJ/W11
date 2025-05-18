using UnityEngine;

public class BubbleBomb : MonoBehaviour
{
    public int health = 100;
    public GameObject bubble;
    public GameObject bullet;


    private bool isBubble;
    private Transform player;

    private Rigidbody2D _rigidBody;
    private Vector3 lastAttackDir;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= -100) Explode();
        if (isBubble) return;

        Debug.Log($"Enemy took {damage} damage! HP: {health}");

        if (health <= 0)
        {
            Die();
        }

    }
    void Die()
    {
        //StartCoroutine(DieBack());
        bubble.SetActive(true);
        isBubble = true;
        //Destroy(gameObject);
    }

    void Explode()
    {
        if (!isBubble) return;
        isBubble = false; 

        int bulletCount = 60;
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Instantiate(bullet, transform.position, rotation);
        }

        Debug.Log("Bomb fired!");

        Camera.main.GetComponent<CameraController>().CameraShaking(0.3f, 0.3f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "RipleBullet")
        {
            if (!isBubble) { lastAttackDir = (transform.position - collision.transform.position).normalized; }

            TakeDamage(10);
            Destroy(collision.gameObject);
        }
        if (collision.tag == "SniperBullet")
        {
            if (!isBubble) { lastAttackDir = (transform.position - collision.transform.position).normalized; }
            TakeDamage(100);
            Destroy(collision.gameObject);
        }

        if (collision.tag == "EnemyRipleBullet")
        {
            Destroy(collision.gameObject);
        }
        if (collision.tag == "EnemySniperBullet")
        {
            Destroy(collision.gameObject);
        }

        if (collision.tag == "Hammer")
        {
            if (isBubble)
            {
                Vector2 direction = (transform.position - player.transform.position).normalized;
                _rigidBody.linearVelocity = direction * player.GetComponent<PlayerAttack>().hammerAttackDamage * 2;
            }
            else
            {
                if (!isBubble) { lastAttackDir = (transform.position - player.transform.position).normalized; }

                TakeDamage(player.GetComponent<PlayerAttack>().hammerAttackDamage);
            }
        }
    }
}

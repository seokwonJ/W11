using UnityEngine;

public class EnemyRifle : MonoBehaviour
{
    public int health = 100;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject bubble;

    public float fireRate = 0.1f;
    public float reloadTime = 2f;
    public int maxAmmo = 30;

    private int currentAmmo;
    private float fireTimer = 0f;
    private bool isReloading = false;

    private Transform player;
    private bool isBubble;

    void Start()
    {
        currentAmmo = maxAmmo;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isBubble)
            return;

        if (isReloading)
            return;

        fireTimer -= Time.deltaTime;

        if (currentAmmo > 0)
        {
            if (fireTimer <= 0f)
            {
                Fire();
                currentAmmo--;
                fireTimer = fireRate;
            }
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    void Fire()
    {
        // 플레이어 방향으로 회전
        Vector2 direction = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle - 90);

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Debug.Log("Enemy fired!");
    }

    System.Collections.IEnumerator Reload()
    {
        Debug.Log("Reloading...");
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reload complete!");
    }

    public void TakeDamage(int damage)
    {
        if (isBubble) return;
        health -= damage;
        Debug.Log($"Enemy took {damage} damage! HP: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Camera.main.GetComponent<CameraController>().CameraOrthographicSizeSetting(61);
        Debug.Log("Enemy died!");
        bubble.SetActive(true);
        isBubble = true;
        gameObject.tag = "Bubble";
        //Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "RipleBullet")
        {
            TakeDamage(10);
            Destroy(collision.gameObject);
        }
        if (collision.tag == "SniperBullet")
        {
            TakeDamage(100);
            Destroy(collision.gameObject);
        }
    }

}

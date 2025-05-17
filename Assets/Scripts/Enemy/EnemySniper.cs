using UnityEngine;

public class EnemySniper : MonoBehaviour
{
    public int health = 100;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject bubble;

    public float fireRate = 2f;    // 탄창 내 발사 간격 (느리게)
    public float reloadTime = 3f;  // 장전 시간
    public int maxAmmo = 5;        // 스나이퍼 탄창 5발

    private int currentAmmo;
    private float fireTimer = 0f;
    private bool isReloading = false;
    private bool isAiming = false;

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

        if (isReloading || isAiming)
            return;

        fireTimer -= Time.deltaTime;

        if (currentAmmo > 0)
        {
            if (fireTimer <= 0f)
            {
                StartCoroutine(AimAndFire());
            }
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    System.Collections.IEnumerator AimAndFire()
    {
        isAiming = true;

        Debug.Log("Sniper aiming...");
        yield return new WaitForSeconds(1.5f);  // 조준 시간

        Fire();

        currentAmmo--;
        fireTimer = fireRate;

        isAiming = false;
    }

    void Fire()
    {
        Vector2 direction = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle - 90);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Debug.Log("Sniper fired!");
    }

    System.Collections.IEnumerator Reload()
    {
        Debug.Log("Sniper Reloading...");
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        Debug.Log("Sniper Reload Complete!");
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
        Camera.main.GetComponent<CameraController>().CameraOrthographicSizeSetting(1);
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

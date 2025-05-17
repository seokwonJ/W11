using System.Collections;
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
    public float bubbleTime = 7f;
    private float bubbleTimer = 1;


    private Vector3 lastAttackDir;
    private Vector3 originalScale;
    private Color originBodyColor;

    private Rigidbody2D _rigidBody;

    void Start()
    {
        currentAmmo = maxAmmo;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalScale = transform.localScale;
        originBodyColor = GetComponent<SpriteRenderer>().color;
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isBubble)
        {
            bubbleTimer -= Time.deltaTime;

            if (bubbleTimer < 1.5f)
            {
                bubble.GetComponent<SpriteRenderer>().color = new Color(0.529f, 0.808f, 0.922f, 0.6f);
            }

            if (bubbleTimer < 0)
            {
                isBubble = false;
                bubble.SetActive(false);
                GetComponent<SpriteRenderer>().color = originBodyColor;
                health = 100;
                _rigidBody.angularVelocity = 0;
                _rigidBody.linearVelocity = Vector2.zero;
                bubble.GetComponent<SpriteRenderer>().color = new Color(0.08395338f, 0, 1, 0.5176471f);
                gameObject.tag = "Enemy";
            }
            return;
        }

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

        if (!isBubble)
        {
            Fire();

            currentAmmo--;
            fireTimer = fireRate;

            isAiming = false;
        }
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
        bubbleTimer = bubbleTime;

        UIManager.Instance.BubbleEnemy();

        StartCoroutine(DieBack());
        GetComponent<SpriteRenderer>().color = Color.magenta;
        isBubble = true;
        //Destroy(gameObject);
    }
    IEnumerator DieBack()
    {
        float knockbackForce = 100f;
        float knockbackDuration = 1f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float timer = 0f;
            while (timer < knockbackDuration)
            {
                if (timer < knockbackDuration / 2) transform.localScale = Vector3.Lerp(transform.localScale, originalScale * 1.5f, Time.deltaTime * 10);
                else
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 10);
                }
                rb.linearVelocity = lastAttackDir.normalized * knockbackForce;
                timer += Time.deltaTime;
                yield return null;
            }

            transform.localScale = originalScale;

            // 넉백 끝나고 멈춤
            rb.linearVelocity = Vector2.zero;
        }

        bubble.SetActive(true);
        gameObject.tag = "Bubble";
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
    }
}

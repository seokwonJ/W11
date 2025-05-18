using System.Collections;
using UnityEngine;

public class EnemyShotgun : MonoBehaviour
{
    public int health = 100;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject bubble;

    public float fireRate = 0.8f; // º¶∞«¿∫ µÙ∑π¿Ã ±Ê∞‘
    public float reloadTime = 2f;
    public int maxAmmo = 10;

    public int pelletsPerShot = 6;
    public float spreadAngle = 30f;

    private int currentAmmo;
    private float fireTimer = 0f;
    private bool isReloading = false;

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
        // «√∑π¿ÃæÓ πÊ«‚¿∏∑Œ ±‚∫ª »∏¿¸∞™ ∞ËªÍ
        Vector2 direction = (player.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, baseAngle - 90);

        // ∆Á∏¥ ∞≥ºˆ∏∏≈≠ ∆€∂ﬂ∑¡º≠ πﬂªÁ
        for (int i = 0; i < pelletsPerShot; i++)
        {
            float randomSpread = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            Quaternion rotation = Quaternion.Euler(0, 0, baseAngle + randomSpread - 90);

            Instantiate(bulletPrefab, firePoint.position, rotation);
        }

        Debug.Log("Enemy Shotgun fired!");
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
        //if (isBubble) return;
        health -= damage;
        Debug.Log($"Enemy took {damage} damage! HP: {health}");

        if (health <= -100)
        {
            Camera.main.GetComponent<CameraController>().CameraShaking(0.15f, 0.15f);
            UIManager.Instance.PangEnemy();
            GetComponent<EnemyDropWeapon>().DeadDropWeapon();
            Destroy(transform.gameObject);
        }
        if (health <= 0)
        {
            if (isBubble) return;
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
        float knockbackForce = 50f;
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

            // ≥ÀπÈ ≥°≥™∞Ì ∏ÿ√„
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

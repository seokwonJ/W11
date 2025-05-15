using UnityEngine;


// 무기 종류 enum
public enum WeaponType
{
    Sniper,
    Shotgun,
    Rifle
}

public class PlayerAttack : MonoBehaviour
{
    public WeaponType currentWeapon = WeaponType.Rifle;

    public GameObject ripleBulletPrefab;
    public GameObject sniperBulletPrefab;
    public Transform firePoint;

    public float sniperDelay = 1.5f;
    public float shotgunSpread = 15f;
    public int shotgunPellets = 5;
    public float rifleFireRate = 0.1f;

    private float attackTimer = 0f;
    public float killRadius = 3f;

    public GameObject ripleTrigger;
    public GameObject shotgunTrigger;
    public GameObject sniperTrigger;

    public GameObject ripleHand;
    public GameObject shotgunHand;
    public GameObject sniperHand;

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && attackTimer <= 0f)
        {
            switch (currentWeapon)
            {
                case WeaponType.Sniper:
                    FireSniper();
                    attackTimer = sniperDelay;
                    break;
                case WeaponType.Shotgun:
                    FireShotgun();
                    attackTimer = 0.8f; // 샷건 발사 후 딜레이
                    break;
                case WeaponType.Rifle:
                    FireRifle();
                    attackTimer = rifleFireRate;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = WeaponType.Rifle;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = WeaponType.Shotgun;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = WeaponType.Sniper;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GetWeapon();
            KillNearbyBubbles();
        }
    }

    void FireSniper()
    {
        Instantiate(sniperBulletPrefab, firePoint.position, firePoint.rotation);
        Debug.Log("Sniper Fire!");
    }

    void FireShotgun()
    {
        for (int i = 0; i < shotgunPellets; i++)
        {
            Quaternion spread = Quaternion.Euler(0, 0, Random.Range(-shotgunSpread, shotgunSpread));
            Instantiate(ripleBulletPrefab, firePoint.position, firePoint.rotation * spread);
        }
        Debug.Log("Shotgun Fire!");
    }

    void FireRifle()
    {
        Instantiate(ripleBulletPrefab, firePoint.position, firePoint.rotation);
        Debug.Log("Rifle Fire!");
    }

    void KillNearbyBubbles()
    {
        GameObject[] bubbles = GameObject.FindGameObjectsWithTag("Bubble");

        foreach (GameObject bubble in bubbles)
        {
            float distance = Vector2.Distance(transform.position, bubble.transform.position);

            if (distance <= 6)
            {
                bubble.GetComponent<EnemyDropWeapon>().DeadDropWeapon();
                Destroy(bubble);
                Debug.Log($"Bubble {bubble.name} destroyed at distance {distance}");
            }
        }
    }


    void GetWeapon()
    {
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Gun");

        foreach (GameObject weapon in weapons)
        {
            float distance = Vector2.Distance(transform.position, weapon.transform.position);

            if (distance <= 6)
            {
                if (currentWeapon == WeaponType.Rifle) {
                    Instantiate(ripleTrigger, transform.position, Quaternion.identity);
                    ripleHand.SetActive(false);
                }

                if (currentWeapon == WeaponType.Shotgun) {
                    Instantiate(shotgunTrigger, transform.position, Quaternion.identity);
                    shotgunHand.SetActive(false);
                }

                if (currentWeapon == WeaponType.Sniper)
                {
                    Instantiate(sniperTrigger, transform.position, Quaternion.identity);
                    sniperHand.SetActive(false);
                }

                currentWeapon = weapon.GetComponent<GunInformation>().weaponType;

                if (currentWeapon == WeaponType.Rifle)
                {
                    ripleHand.SetActive(true);
                }

                if (currentWeapon == WeaponType.Shotgun)
                {
                    shotgunHand.SetActive(true);
                }

                if (currentWeapon == WeaponType.Sniper)
                {
                    sniperHand.SetActive(true);
                }

                Destroy(weapon);
                break;
            }
        }
    }
}

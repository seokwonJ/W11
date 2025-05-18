using System.Collections;
using UnityEngine;
using UnityEngine.UI;


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

    public GameObject rifleBulletPrefab;
    public GameObject sniperBulletPrefab;
    public Transform firePoint;

    public float sniperDelay = 1.5f;
    public float shotgunSpread = 15f;
    public int shotgunPellets = 5;
    public float rifleFireRate = 0.1f;
    public float sniperSpread = 15f;
    public float nowSniperSpread = 15f;

    private float attackTimer = 0f;
    public float killRadius = 3f;

    public GameObject rifleTrigger;
    public GameObject shotgunTrigger;
    public GameObject sniperTrigger;

    public GameObject rifleHand;
    public GameObject shotgunHand;
    public GameObject sniperHand;
    public GameObject hammerHand;

    public GameObject rifleCrossHair;
    public GameObject shotgunCrossHair;
    public GameObject sniperCrossHair;
    public GameObject hammerCrossHair;

    public Text weaponName_Text;
    public Text currentBulletCount_Text;
    public Text totalBulletCount_Text;

    private PlayerHP _playerHP;
    private PlayerMove _playerMove;
    
    private int totalBulletCount;
    private int currentBulletCount;

    private float reloadTime;
    private bool isReloading;

    float zoomTime = 0.5f;
    float zoomTimer = 0f;
    public bool isZooming = false;
    private CameraController _cameraController;

    public bool isHammer;
    public float hammerDamageGauage;
    public int hammerAttackDamage;

    private void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        totalBulletCount = 90;
        currentBulletCount = 30;
        weaponName_Text.text = "Rifle";
        weaponName_Text.color = Color.blue;
        currentBulletCount_Text.text = currentBulletCount.ToString();
        totalBulletCount_Text.text = totalBulletCount.ToString();
        nowSniperSpread = sniperSpread;
        _cameraController = Camera.main.GetComponent<CameraController>();

    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (_playerMove.isPanging) return;  // Pang 중일 때는 행동x

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (scroll > 0f)
            {
                isHammer = false;
            }
            else if (scroll < 0f)
            {
                isHammer = true;
            }
            ChangeWeapons(isHammer);
        }

        if (Input.GetMouseButton(0) && attackTimer <= 0f)
        {
            if (isReloading) return;
            if (isHammer) {
                if (hammerDamageGauage <= 100)
                {
                    hammerDamageGauage += Time.deltaTime * 50;
                    hammerCrossHair.transform.GetChild(1).GetComponent<Image>().fillAmount = hammerDamageGauage/100;
                }
                return;
            }
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
        if (Input.GetMouseButtonUp(0))
        {
            if (isHammer)
            {
                FireHammer();
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (currentWeapon == WeaponType.Sniper)
            {
                if (!isZooming)
                {
                    isZooming = true;
                    _cameraController.isZoomingCamera = true;
                    zoomTimer = 0f;
                }

                zoomTimer += Time.deltaTime;
                float t = Mathf.Clamp01(zoomTimer / zoomTime);
                nowSniperSpread = Mathf.Lerp(sniperSpread, 1f, t);
                sniperCrossHair.transform.GetChild(0).GetComponent<Image>().fillAmount = 1 - t + 0.05f;

            }
            else
            {
                if (!isZooming)
                {
                    isZooming = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (currentWeapon == WeaponType.Sniper) ZoomCancle();
            else
            {
                isZooming = false;
                hammerDamageGauage = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GetWeapon();
            KillNearbyBubbles();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isReloading) return;
            if (totalBulletCount <= 0) return;
            StartCoroutine(WeaponReloading());
        }
    }

    void ZoomCancle()
    {
        isZooming = false;
        _cameraController.isZoomingCamera = false;
        nowSniperSpread = sniperSpread;
        sniperCrossHair.transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
        zoomTimer = 0;

        // 테스트용 로그
        Debug.Log(nowSniperSpread);
    }

    void FireSniper()
    {
        if (currentBulletCount <= 0)
        {
            return;
        }

        if (currentBulletCount == 3)
        {
            sniperCrossHair.transform.GetChild(2).gameObject.SetActive(true);
        }

        _cameraController.CameraShaking(0.1f, 0.2f);

        Vector2 fireDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;

        Quaternion spread = Quaternion.Euler(0, 0, Random.Range(-nowSniperSpread, nowSniperSpread));
        Instantiate(sniperBulletPrefab, firePoint.position, Quaternion.LookRotation(Vector3.forward, fireDir) * spread);

        currentBulletCount -= 1;

        if (currentBulletCount <= 0)
        {
            StartCoroutine(WeaponReloading());
        }

        currentBulletCount_Text.text = currentBulletCount.ToString();
        Debug.Log("Sniper Fire!");
    }

    void FireHammer()
    {
        hammerHand.GetComponent<Animator>().Play("HammerAnim", -1, 0f);
        StartCoroutine(SwingHammer());
    }

    void FireShotgun()
    {
        if (currentBulletCount <= 0)
        {
            return;
        }

        if (currentBulletCount == 3)
        {
            shotgunCrossHair.transform.GetChild(1).gameObject.SetActive(true);
        }

        Vector2? targetDir = FindClosestEnemyToCursor();
        Vector2 fireDir = targetDir.HasValue ? targetDir.Value : (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;

        for (int i = 0; i < shotgunPellets; i++)
        {
            Quaternion spread = Quaternion.Euler(0, 0, Random.Range(-shotgunSpread, shotgunSpread));
            Instantiate(rifleBulletPrefab, firePoint.position, Quaternion.LookRotation(Vector3.forward, fireDir) * spread);
        }
        shotgunCrossHair.GetComponent<FollowCrossHair>().ScaleSetting(Vector3.one * 1.2f);
        currentBulletCount -= 1;

        if (currentBulletCount <= 0)
        {
            StartCoroutine(WeaponReloading());

        }
        currentBulletCount_Text.text = currentBulletCount.ToString();
        Debug.Log("Shotgun 3r!");
    }

    void FireRifle()
    {
        
        if (currentBulletCount <= 0)
        {
            return;
        }

        if (currentBulletCount == 10)
        {
            rifleCrossHair.transform.GetChild(1).gameObject.SetActive(true);
        }

        Vector2? targetDir = FindClosestEnemyToCursor();
        Vector2 fireDir = targetDir.HasValue ? targetDir.Value : (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;

        rifleCrossHair.GetComponent<FollowCrossHair>().ScaleSetting(Vector3.one * 1.1f);
        Instantiate(rifleBulletPrefab, firePoint.position, Quaternion.LookRotation(Vector3.forward, fireDir));
        currentBulletCount -= 1;

        if (currentBulletCount <= 0)
        {
            StartCoroutine(WeaponReloading());
        }

        currentBulletCount_Text.text = currentBulletCount.ToString();
        Debug.Log("Rifle Fire!");
    }

    void KillNearbyBubbles()
    {
        GameObject[] bubbles = GameObject.FindGameObjectsWithTag("Bubble");

        foreach (GameObject bubble in bubbles)
        {
            float distance = Vector2.Distance(transform.position, bubble.transform.position);

            if (distance <= 25)
            {
                Debug.Log($"Bubble {bubble.name} destroyed at distance {distance}");
                //StartCoroutine(PangStop());
                StartCoroutine(_playerMove.PangDoing(bubble));
                break;
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
                StopAllCoroutines();
                isReloading = false;
                
                GameObject dropGun = null;
                if (currentWeapon == WeaponType.Rifle) {
                    dropGun = Instantiate(rifleTrigger, transform.position, Quaternion.identity);

                    rifleHand.SetActive(false);
                    rifleCrossHair.SetActive(false);
                }

                if (currentWeapon == WeaponType.Shotgun) {
                    dropGun = Instantiate(shotgunTrigger, transform.position, Quaternion.identity);  
                    shotgunHand.SetActive(false);
                    shotgunCrossHair.SetActive(false);
                }

                if (currentWeapon == WeaponType.Sniper)
                {
                    dropGun = Instantiate(sniperTrigger, transform.position, Quaternion.identity);
                    sniperHand.SetActive(false);
                    sniperCrossHair.SetActive(false);
                }

                dropGun.GetComponent<GunInformation>().totalBulletCount = totalBulletCount;
                dropGun.GetComponent<GunInformation>().currentBulletCount = currentBulletCount;

                currentWeapon = weapon.GetComponent<GunInformation>().weaponType;


                if (currentWeapon == WeaponType.Rifle)
                {
                    rifleHand.SetActive(true);
                    rifleCrossHair.SetActive(true);
                    weaponName_Text.text = "Rifle";
                    weaponName_Text.color = Color.blue;
                    //totalBulletCount = 90;
                    //currentBulletCount = 30;
                }

                if (currentWeapon == WeaponType.Shotgun)
                {
                    shotgunHand.SetActive(true);
                    shotgunCrossHair.SetActive(true);
                    weaponName_Text.text = "Shotgun";
                    weaponName_Text.color = Color.green;
                    //totalBulletCount = 21;
                    //currentBulletCount = 7;
                }

                if (currentWeapon == WeaponType.Sniper)
                {
                    sniperHand.SetActive(true);
                    sniperCrossHair.SetActive(true);
                    weaponName_Text.text = "Sniper";
                    weaponName_Text.color = Color.red;
                    //totalBulletCount = 15;
                    //currentBulletCount = 5;
                }

                totalBulletCount = weapon.GetComponent<GunInformation>().totalBulletCount;
                currentBulletCount = weapon.GetComponent<GunInformation>().currentBulletCount;

                totalBulletCount_Text.text = totalBulletCount.ToString();
                currentBulletCount_Text.text = currentBulletCount.ToString();
                OutHammer();

                Destroy(weapon);
                break;
            }
        }
    }

    Vector2? FindClosestEnemyToCursor()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // 적의 태그 확인해줘야 함
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(mouseWorldPos, enemy.transform.position);
            if (currentWeapon == WeaponType.Rifle)
            {
                if (distance <= 12f && distance < closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }
            if (currentWeapon == WeaponType.Shotgun)
            {
                if (distance <= 25f && distance < closestDistance)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }
        }

        if (closestEnemy != null)
        {
            return ((Vector2)closestEnemy.transform.position - (Vector2)firePoint.position).normalized;
        }

        return null;
    }

    public void ChangeWeapons(bool ishammer)
    {
        if (!ishammer)
        {
            if (currentWeapon == WeaponType.Rifle)
            {
                rifleHand.SetActive(true);
                rifleCrossHair.SetActive(true);
                weaponName_Text.text = "Rifle";
                weaponName_Text.color = Color.blue;
            }

            if (currentWeapon == WeaponType.Shotgun)
            {
                shotgunHand.SetActive(true);
                shotgunCrossHair.SetActive(true);
                weaponName_Text.text = "Shotgun";
                weaponName_Text.color = Color.green;
            }

            if (currentWeapon == WeaponType.Sniper)
            {
                sniperHand.SetActive(true);
                sniperCrossHair.SetActive(true);
                weaponName_Text.text = "Sniper";
                weaponName_Text.color = Color.red;
            }
            totalBulletCount_Text.text = totalBulletCount.ToString();
            currentBulletCount_Text.text = currentBulletCount.ToString();
            OutHammer();
        }
        else
        {
            if (currentWeapon == WeaponType.Rifle)
            {
                rifleHand.SetActive(false);
                rifleCrossHair.SetActive(false);
            }

            if (currentWeapon == WeaponType.Shotgun)
            {
                shotgunHand.SetActive(false);
                shotgunCrossHair.SetActive(false);
            }

            if (currentWeapon == WeaponType.Sniper)
            {
                sniperHand.SetActive(false);
                sniperCrossHair.SetActive(false);
            }

            hammerCrossHair.SetActive(true);
            hammerHand.SetActive(true);
            weaponName_Text.text = "Hammer";
            weaponName_Text.color = Color.yellow;
            totalBulletCount_Text.text = 0.ToString();
            currentBulletCount_Text.text = 0.ToString();
        }
    }

    public void OutHammer()
    {
        isHammer = false;
        hammerCrossHair.SetActive(false);
        hammerHand.SetActive(false);
    }

    IEnumerator WeaponReloading()
    {
        isReloading = true;

        if (currentWeapon == WeaponType.Rifle)
        {
            rifleCrossHair.transform.GetChild(1).gameObject.SetActive(false);
            rifleCrossHair.transform.GetChild(2).gameObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            rifleCrossHair.transform.GetChild(2).gameObject.SetActive(false);

            totalBulletCount = totalBulletCount + currentBulletCount - 30;
            if (totalBulletCount < 0)
            {
                currentBulletCount = 30 + totalBulletCount;
                totalBulletCount = 0;
            }
            else
            {
                currentBulletCount = 30;
            }
        }

        if (currentWeapon == WeaponType.Shotgun)
        {
            shotgunCrossHair.transform.GetChild(1).gameObject.SetActive(false);
            shotgunCrossHair.transform.GetChild(2).gameObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            shotgunCrossHair.transform.GetChild(2).gameObject.SetActive(false);

            totalBulletCount = totalBulletCount + currentBulletCount - 7;
            if (totalBulletCount < 0)
            {
                currentBulletCount = 7 + totalBulletCount;
                totalBulletCount = 0;
            }
            else
            {
                currentBulletCount = 7;
            }
        }

        if (currentWeapon == WeaponType.Sniper)
        {
            sniperCrossHair.transform.GetChild(2).gameObject.SetActive(false);
            sniperCrossHair.transform.GetChild(3).gameObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            sniperCrossHair.transform.GetChild(3).gameObject.SetActive(false);

            totalBulletCount = totalBulletCount + currentBulletCount - 5;
            if (totalBulletCount < 0)
            {
                currentBulletCount = 5 + totalBulletCount;
                totalBulletCount = 0;
            }
            else
            {
                currentBulletCount = 5;
            }
        }

        totalBulletCount_Text.text = totalBulletCount.ToString();
        currentBulletCount_Text.text = currentBulletCount.ToString();

        isReloading = false;
        if (isHammer)
        {
            totalBulletCount_Text.text = 0.ToString();
            currentBulletCount_Text.text = 0.ToString();
        }
        
    }

    IEnumerator SwingHammer()
    {
        print("hammerDamage" + hammerDamageGauage);

        hammerAttackDamage = (int)hammerDamageGauage;
        hammerHand.tag = "Hammer";
        yield return new WaitForSeconds(0.3f);
        hammerHand.tag = "Untagged";
        hammerDamageGauage = 0;
        hammerCrossHair.transform.GetChild(1).GetComponent<Image>().fillAmount = hammerDamageGauage;
    }
}

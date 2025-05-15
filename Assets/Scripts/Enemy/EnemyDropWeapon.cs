using UnityEngine;

public class EnemyDropWeapon : MonoBehaviour
{
    public GameObject Gun;

    public void DeadDropWeapon()
    {
        print("ÆÎ!");
        Instantiate(Gun,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}

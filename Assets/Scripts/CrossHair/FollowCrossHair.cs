using UnityEngine;

public class FollowCrossHair : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // 2D´Ï±î Z´Â 0
        transform.position = mouseWorldPos;
    }
}

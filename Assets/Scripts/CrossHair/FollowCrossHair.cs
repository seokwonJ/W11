using UnityEngine;

public class FollowCrossHair : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 10);
        mouseWorldPos.z = 0f; // 2D´Ï±î Z´Â 0
        transform.position = mouseWorldPos;
    }

    public void ScaleSetting(Vector3 scale)
    {
        transform.localScale = scale;
    }
}

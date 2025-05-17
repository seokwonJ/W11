using UnityEngine;

public class FollowCrossHair : MonoBehaviour
{
    public GameObject player; // �÷��̾� ������Ʈ ����

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // 2D�ϱ� Z�� 0
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 10);
        transform.position = mouseWorldPos;
    }

    public void ScaleSetting(Vector3 scale)
    {
        transform.localScale = scale;
    }
}

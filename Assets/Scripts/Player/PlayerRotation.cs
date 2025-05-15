using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Transform playerBody; // ȸ���� �ڽ� ������Ʈ (��������Ʈ �ִ� ��)

    private void Start()
    {

    }

    void Update()
    {
        // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // playerBody �������� ���콺 ���� ���� ���
        Vector2 direction = (mouseWorldPos - playerBody.position);

        // ���� ���͸� ������ ��ȯ (���� -> ��)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // playerBody ȸ�� ���� (Z�ุ ȸ��)
        playerBody.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}

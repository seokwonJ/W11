using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Transform playerBody; // 회전할 자식 오브젝트 (스프라이트 있는 것)

    private void Start()
    {

    }

    void Update()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // playerBody 기준으로 마우스 방향 벡터 계산
        Vector2 direction = (mouseWorldPos - playerBody.position);

        // 방향 벡터를 각도로 변환 (라디안 -> 도)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // playerBody 회전 적용 (Z축만 회전)
        playerBody.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}

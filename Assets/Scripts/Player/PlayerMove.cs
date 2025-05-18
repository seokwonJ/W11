using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float pangRushSpeed = 2f;
    public float dashDuration = 0.5f;
    public float dashInputWindow = 0.3f;  // 0.3초 이후부터 입력받기
    public float dashRecoveryTime = 0.2f; // 대쉬 후 경직 시간
    public AnimationCurve dashDecayCurve;
    public float rushTime = 3f;
    public float pangRushTime = 5f;

    public Transform playerBody;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveVelocity;
    private Vector2 dashVelocity;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float lastDashEndTime = -Mathf.Infinity;

    private bool canQueueDash = false;
    private bool queuedDash = false;

    private bool isRecovering = false;
    private float recoveryTimer = 0f;

    private bool isRushing = false;
    private float rushTimer = 0f;
    private float lastRushEndTime = -Mathf.Infinity;

    private bool isPangRush = false;
    private float pangRushTimer = 0f;

    private PlayerAttack _playerAttack;
    public bool isPanging;

    public GameObject dashEffect;
    private PlayerHP _playerHP;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerHP = GetComponent<PlayerHP>();
    }

    void Update()
    {
        if (!isRecovering && !isPanging) // 경직 중엔 입력 무시
        {
            HandleDashInput();
            HandleMovementInput();
            HandleRushInput();
        }
    }

    void FixedUpdate()
    {
        if (_playerAttack.isZooming)
        {
            rb.linearVelocity = Vector2.zero; // 경직 중엔 이동 불가
            dashTimer = 0;
            return;
        }

        if (isPanging)
        {
            rb.angularVelocity = 0;
            return;
        }

        if (isRecovering)
        {
            rb.linearVelocity = Vector2.zero; // 경직 중엔 이동 불가
        }
        else if (isDashing)
        {
            float t = 1f - (dashTimer / dashDuration);
            float speedMultiplier = dashDecayCurve.Evaluate(t);

            rb.linearVelocity = dashVelocity * dashSpeed * speedMultiplier;
        }
        else
        {
            if (isRushing)
            {
                rb.linearVelocity = moveVelocity;
            }
            else if (isPangRush)
            {
                rb.linearVelocity = moveVelocity * pangRushSpeed;
            }
            else
            {
                rb.linearVelocity = moveVelocity;
            }
        }
    }

    void HandleMovementInput()
    {
        float inputY = Input.GetAxisRaw("Vertical");
        float inputX = Input.GetAxisRaw("Horizontal");

        //if (isRushing) inputX = 0;

        //Vector2 forward = playerBody.up;
        //Vector2 right = playerBody.right;

        //moveVelocity = (forward * inputY + right * inputX).normalized * moveSpeed;
        moveVelocity = ( inputY * Vector2.up + inputX * Vector2.right).normalized * moveSpeed;
    }

    void HandleDashInput()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (!canQueueDash && dashTimer <= dashDuration - dashInputWindow)
            {
                canQueueDash = true;
            }

            if (!canQueueDash &&Input.GetKeyDown(KeyCode.Space))
            {
                queuedDash = true;
            }

            if (canQueueDash && !queuedDash && Input.GetKeyDown(KeyCode.Space))
            {
                //queuedDash = true;
                isDashing = true;
                dashTimer = dashDuration;
                dashVelocity = moveVelocity;

                canQueueDash = false;

                Instantiate(dashEffect, transform.position, Quaternion.LookRotation(-dashVelocity.normalized, Vector3.up), transform);
            }

            if (dashTimer <= 0f)
            {
                isDashing = false;
                lastDashEndTime = Time.time;

                canQueueDash = false;
                queuedDash = false;

                // 대쉬 후 경직 시작
                isRecovering = true;
                recoveryTimer = dashRecoveryTime;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && (Time.time >= lastDashEndTime))
            {
                isDashing = true;
                dashTimer = dashDuration;
                dashVelocity = moveVelocity;

                canQueueDash = false;
                queuedDash = false;

                Instantiate(dashEffect, transform.position, Quaternion.LookRotation(-dashVelocity.normalized, Vector3.up), transform);
            }
        }
    }

    void HandleRushInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastRushEndTime < 0.2f)
            {
                isRushing = true;
                rushTimer = rushTime;
            }
            else { lastRushEndTime = Time.time; }
        }
        if (isRushing)
        {
            if (rushTimer < 0)
            {
                rushTimer = rushTime;
                isRushing = false;
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
                isRushing = false;
            }
            rushTimer -= Time.deltaTime;
        }

    }

    public void PangRushInput()
    {
        print("PangRushInput");
        isPangRush = true;
        pangRushTimer = pangRushTime;
    }

    void LateUpdate()
    {
        if (isRecovering)
        {
            recoveryTimer -= Time.deltaTime;
            if (recoveryTimer <= 0f)
            {
                isRecovering = false;
            }
        }

        if (isPangRush)
        {
            pangRushTimer -= Time.deltaTime;
            if (pangRushTimer <= 0f)
            {
                isPangRush = false;
            }
        }
    }

    public float targetScale = 1.2f;
    public float duration = 0.8f;

    public IEnumerator PangDoing(GameObject closeEnemy)
    {

        Vector3 originalScale = Vector3.one;
        Vector3 enlargedScale = Vector3.one * targetScale;

        isPanging = true;

        Vector3 targetPos = closeEnemy.transform.position;
        float PangMoveSpeed = 175f;
        float stopDistance = 6f; // 거의 붙었을 때

        playerBody.up = (targetPos - transform.position).normalized;

        rb.linearVelocity = Vector3.zero;
        dashTimer = 0;

        // 빠르게 이동
        while (Vector2.Distance(transform.position, targetPos) > stopDistance)
        {
            Vector2 newPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * PangMoveSpeed);
            rb.MovePosition(newPos);
            yield return null;
        }

        closeEnemy.GetComponent<EnemyDropWeapon>().DeadDropWeapon();
        Destroy(closeEnemy);
        UIManager.Instance.PangEnemy();

        rb.linearVelocity = Vector2.zero;

        Camera.main.GetComponent<CameraController>().CameraShaking(0.15f, 0.15f);
        Camera.main.GetComponent<CameraController>().CameraOrthographicSizeSetting(2);

        // 커지기
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(originalScale, enlargedScale, t);
            yield return null;
        }

        // 작아지기
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(enlargedScale, originalScale, t);
            yield return null;
        }

        isPanging = false;

        _playerHP.HealHP(50);
        transform.localScale = originalScale; // 안전하게 리셋
        PangRushInput();
    }
}

using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float dashInputWindow = 0.3f;  // 0.3초 이후부터 입력받기
    public float dashRecoveryTime = 0.2f; // 대쉬 후 경직 시간
    public AnimationCurve dashDecayCurve;
    public float rushTime = 3f;

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


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isRecovering) // 경직 중엔 입력 무시
        {
            HandleMovementInput();
            HandleDashInput();
            HandleRushInput();
        }
    }

    void FixedUpdate()
    {
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
    }
}

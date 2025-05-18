using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }


    public float comboTime;
    public Text reactionText;
    private float comboTimer;
    private int bubbleCount = 0;
    private bool isCombo;


    private void Update()
    {
        if (comboTimer < 0 && isCombo)
        {
            isCombo = false;
            bubbleCount = 0;
            reactionText.gameObject.SetActive(false);
        }
        else
        {
            comboTimer -= Time.deltaTime;
        }
    }

    public void BubbleEnemy()
    {
        comboTimer = comboTime;
        reactionText.gameObject.SetActive(true);
        isCombo = true;

        bubbleCount += 1;
        if (bubbleCount == 1)
        {
            reactionText.text = "One Bubble";
            reactionText.color = Color.white;
        }
        if (bubbleCount == 2)
        {
            reactionText.text = "Double Bubble";
            reactionText.color = Color.yellow;
        }
        if (bubbleCount == 3)
        {
            reactionText.text = "Triple!";
            reactionText.color = Color.green;
        }
        if (bubbleCount == 4)
        {
            reactionText.text = "Wow!!";
            reactionText.color = Color.blue;
        }
        if (bubbleCount == 5)
        {
            reactionText.text = "Amazing!!!";
            reactionText.color = Color.magenta;
        }
        if (bubbleCount >= 6)
        {
            reactionText.text = "INCREDIBLE!!!";
            reactionText.color = Color.red;
        }
        reactionText.gameObject.GetComponent<Animator>().Play("ResultAnim", -1, 0f);
    }
    public void PangEnemy()
    {
        comboTimer = comboTime;
        reactionText.gameObject.SetActive(true);
        isCombo = true;

        reactionText.color = Color.cyan;

        reactionText.text = "Pang!";
        reactionText.gameObject.GetComponent<Animator>().Play("ResultAnim", -1, 0f);
    }
}

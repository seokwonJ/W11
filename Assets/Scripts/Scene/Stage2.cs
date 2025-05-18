using UnityEngine;
using UnityEngine.UI;

public class Stage2 : MonoBehaviour
{
    public bool inBombArea;
    public GameObject Bomb;
    public GameObject ExitPoint;
    public Text Stage2Information;
    public Text Timer;
    public Image bombGauge;
    public GameObject bombSettingUI;

    private bool isBombSetting;
    private float settingBombTime = 5;
    private float settingBombTimer;
    private float bombTime = 30;
    private float bombTimer;


    // Update is called once per frame
    void Update()
    {
        if (inBombArea && !isBombSetting)
        {
            if(Input.GetKey(KeyCode.E))
            {
                if (settingBombTimer > settingBombTime)
                {
                    CompleteBomb();
                }
                settingBombTimer += Time.deltaTime;
                bombGauge.fillAmount = settingBombTimer / settingBombTime;
            }
        }
        if(isBombSetting)
        {
            bombTimer -= Time.deltaTime;
            Timer.text = bombTimer.ToString("F1");
        }
    }

    public void CompleteBomb()
    {
        Stage2Information.text = "안전 지역으로 돌아가라!";
        Timer.gameObject.SetActive(true);
        isBombSetting = true;
        bombTimer = bombTime;
        Bomb.SetActive(true);
        ExitPoint.SetActive(true);
        bombSettingUI.SetActive(false);
        bombTimer = bombTime;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isBombSetting)
        {
            bombSettingUI.SetActive(true);
            inBombArea = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isBombSetting)
        {
            bombSettingUI.SetActive(false);
            inBombArea = false;
        }
    }
}

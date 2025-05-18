using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public int health = 100;
    public Text hpText;

    public GameObject bubble;
    private bool isBubble;
    private PlayerMove _playerMove;

    private void Start()
    {
        hpText.text = health.ToString();
        _playerMove = GetComponent<PlayerMove>();
    }

    public void TakeDamage(int damage)
    {
        if (_playerMove.isPanging) return;
        health -= damage;
        hpText.text = health.ToString();
        Debug.Log($"Player took {damage} damage! HP: {health}");

        if (health <= 0)
        {
            InBubble();
        }
    }

    public void HealHP(int Heal)
    {
        health += Heal;
        if (health > 200)
        {
            health = 200;
        }
        hpText.text = health.ToString();
    }

    void InBubble()
    {
        Debug.Log("Player died!");
        bubble.SetActive(true);
        isBubble = true;
        //gameObject.tag = "Bubble";
        //Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "EnemyRipleBullet")
        {
            TakeDamage(10);
            Destroy(collision.gameObject);
        }
        if (collision.tag == "EnemySniperBullet")
        {
            TakeDamage(100);
            Destroy(collision.gameObject);
        }
    }
}

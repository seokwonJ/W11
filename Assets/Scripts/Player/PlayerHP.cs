using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int health = 100;

    public GameObject bubble;
    private bool isBubble;
    private PlayerMove _playerMove;

    private void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
    }

    public void TakeDamage(int damage)
    {
        if (_playerMove.isPanging) return;
        health -= damage;
        Debug.Log($"Player took {damage} damage! HP: {health}");

        if (health <= 0)
        {
            InBubble();
        }
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

using UnityEngine;

public class Stage4 : MonoBehaviour
{
    public GameObject Exit;

    // Update is called once per frame
    void Update()
    {
        if (UIManager.Instance.totalPang == 28)
        {
            Exit.SetActive(true);
        }
    }
}

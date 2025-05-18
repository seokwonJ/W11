using UnityEngine;

public class Stage3 : MonoBehaviour
{
    public GameObject Exit;

    // Update is called once per frame
    void Update()
    {
        if (UIManager.Instance.totalPang == 10)
        {
            Exit.SetActive(true);
        }
    }
}

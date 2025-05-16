using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Camera cameraNow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraNow = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);

        cameraNow.orthographicSize = Mathf.Lerp(cameraNow.orthographicSize, 60, Time.deltaTime * 20);
    }

    public void CameraOrthographicSizeSetting(float size) {
        cameraNow.orthographicSize = size;
    }

    public void CameraShaking(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}

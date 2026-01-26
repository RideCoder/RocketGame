using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public float duration = 0.1f;
    public float magnitude = 0.1f;

    Vector3 originalPos;
    Coroutine shakeRoutine;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 offset = Random.insideUnitSphere * magnitude;
            transform.localPosition = originalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}

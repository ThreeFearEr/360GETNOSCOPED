using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {
    private Vector3 originalPosition;
    private float originalSize;
    private Coroutine shakeCoroutine;
    private Coroutine zoomCoroutine;

    // Call this method to start the screen shake
    public void Shake(Vector3 direction, float power, float duration) {
        if(shakeCoroutine != null) {
            transform.localPosition = originalPosition;
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine(direction, power, duration));
    }

    private IEnumerator ShakeCoroutine(Vector2 direction, float power, float duration) {
        originalPosition = transform.localPosition;
        float elapsedTime = 0f;

        while(elapsedTime < duration) {
            float smoothFactor = Mathf.Lerp(power, 0, elapsedTime / duration);

            float offsetX = Mathf.PerlinNoise(Time.time * 10f, 0f) * 2f - 1f;
            float offsetY = Mathf.PerlinNoise(0f, Time.time * 10f) * 2f - 1f;

            offsetX *= smoothFactor * direction.x;
            offsetY *= smoothFactor * direction.y;

            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }

    // Call this method to start a zoom-based screen shake
    public void ZoomShake(float zoomSize, float duration) {
        if(zoomCoroutine != null) {
            StopCoroutine(zoomCoroutine);
            Camera.main.orthographicSize = originalSize;
        }
        zoomCoroutine = StartCoroutine(ZoomShakeCoroutine(zoomSize, duration));
    }

    private IEnumerator ZoomShakeCoroutine(float shakeSize, float duration) {
        originalSize = Camera.main.orthographicSize;
        float elapsedTime = 0f;

        while(elapsedTime < duration) {
            float progress = Mathf.PingPong(elapsedTime / duration, 0.5f) * 2f;
            float zoomOffset = Mathf.Lerp(0, shakeSize, progress);
            Camera.main.orthographicSize = originalSize + zoomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Camera.main.orthographicSize = originalSize;
        zoomCoroutine = null;
    }
}

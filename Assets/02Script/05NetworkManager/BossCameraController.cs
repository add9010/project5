using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class BossCameraController : MonoBehaviour
{
    public static BossCameraController Instance;

    [Header("Target")]
    public Transform target;

    [Header("Shake Settings")]
    private float shakeAmount = 0.2f;
    private float shakeTimeRemaining = 0f;

    [Header("Zoom Settings")]
    private float defaultSize;
    private Coroutine zoomRoutine;
    private Camera cam;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 followPos = new Vector3(target.position.x, target.position.y, transform.position.z);

            if (shakeTimeRemaining > 0f)
            {
                float x = Random.Range(-shakeAmount, shakeAmount);
                float y = Random.Range(-shakeAmount, shakeAmount);
                followPos += new Vector3(x, y, 0f);
                shakeTimeRemaining -= Time.deltaTime;
            }

            transform.position = followPos;
        }
    }

    public void Shake(float duration, float amount)
    {
        shakeTimeRemaining = duration;
        shakeAmount = amount;
    }

    public void ZoomIn(float targetSize, float duration)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomCoroutine(targetSize, duration));
    }

    public void ResetZoom(float duration)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(ZoomCoroutine(defaultSize, duration));
    }

    private IEnumerator ZoomCoroutine(float targetSize, float duration)
    {
        float startSize = cam.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = targetSize;
    }
}

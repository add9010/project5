using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Transform target;

    [Header("Shake")]
    public float shakeAmount = 0.2f;
    private float shakeTimeRemaining = 0f;

    [Header("Zoom")]
    private float defaultSize;
    private Coroutine zoomRoutine;

    private Camera cam;

    private void Awake()
    {

        cam = GetComponent<Camera>();

        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        defaultSize = cam.orthographicSize;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 followPos = new Vector3(target.position.x, target.position.y, transform.position.z);

        if (shakeTimeRemaining > 0f)
        {
            float shakeX = Random.Range(-shakeAmount, shakeAmount);
            float shakeY = Random.Range(-shakeAmount, shakeAmount);
            followPos += new Vector3(shakeX, shakeY, 0f);

            shakeTimeRemaining -= Time.deltaTime;
        }

        transform.position = followPos;
    }

    public void Shake(float duration = 0.1f, float amount = 0.2f)
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

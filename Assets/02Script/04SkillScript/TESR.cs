using UnityEngine;
using System.Collections;

public class TESR : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TestZoom());
    }

    IEnumerator TestZoom()
    {
        yield return new WaitForSeconds(1f);
        var cam = Camera.main?.GetComponent<CameraController>();
        if (cam != null)
        {
            Debug.Log("✅ 테스트 줌인 시도");
            cam.ZoomIn(3f, 1f);
        }
        else
        {
            Debug.LogWarning("❌ 테스트 줌인 실패: CameraController 없음");
        }
    }
}

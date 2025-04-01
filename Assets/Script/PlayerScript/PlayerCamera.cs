using UnityEngine;

public class PlayerCamera
{
    private PlayerManager manager;
    private Transform cameraTransform;
    private Vector3 offset;
    private float followSpeed;
    private float stopDistance;
    private LayerMask wallLayer;
    private bool isNearWall = false;

    public PlayerCamera(PlayerManager manager, float followSpeed, float stopDistance, LayerMask wallLayer)
    {
        this.manager = manager;
        this.followSpeed = followSpeed;
        this.stopDistance = stopDistance;
        this.wallLayer = wallLayer;

        cameraTransform = Camera.main.transform;
        offset = cameraTransform.position - manager.transform.position;
    }

    public void Update()
    {
        if (cameraTransform == null || manager == null) return;

        CheckWallProximity();

        if (!isNearWall)
        {
            Vector3 targetPos = manager.transform.position + offset;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }

    private void CheckWallProximity()
    {
        Vector3 direction = (cameraTransform.position - manager.transform.position).normalized;

        if (Physics.Raycast(manager.transform.position, direction, out RaycastHit hit, stopDistance, wallLayer))
        {
            isNearWall = true;
        }
        else
        {
            isNearWall = false;
        }
    }
}
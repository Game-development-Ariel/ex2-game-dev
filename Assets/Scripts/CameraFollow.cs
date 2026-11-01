using UnityEngine;
using UnityEngine.Tilemaps;

// Keeps the camera centered on the target, without showing anything outside the map.
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Tilemap mapBounds;
    [SerializeField] private float smoothTime = 0.15f;

    private Camera cam;
    private Bounds worldBounds;
    private Vector3 velocity;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (mapBounds != null)
        {
            mapBounds.CompressBounds();
            worldBounds = mapBounds.localBounds;
            worldBounds.center = mapBounds.transform.TransformPoint(worldBounds.center);
        }

        transform.position = GoalPosition();
    }

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.SmoothDamp(transform.position, GoalPosition(), ref velocity, smoothTime);
    }

    private Vector3 GoalPosition()
    {
        Vector3 goal = target.position;
        goal.z = transform.position.z;
        if (mapBounds == null) return goal;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        goal.x = ClampAxis(goal.x, worldBounds.min.x, worldBounds.max.x, halfWidth);
        goal.y = ClampAxis(goal.y, worldBounds.min.y, worldBounds.max.y, halfHeight);
        return goal;
    }

    private static float ClampAxis(float value, float min, float max, float halfView)
    {
        // If the map is smaller than the view on this axis, just center it.
        if (max - min < halfView * 2f) return (min + max) / 2f;
        return Mathf.Clamp(value, min + halfView, max - halfView);
    }
}

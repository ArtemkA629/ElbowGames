using ClashTheCube;
using Framework.SystemInfo;
using Framework.Variables;
using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    [SerializeField] private IntReference numPoints;
    [SerializeField] private FloatReference angleRatio;
    [SerializeField] private FloatReference timeStep;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject endPointPrefab;

    private GameObject endPoint;
    private Vector3 speed;
    private bool aimed;

    public Vector3 Speed => speed;
    public bool Aimed => aimed;

    private void Awake()
    {
        endPoint = Instantiate(endPointPrefab, transform.position, Quaternion.identity);
    }

    public void Predict(float deltaMultiplier)
    {
        aimed = true;
        var ray = Camera.main.ScreenPointToRay(GetInputPosition());
        new Plane(Vector3.up, transform.position).Raycast(ray, out float enter);
        var inputPointInWorld = ray.GetPoint(enter);
        speed = (inputPointInWorld - transform.position) * deltaMultiplier;
        DrawTrajectory();
    }

    public void OnObjectThrow()
    {
        aimed = false;
        ResetTrajectory();
        Destroy(endPoint);
    }

    private void DrawTrajectory()
    {
        speed += Vector3.up * angleRatio;
        Vector3 position = transform.position;
        Vector3 nextPosition;

        UpdateLineRender(numPoints, (0, position));

        for (int i = 1; i < numPoints; i++)
        {
            float time = i * timeStep;
            nextPosition = transform.position + speed * time + time * time * Physics.gravity / 2f;

            var results = new Collider[1];
            float radius = 0.5f;
            int collidersCount = Physics.OverlapSphereNonAlloc(position, radius, results);
            var collider = results[0];
            if (collidersCount > 0 && collider.TryGetComponent<FieldObjectBase>(out _) == false && collider.gameObject != endPoint)
            {
                var closestPoint = collider.ClosestPoint(position);
                UpdateLineRender(i, (i - 1, closestPoint));
                MoveHitMarker(closestPoint);
                break;
            }

            endPoint.SetActive(false);
            position = nextPosition;
            UpdateLineRender(numPoints, (i, position));
        }
    }

    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
    {
        lineRenderer.positionCount = count;
        lineRenderer.SetPosition(pointPos.point, pointPos.pos);
    }

    private void MoveHitMarker(Vector3 position)
    {
        endPoint.SetActive(true);
        endPoint.transform.position = position;
    }

    private void ResetTrajectory()
    {
        lineRenderer.positionCount = 0;
    }

    private Vector2 GetInputPosition()
    {
        return Platform.IsMobilePlatform() ? Input.GetTouch(0).position : Input.mousePosition;
    }
}

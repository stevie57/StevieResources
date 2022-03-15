using UnityEngine;

[SelectionBase]
public class Launcher : MonoBehaviour
{
    [SerializeField]
    private BezierCurve _bezierCurve;
    [SerializeField]
    private int _lineSteps;
    [SerializeField]
    private Vector3[] _trajectoryPositions;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _spawn;
    [SerializeField]
    private Vector3 debugVector;
    [SerializeField]
    private float _projectileSpeed;
    
    [SerializeField] [Range(0,1)]
    private float _distanceMaxHeight = 0.75f;

    [SerializeField]
    private ProjectileController _projectile;

    private Vector3[] GetTrajectoryPath()
    {
        SetBezierPoints();
        _trajectoryPositions = new Vector3[_lineSteps];
        for(int i = 1; i <= _lineSteps; i++)
        {       
            Vector3 position = _bezierCurve.GetPoint(i / (float)_lineSteps);
            _trajectoryPositions[i - 1] = position;
        }

        return _trajectoryPositions;
    }

    private void SetBezierPoints()
    {
        Vector3 startPos = _spawn.position; 
        Vector3 endPos = _target.position;
        Vector3 direction = (endPos - startPos).normalized;
        Vector3 midPoint = transform.position + direction * Vector3.Distance(startPos, endPos) * _distanceMaxHeight;
        midPoint.y = Vector3.Distance(startPos, endPos);

        _bezierCurve.points[0] = transform.InverseTransformPoint(startPos);
        _bezierCurve.points[1] = transform.InverseTransformPoint(midPoint);
        _bezierCurve.points[2] = transform.InverseTransformPoint(endPos);
    }

    [ContextMenu("Fire Projectile")]
    public void FireProjectile()
    {
        ProjectileController projectile = Instantiate(_projectile);
        Vector3[] curvePos = GetTrajectoryPath();
        projectile.LaunchProjectile(curvePos, _projectileSpeed, _spawn.position);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3[] curve = GetTrajectoryPath();
        for(int i = 0; i < curve.Length; i++)
        {
            Gizmos.DrawSphere(curve[i], 0.1f);
        }
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ProjectileController : MonoBehaviour
{
    private Vector3[] _curvePos;
    private int _currentIndex;
    private float _speed;
    private float _step;

    public void LaunchProjectile(Vector3[] CurvePos, float speed, Vector3 spawnPos)
    {
        transform.position = spawnPos;
        _curvePos = CurvePos;
        _speed = speed;
        _currentIndex = 0;
        _step = _speed * Time.deltaTime;
        StartCoroutine(FollowCurve());
    }

    private IEnumerator FollowCurve()
    {
        while (_currentIndex < _curvePos.Length)
        {
            Vector3 targetPos = _curvePos[_currentIndex + 1];
            transform.LookAt(targetPos);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _step);
            if (Vector3.Distance(transform.position, targetPos) < 0.25f)
            {
                _currentIndex++;
                if (_currentIndex + 1 >= _curvePos.Length) break;
            }                
            yield return null;
        }
    }
}

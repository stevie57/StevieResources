using UnityEngine;
public struct SineWaveSettings
{
    public float MaxAmplitude;
    public float MinAmplitude;

    public float MinFrequency;
    public float MaxFrequency;

    public float MinMovement;
    public float MaxMovement;
}
public class Sinewave : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _myLineRenderer;
    [Header("Smoothness")]
    [SerializeField]
    private int _points;

    [Header("Wave Values")]
    [Range(-.25f, .25f)]
    [SerializeField]
    private float _amplitude = 0.2f;
    private float _maxAmplitude = .25f;
    private float _minAmplitude = -.25f;
    [Range(2f,4f)]
    [SerializeField]
    private float _frequency = 3f;
    private float _minFrequency = 2f;
    private float _maxFrequency = 4f;
    [Range(2f, 3f)]
    [SerializeField]
    private float _movementSpeed = 1;
    private float _minMovement = 2f;
    private float _maxMovement = 3f;

    [Header("Line Graph Length")]
    [SerializeField]
    private Vector2 _graphLimitX = new Vector2(0, 1);

    [Header("World Position")]
    [SerializeField]
    private Transform _offset;

    [Range(0, 2 * Mathf.PI)]
    [SerializeField]
    private float radians;
    
    private void Start()
    {
        _myLineRenderer = GetComponent<LineRenderer>();
    }

    public void Initialize(SineWaveSettings settings)
    {
        _maxAmplitude = settings.MaxAmplitude;
        _minAmplitude = settings.MinAmplitude;
        _maxFrequency = settings.MaxFrequency;
        _minFrequency = settings.MinAmplitude;
        _minMovement = settings.MinMovement;
        _maxMovement = settings.MaxMovement;
    }

    private void Draw()
    {
        float xStart = _graphLimitX.x;
        float Tau = 2 * Mathf.PI;
        float xFinish = _graphLimitX.y;

        _myLineRenderer.positionCount = _points;

        for (int currentPoint = 0; currentPoint < _points; currentPoint++)
        {
            float progress = (float)currentPoint / (_points - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = (_amplitude * Mathf.Sin((Tau * _frequency * x) + (Time.timeSinceLevelLoad * _movementSpeed)));
            Vector3 graphResult = new Vector3(x, y, 0);
            graphResult += _offset.position;
            _myLineRenderer.SetPosition(currentPoint, graphResult);
        }
    }

    private void Update()
    {
        Draw();
    }

    /// <summary>
    /// Returns float[] of amplitude, frequency and movement speed
    /// </summary>
    /// <returns></returns>
    public float[] GetValues()
    {
        float[] values = new float[3] { _amplitude, _frequency, _movementSpeed };
        return values;
    }

    public void AdjustAmplitude(float amplitudeAdjustment)
    {
        float newValue = amplitudeAdjustment;
        _amplitude = CheckValueLimits(_minAmplitude, _maxAmplitude, newValue);
    }

    private float CheckValueLimits(float minValue, float MaxValue, float currentValue)
    {
        if(currentValue > MaxValue ) return MaxValue;
        if (currentValue < minValue) return minValue;
        return currentValue;
    }

    public void AdjustFrequency(float frequencyAdjustment)
    {
        float newValue = Mathf.Lerp(_minFrequency, _maxFrequency, frequencyAdjustment);
        _frequency = newValue;
    }

    public void AdjustMovement(float movementAdjustment)
    {
        float newValue = Mathf.Lerp(_minMovement, _maxMovement, movementAdjustment);
        _movementSpeed = newValue;
    }

   
}
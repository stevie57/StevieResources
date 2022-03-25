using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingPuzzle : PlayerFeature
{
    [Header("References")]
    [SerializeField]
    private Sinewave _solutionWave;
    [SerializeField]
    private Sinewave _playerWave;
    [SerializeField]
    private Throttle _leftLever;
    [SerializeField]
    private Throttle _rightLever;

    [Header("Puzzle Height")]
    [SerializeField]
    private float _maxAmplitude = .25f;
    [SerializeField]
    private float _minAmplitude = -.25f;

    [Header("Puzzle Density")]
    [SerializeField]
    private float _minFrequency = 2f;
    [SerializeField]
    private float _maxFrequency = 4f;

    [Header("Puzzle Velocity")]
    [SerializeField]
    private float _minMovement = 2f;
    [SerializeField]
    private float _maxMovement = 3f;

    [Header("Solution Sensitivity")]
    [SerializeField]
    private float _solutionLeniency;

    private void SetupWaves()
    {
        SineWaveSettings settings = new SineWaveSettings();
        settings.MaxAmplitude = _maxAmplitude;
        settings.MinAmplitude = _minAmplitude;
        settings.MinFrequency = _minFrequency;
        settings.MaxFrequency = _maxFrequency;
        settings.MinMovement = _minMovement;
        settings.MaxMovement = _maxMovement;

        _solutionWave.Initialize(settings);
        _playerWave.Initialize(settings);
    }

    private void OnEnable()
    {
        _leftLever.OnThrottleValueChanged.AddListener(NewFrequency);
        _rightLever.OnThrottleValueChanged.AddListener(NewMovement);
    }

    private void OnDisable()
    {
        _leftLever.OnThrottleValueChanged.RemoveListener(NewFrequency);
        _rightLever.OnThrottleValueChanged.AddListener(NewMovement);
    }

    private void Start()
    {
        SetupWaves();
        DeactivateHackingPuzzle();
    }

    protected override void OnPlayerFeatureStateChanged(States state)
    {
        switch (state)
        {
            case States.Inactive:
                DeactivateHackingPuzzle();
                break;
            case States.Locked:
                DeactivateHackingPuzzle();
                break;
            case States.Unlocked:
                ActivateHackingPuzzle();
                break;
        }
    }

    private void DeactivateHackingPuzzle()
    {
        _solutionWave.gameObject.SetActive(false);
        _playerWave.gameObject.SetActive(false);
    }

    private void ActivateHackingPuzzle()
    {
        _solutionWave.gameObject.SetActive(true);
        _playerWave.gameObject.SetActive(true);
    }

    private void NewFrequency(float value)
    {
        _playerWave.AdjustFrequency(value);
        CheckPlayerValues();
    }

    private void NewMovement(float value)
    {
        _playerWave.AdjustMovement(value);
        //CheckPlayerValues();
    }

    private void CheckPlayerValues()
    {
        float[] solutionValues = _solutionWave.GetValues();
        float[] playerValues = _playerWave.GetValues();

        bool isCorrect = false;

        if (playerValues[1] < solutionValues[1] + _solutionLeniency)
            isCorrect = true;

        if (playerValues[1] > solutionValues[1] - _solutionLeniency)
            isCorrect = true;
        else
            isCorrect = false;


        if (playerValues[2] < solutionValues[2] + _solutionLeniency)
            isCorrect = true;
        else
            isCorrect = false;

        if (playerValues[2] > solutionValues[2] - _solutionLeniency)
            isCorrect = true;
        else
            isCorrect = false;

        print($"isCorrect = {isCorrect}");

        //for(int i = 1; i < solutionValues.Length; i++)
        //{
        //    if (playerValues[i] < solutionValues[i] + _solutionLeniency)
        //        isCorrect = false;
        //    if (playerValues[i] > solutionValues[i] - _solutionLeniency)
        //        isCorrect = false;
        //}

        //if (isCorrect)
        //    print($"Player has solved the hacking puzzle");
        //else
        //{
        //    print($"Solution is {solutionValues[1]} and player value is {playerValues[1]}");
        //}
    }
}

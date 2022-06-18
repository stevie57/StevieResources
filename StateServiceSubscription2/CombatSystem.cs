using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour, IStateListener<GameState_Play>
{
    [SerializeField] private GameObject _playCube;

    private void Start()
    {
        GameStateManager.Instance.Bind(this);    
    }

    public void Setup(GameState_Play gameState)
    {
        print($"CombatSystem is setting up {gameState}");
        _playCube.SetActive(true);
    }

    public void TearDown(GameState_Play gameState)
    {
        print($"CombatSystem is tearing down {gameState}");
        _playCube.SetActive(false);
    }
}

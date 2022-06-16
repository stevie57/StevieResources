using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour, IStateListener<GameState_Play>
{
    private void Start()
    {
        GameStateManager.Instance.Bind(this);    
    }

    public void Setup(GameState_Play gameState)
    {
        print($"Setup has been called");
    }

}

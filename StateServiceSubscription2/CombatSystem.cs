using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour, IStateListener<GameState_Play>, IStateListener<GameState_Setup>
{
    private void Start()
    {
        GameStateManager.Instance.Bind(this);    
    }

    public void Setup(GameState_Play gameState)
    {
        print($"CombatSystem is setting up {gameState.ToString()}");
    }

    public void Setup(GameState_Setup gameState)
    {
        print($"CombatSystem is setting up {gameState.ToString()}");
    }
}

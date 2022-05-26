using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour, IStateListener<GameState_Play>
{
    public void Bind()
    {
        
    }

    void Start()
    {
        var methods = typeof(CombatSystem).GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        print($"methods length is {methods.Length}");
        print($"method is {methods[0].Name}");
        print($"Is it generic = {methods[0].IsGenericMethod} ");

        GameStateManager.Instance.Add(this);
    }

    private void SetUp()
    {

    }

    private void TearDown()
    {

    }
}

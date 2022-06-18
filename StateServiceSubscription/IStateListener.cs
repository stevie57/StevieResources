using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateListener<T> where T : AGameState
{
    public void Setup(T gameState);

    public void TearDown(T gameState);
}

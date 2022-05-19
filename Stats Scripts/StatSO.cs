using UnityEngine;

public abstract class StatSO : ScriptableObject
{
    public int Value;
    public int Max;
    public abstract string GetStatName();
}
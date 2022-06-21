using UnityEngine;

public class StatsTester : MonoBehaviour
{
    private Actor _actor;
    [SerializeField]
    private string _statType;
    [SerializeField]
    private PlayerStatType _playerStatType;

    [SerializeField]
    private int _value;

    private void Awake()
    {
        _actor = GetComponent<Actor>();
    }

    [ContextMenu("Increase Stat")]
    public void IncreaseStat()
    {
        if (!CheckStatAvailable())
            return;

        _actor.Stats[_statType].Value += _value;
        print($" {_statType} is {_actor.Stats[_statType].Value}");
    }

    [ContextMenu("Decrease Stat")]
    public void DecreaseStat()
    {
        if (!CheckStatAvailable())
            return;

        _actor.Stats[_statType].Value -= _value;
        print($" {_statType} is {_actor.Stats[_statType].Value}");
    }

    private bool CheckStatAvailable()
    {
        return _actor.Stats[_statType] != null ? true : false;
    }
}

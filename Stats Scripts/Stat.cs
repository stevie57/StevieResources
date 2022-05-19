using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat 
{
    public string Name { get; set; }
    public int Value
    {
        get => _value;
        set
        {
            if (value < 0)
                _value = 0;
            _value = value > Max ? Max : value;
        }
    }

    private int _value;
    public int Max { get; set; }

    public void Fill() => Value = Max;
    public void Empty() => Value = 0;
    public void IncreaseBy(float percent) => Max = (int)(Max * percent);
    public void DecreaseBy(float percent) => Max = (int)(Max * percent);
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : IEnumerable<Stat>
{
    private List<Stat> _stats;

    public Stats() => _stats = new List<Stat>();

    public IEnumerator<Stat> GetEnumerator()
    {
        return _stats.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Stat this[string name]
    {
        get
        {
            Stat result = CheckForStat(name);
            if (result != null)
                return result;
            else
            {
                Debug.Log($"Stat [{name}] does not exist ");
                return null;
            }
        }
        set
        {
            Stat result = CheckForStat(name);

            if(result != null)
            {
                result.Value = value.Value;
                result.Max = value.Max;
            }
        }
    }

    private Stat CheckForStat(string name)
    {
        for (int i = 0; i < _stats.Count; i++)
        {
                if (_stats[i].Name == name)
            {
                return _stats[i];
            }
        }

        return null;
    }

    public void Add(string name, int max, int value) => _stats.Add(new Stat { Name = name, Max = max, Value = value });
}

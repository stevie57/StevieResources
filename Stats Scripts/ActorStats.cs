using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =("Stats/ActorStats"))]
[System.Serializable]
public class ActorStats : ScriptableObject
{
    public List<StatSO> statsList = new List<StatSO>();

    public Stats CreateStats()
    {
        Stats actorStats = new Stats();

        for(int i = 0; i < statsList.Count; i++)
        {
            actorStats.Add(statsList[i].GetStatName(), statsList[i].Max, statsList[i].Value);                       
        }
        return actorStats;
    }
}

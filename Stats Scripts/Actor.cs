using UnityEngine;

public class Actor : MonoBehaviour
{
    public Stats Stats;
    [SerializeField] 
    private ActorStats _actorStats;

    private void Awake()
    {
        Stats = _actorStats.CreateStats();     
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = ("Stats/PlayerStats"))]
public class PlayerStats : StatSO
{
    public StatTypeEnum StatType;
    public override string GetStatName()
    {
        return StatType.ToString();
    } 
}
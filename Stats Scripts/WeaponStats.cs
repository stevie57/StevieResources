using UnityEngine;

[CreateAssetMenu(menuName = ("Stats/WeaponStats"))]
public class WeaponStats : StatSO
{
    public WeaponStats StatType;
    public override string GetStatName()
    {
        return StatType.ToString();
    }
}
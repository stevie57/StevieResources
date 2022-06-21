using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = ("Stats/PlayerStats"))]
public class PlayerStats : StatSO
{
    public PlayerStatTypesEnum PlayerStatType;

    // used for updating the enum
    private string _filePath = "/Scripts/Stats/";
    private protected string _scriptName = "PlayerStatTypesEnum";

    public override string GetStatName()
    {
        return PlayerStatType.ToString();
    }

    [ContextMenu("Update Player Stats Enum")]
    public void UpdateStatsEnum()
    {
        string savePath = Application.dataPath + _filePath + _scriptName + ".cs";

        using (StreamWriter newWriter = new StreamWriter(savePath, false))
        {
            #region CreateSpace
            void CreateSpace()
            {
                newWriter.WriteLine();
            }
            #endregion 

            newWriter.WriteLine($"public enum PlayerStatTypesEnum");
            newWriter.Write("{");
            CreateSpace();
            List<Type> playerStatTypes = GetPlayerStatTypes();
            foreach (Type type in playerStatTypes)
            {
                newWriter.WriteLine($"  {type.Name},");
            }
            newWriter.WriteLine("}");
        }
        Debug.Log($"Player Stat Types Enum Generated and saved at : {savePath}");
    }

    private List<Type> GetPlayerStatTypes()
    {
        List<Type> statTypeList = new List<Type>();
        var StatTypes = Assembly.GetAssembly(typeof(PlayerStatType)).GetTypes();

        foreach (Type statType in StatTypes)
        {
            if (statType.IsClass && !statType.IsAbstract && statType.IsSubclassOf(typeof(PlayerStatType)))
            {
                statTypeList.Add(statType);
            }
        }
        return statTypeList;
    }
}
using UnityEditor;
[InitializeOnLoad]
public static class CheckmarkMenuItem
{
     private const string MenuName = "My Menu Item";
 
     public static bool isEnabled;
 
     static MyMenu()
     {
         isEnabled = EditorPrefs.GetBool(MenuName, true);
     }
 
     [MenuItem(MenuName)]
     private static void ToggleAction()
     {
         isEnabled = !isEnabled;
         EditorPrefs.SetBool(MenuName, isEnabled);
     }
 
     [MenuItem(MenuName, true)]
     private static bool ToggleActionValidate()
     {
         Menu.SetChecked(MenuName, isEnabled);
         return true;
    }
}


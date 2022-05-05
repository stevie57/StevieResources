using UnityEditor;
[InitializeOnLoad]
public static class CheckmarkMenuItem
{

    private const string MENU_NAME = "Example/Toggle";

    private static bool enabled_;
    /// Called on load thanks to the InitializeOnLoad attribute
    static CheckmarkMenuItem()
    {
        CheckmarkMenuItem.enabled_ = EditorPrefs.GetBool(CheckmarkMenuItem.MENU_NAME, false);

        /// Delaying until first editor tick so that the menu
        /// will be populated before setting check state, and
        /// re-apply correct action
        EditorApplication.delayCall += () => {
            PerformAction(CheckmarkMenuItem.enabled_);
        };
    }

    [MenuItem(CheckmarkMenuItem.MENU_NAME)]
    private static void ToggleAction()
    {

        /// Toggling action
        PerformAction(!CheckmarkMenuItem.enabled_);
    }

    public static void PerformAction(bool enabled)
    {

        /// Set checkmark on menu item
        Menu.SetChecked(CheckmarkMenuItem.MENU_NAME, enabled);
        /// Saving editor state
        EditorPrefs.SetBool(CheckmarkMenuItem.MENU_NAME, enabled);

        CheckmarkMenuItem.enabled_ = enabled;

        /// Perform your logic here...
    }
}
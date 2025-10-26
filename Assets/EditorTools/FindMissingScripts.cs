using UnityEngine;
using UnityEditor;

// Based on:
// https://web.archive.org/web/20130514114424/http://wiki.unity3d.com/index.php/FindMissingScripts
// modified by Joren Van Borm

public static class FindMissingScriptsRecursively
{
    static int go_count = 0, components_count = 0, missing_count = 0;

    // validator function (menu item will be greyed out if this returns False)
    [MenuItem("Tools/Argo's Grab Bag/Find Missing Scripts", validate = true)] // standard grab bag location
    [MenuItem("GameObject/Find Missing Scripts", validate = true)] // adds to menu bar and context menu
    static bool ValidateMenuItem()
    {
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem("Tools/Argo's Grab Bag/Find Missing Scripts")] // standard grab bag location
    [MenuItem("GameObject/Find Missing Scripts")] // adds to menu bar and context menu
    static void FindMissingScripts(MenuCommand command)
    {
        // When invoked from menu bar, FindInSelected() runs once without context.
        // When invoked from context menu, it runs once for each selected item (setting it as its context)

        // only run the search once, across all selected objects
        // (do NOT compare to Selection.activeGameObject as it may be null)
        if (command == null || command.context == Selection.gameObjects[0])
        {
            FindInSelected();
        }
    }

    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing scripts", go_count, components_count, missing_count));
    }

    private static void FindInGO(GameObject g)
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                Debug.Log(g.name + " has an empty script attached in position: " + i, g);
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject);
        }
    }
}

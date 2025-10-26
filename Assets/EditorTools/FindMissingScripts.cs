using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

// Based on:
// https://web.archive.org/web/20130514114424/http://wiki.unity3d.com/index.php/FindMissingScripts
// modified by Joren Van Borm

public static class FindMissingScriptsRecursively
{
    // pass-by-reference data class to track search stats
    private class SearchResults
    {
        public int go_count = 0, components_count = 0, missing_count = 0;
        public HashSet<GameObject> searched = new();
    }

    // validator function (menu item will be greyed out if this returns False)
    [MenuItem("Tools/Argo's Grab Bag/Find Missing Scripts", validate = true)] // standard grab bag location
    [MenuItem("GameObject/Find Missing Scripts", validate = true)] // adds to menu bar and context menu
    static bool ValidateMenuItem()
    {
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem("Tools/Argo's Grab Bag/Find Missing Scripts")] // standard grab bag location
    [MenuItem("GameObject/Find Missing Scripts")] // adds to menu bar and context menu
    static void MenuItem(MenuCommand command)
    {
        // When invoked from menu bar, MenuItem() runs once without context.
        // When invoked from context menu, it runs once for each selected item (setting that item as its context)

        // only run the search once, across all selected objects
        // (do NOT compare to Selection.activeGameObject as it may be null)
        if (command.context == null || command.context == Selection.gameObjects[0])
        {
            FindInSelected();
        }
    }

    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        SearchResults results = new();
        Debug.LogFormat("Start search of [{0}]", string.Join(", ", go.Select(x => $"\"{x.name}\"")));
        foreach (GameObject g in go)
        {
            FindInGO(g, results);
        }
        Debug.LogFormat
        (
            "Searched {0} GameObjects, {1} components, found {2} missing scripts (see warnings above)",
            results.go_count,
            results.components_count,
            results.missing_count
        );
    }

    private static void FindInGO(GameObject g, SearchResults results)
    {
        if (results.searched.Contains(g))
        {
            // if this GO has been searched already, so have all it's children
            return;
        }

        results.searched.Add(g);

        results.go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            results.components_count++;
            if (components[i] == null)
            {
                results.missing_count++;
                Debug.LogWarning(g.name + " has an empty script attached in position: " + i, g);
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject, results);
        }
    }
}

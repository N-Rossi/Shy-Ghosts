using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// Editor tool: replaces one tile with another in every room prefab at once.
// Tools > Shy Ghosts > Swap Tiles In All Rooms. Lives in an Editor folder, so it's never in the build.
public class TileSwapWindow : EditorWindow
{
    private const string RoomsFolder = "Assets/Prefabs/Rooms";

    private TileBase from;
    private TileBase to;

    [MenuItem("Tools/Shy Ghosts/Swap Tiles In All Rooms")]
    private static void Open() => GetWindow<TileSwapWindow>("Swap Tiles");

    private void OnGUI()
    {
        EditorGUILayout.HelpBox($"Replaces every '{(from ? from.name : "from")}' tile with '{(to ? to.name : "to")}' in all prefabs under {RoomsFolder}. Commit first so you can undo it with git.", MessageType.Info);
        from = (TileBase)EditorGUILayout.ObjectField("Replace", from, typeof(TileBase), false);
        to = (TileBase)EditorGUILayout.ObjectField("With", to, typeof(TileBase), false);

        using (new EditorGUI.DisabledScope(from == null || to == null || from == to))
        {
            if (GUILayout.Button("Swap in all rooms")) Swap();
        }
    }

    private void Swap()
    {
        int changedPrefabs = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { RoomsFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            bool changed = false;

            foreach (Tilemap tilemap in root.GetComponentsInChildren<Tilemap>(true))
            {
                if (!tilemap.ContainsTile(from)) continue;
                tilemap.SwapTile(from, to);
                changed = true;
            }

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(root, path);
                changedPrefabs++;
            }
            PrefabUtility.UnloadPrefabContents(root);
        }

        Debug.Log($"Swap Tiles: replaced {from.name} with {to.name} in {changedPrefabs} room prefab(s).");
    }
}

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// Editor tool for changes across every room prefab at once:
// swap one tile for another, and set the tilemap tint colors so all rooms match.
// Tools > Shy Ghosts > Room Tools. Lives in an Editor folder, so it's never in the build.
public class TileSwapWindow : EditorWindow
{
    private const string RoomsFolder = "Assets/Prefabs/Rooms";

    private TileBase from;
    private TileBase to;
    private string onlyTilemapNamed = "";

    private Color solidColor = Color.white;
    private Color backgroundColor = Color.white;

    [MenuItem("Tools/Shy Ghosts/Room Tools")]
    private static void Open() => GetWindow<TileSwapWindow>("Room Tools");

    private void OnGUI()
    {
        EditorGUILayout.HelpBox($"These change every prefab under {RoomsFolder}. Commit first so you can undo with git.", MessageType.Info);

        EditorGUILayout.LabelField("Swap tiles", EditorStyles.boldLabel);
        from = (TileBase)EditorGUILayout.ObjectField("Replace", from, typeof(TileBase), false);
        to = (TileBase)EditorGUILayout.ObjectField("With", to, typeof(TileBase), false);
        onlyTilemapNamed = EditorGUILayout.TextField(new GUIContent("Only tilemaps named", "e.g. Solid or Background. Empty = every tilemap."), onlyTilemapNamed);
        using (new EditorGUI.DisabledScope(from == null || to == null || from == to))
        {
            if (GUILayout.Button("Swap in all rooms")) Swap();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tilemap colors", EditorStyles.boldLabel);
        solidColor = EditorGUILayout.ColorField("Solid", solidColor);
        backgroundColor = EditorGUILayout.ColorField("Background", backgroundColor);
        if (GUILayout.Button("Set colors in all rooms")) SetColors();
    }

    private void Swap()
    {
        int changed = ForEachRoomTilemap(tilemap =>
        {
            if (onlyTilemapNamed.Length > 0 && tilemap.name != onlyTilemapNamed) return false;
            if (!tilemap.ContainsTile(from)) return false;
            tilemap.SwapTile(from, to);
            return true;
        });
        Debug.Log($"Room Tools: replaced {from.name} with {to.name} in {changed} room prefab(s).");
    }

    private void SetColors()
    {
        int changed = ForEachRoomTilemap(tilemap =>
        {
            Color target;
            if (tilemap.name == "Solid") target = solidColor;
            else if (tilemap.name == "Background") target = backgroundColor;
            else return false;

            if (tilemap.color == target) return false;
            tilemap.color = target;
            return true;
        });
        Debug.Log($"Room Tools: set tilemap colors in {changed} room prefab(s).");
    }

    // Runs edit on every tilemap in every room prefab; saves the prefabs where it returned true.
    private static int ForEachRoomTilemap(Func<Tilemap, bool> edit)
    {
        int changedPrefabs = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { RoomsFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);

            bool changed = false;
            foreach (Tilemap tilemap in root.GetComponentsInChildren<Tilemap>(true))
                changed |= edit(tilemap);

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(root, path);
                changedPrefabs++;
            }
            PrefabUtility.UnloadPrefabContents(root);
        }
        return changedPrefabs;
    }
}

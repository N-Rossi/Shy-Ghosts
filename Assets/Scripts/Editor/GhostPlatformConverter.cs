using UnityEditor;
using UnityEngine;

// One-time editor tool: ghost platforms were sized with Transform scale, which stretches real art.
// This converts every one in the room prefabs (and the GhostPlatform prefab itself) to scale 1
// with a Tiled sprite of the same size, so the art repeats along the platform instead.
// Tools > Shy Ghosts > Convert Ghost Platforms To Tiled. Safe to run twice: converted ones are skipped.
public static class GhostPlatformConverter
{
    private const string RoomsFolder = "Assets/Prefabs/Rooms";
    private const string GhostPrefabPath = "Assets/Prefabs/GhostPlatform.prefab";

    [MenuItem("Tools/Shy Ghosts/Convert Ghost Platforms To Tiled")]
    private static void Convert()
    {
        if (!EditorUtility.DisplayDialog("Convert ghost platforms",
                "Converts every ghost platform in the room prefabs to a Tiled sprite at scale 1, keeping its size.\n\n" +
                "Apply all room overrides in the scene and commit first, so you can undo with git.",
                "Convert", "Cancel"))
            return;

        int converted = 0;

        // Rooms first: their platforms still read the old prefab scale (3 x 0.5) where not overridden.
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { RoomsFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);

            int before = converted;
            foreach (GhostPlatform platform in root.GetComponentsInChildren<GhostPlatform>(true))
                if (ConvertOne(platform.gameObject)) converted++;

            if (converted > before) PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
        }

        // Then the source prefab, so new platforms dragged in start correct.
        GameObject ghostRoot = PrefabUtility.LoadPrefabContents(GhostPrefabPath);
        if (ConvertOne(ghostRoot))
        {
            PrefabUtility.SaveAsPrefabAsset(ghostRoot, GhostPrefabPath);
            converted++;
        }
        PrefabUtility.UnloadPrefabContents(ghostRoot);

        Debug.Log($"Ghost platforms: converted {converted}.");
    }

    private static bool ConvertOne(GameObject platform)
    {
        var sprite = platform.GetComponent<SpriteRenderer>();
        var box = platform.GetComponent<BoxCollider2D>();
        Vector3 scale = platform.transform.localScale;

        if (sprite == null || box == null) return false;
        if (sprite.drawMode == SpriteDrawMode.Tiled && scale == Vector3.one) return false;

        Vector2 size = new Vector2(Mathf.Abs(scale.x) * box.size.x, Mathf.Abs(scale.y) * box.size.y);

        platform.transform.localScale = Vector3.one;
        sprite.drawMode = SpriteDrawMode.Tiled;
        sprite.tileMode = SpriteTileMode.Continuous;
        sprite.size = size;
        box.size = size;
        box.offset = Vector2.zero;
        return true;
    }
}

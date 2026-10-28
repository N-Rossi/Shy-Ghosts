using UnityEngine;
using UnityEngine.Tilemaps;

// Spreads one larger texture across a block of grid cells. A 32x32 texture at 16 px per unit
// covers 2x2 tiles, so its pixels stay the same size as the character's. Paint it like any tile.
[CreateAssetMenu(fileName = "PatternTile", menuName = "Shy Ghosts/Pattern Tile")]
public class PatternTile : TileBase
{
    [Tooltip("Pattern size in cells. 32x32 texture at 16 PPU = 2 x 2.")]
    public Vector2Int size = new Vector2Int(2, 2);
    [Tooltip("The texture's slices in Unity's slice order: left to right, top row first.")]
    public Sprite[] sprites;

    [Header("Variation (optional)")]
    [Tooltip("Slices of a second texture with the same layout, e.g. a cracked version of the bricks.")]
    public Sprite[] alternateSprites;
    [Tooltip("Chance that a whole block uses the alternate texture. Fixed per position, so it never flickers.")]
    [Range(0f, 1f)] public float alternateChance = 0.15f;

    public Tile.ColliderType colliderType = Tile.ColliderType.Grid;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        Sprite[] set = UseAlternate(position) ? alternateSprites : sprites;
        if (set == null || set.Length == 0) return;

        int x = Mod(position.x, size.x);
        int rowFromTop = size.y - 1 - Mod(position.y, size.y);
        int index = Mathf.Min(rowFromTop * size.x + x, set.Length - 1);

        tileData.sprite = set[index];
        tileData.color = Color.white;
        tileData.transform = Matrix4x4.identity;
        tileData.flags = TileFlags.LockTransform;
        tileData.colliderType = colliderType;
    }

    private bool UseAlternate(Vector3Int position)
    {
        if (alternateSprites == null || alternateSprites.Length == 0 || alternateChance <= 0f) return false;

        // Same answer for every cell in a block, so the alternate texture stays whole.
        int blockX = Mathf.FloorToInt((float)position.x / size.x);
        int blockY = Mathf.FloorToInt((float)position.y / size.y);
        uint hash = (uint)(blockX * 73856093 ^ blockY * 19349663);
        hash ^= hash >> 13;
        hash *= 0x5bd1e995;
        hash ^= hash >> 15;
        return (hash % 1000) / 1000f < alternateChance;
    }

    private static int Mod(int value, int divisor) => ((value % divisor) + divisor) % divisor;
}

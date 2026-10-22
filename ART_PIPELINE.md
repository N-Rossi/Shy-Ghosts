# Art pipeline (Shy Ghosts)

Checklist for swapping placeholder squares for real pixel art. All art must be hand-made or licensed (no AI art); log every external asset in CREDITS.md.

## Rules for every sprite
- 16 px = 1 tile = 1 world unit.
- Import: Pixels Per Unit 16, Filter Mode Point, Compression None (the `PixelSprite` default preset does this).
- Spritesheets: Sprite Mode Multiple, then Sprite Editor → Slice → Grid By Cell Size.
- Draw at normal brightness. Darkness comes from the lights, not dark paint.

## Sorting layers (back to front)
| Layer | Used by |
|---|---|
| Background | `Background` tilemap (wallpaper, bricks, windows) |
| Level | `Solid` tilemap, ghost platforms |
| Props | candles, decorations |
| Player | player, lantern sprite |
| Foreground | anything drawn in front of the player |
| UI | battery bar canvas |

Every Light 2D's **Target Sorting Layers** must include all layers except UI. Re-check after adding a new sorting layer.

## Swapping in tiles
1. Import the tile sheet into `Assets/Art`, then slice it.
2. Open the Rule Tile (`Assets/Tiles`) and set its Default Sprite, or add rules for edges and corners.
   - Background variety: add a rule with Output = Random and several sprites, so the wallpaper doesn't repeat visibly.
3. For a new look (e.g. outdoor bricks), duplicate a Rule Tile, swap its sprites, and add it to the `Manor` palette. Paint it on the same `Background` tilemap.
4. Set the tilemap **Color** back to white (`FFFFFF`). The placeholder tints (`Background` 5A5068, `Solid` A8A0B5) are only there for the gray squares.

## Swapping in object sprites
Open the prefab (`Assets/Prefabs`), select the object, and drag the new sprite into the Sprite Renderer's **Sprite** field. Check the collider still matches the visible shape. Every copy in every room updates.

## Optional: normal maps
2D lights can use a normal map so bricks and wood catch the lantern light. Add it in the Sprite Editor → Secondary Textures, name `_NormalMap`. Only if time allows on the art day.

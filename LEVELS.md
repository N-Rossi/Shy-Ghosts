# Level layouts (Shy Ghosts)

Rooms are 30 x 17 tiles. Tile coordinates are relative to the room root (its bottom-left corner): x 0–29, y 0–16.
Movement reference: jump about 3.3 tiles high; a running jump clears about 6 tiles on flat ground.
A gap of 7+ tiles cannot be jumped without a ghost platform.

## Room 1-01 "Look away" (teaches the core rule)

Root position (0, 0). Battery: Use Fixed Start Charge on, 1.0; Battery Drain Multiplier 0.

```
 y   x: 0         1         2
        012345678901234567890123456789
16      ##############################
15      #                            #
14      #                            #
13      #                            #
12      #                            #
11      #                            #
10      #                            #
 9      #                               <- doorway: x 29, y 7-9 open
 8      #
 7      #
 6      #                          ###  <- exit ledge x 27-28, top at y 7
 5      #                          ###
 4      #  S                  ===  ###  <- G2 (x 22-24), top at y 5
 3      #########  ===   ####      ###  <- G1 (x 11-13), top at y 4, flush with floor
 2      #########        ####      ###
 1      #####################      ###
 0      #####################      ###
                             ^^^^^^ x 21-26: deadly pit, open to the bottom

# solid tile   = ghost platform (half a tile thick)   S spawn
```

Read the bottom rows as: floor x 1–8 (rows 0–3), shallow trough x 9–16 (floor rows 0–1 only),
middle floor x 17–20 (rows 0–3), pit x 21–26 (empty, open bottom), exit ledge x 27–28 (rows 0–6), wall x 29 (rows 0–6).

| Object | Position (center) | Notes |
|---|---|---|
| Spawn | (3, 4) | On the left floor |
| GhostPlatform G1 | (12.5, 3.75) | Spans x 11–14. Bridges the safe trough. |
| GhostPlatform G2 | (23.5, 4.75) | Spans x 22–25. Step up toward the exit ledge. |
| KillZone_Pit | (24, -0.5), size (6, 1) | Under the pit only |
| Doorway | erase x 29, y 7–9 | Leads to room 1-02 |

Beats:
1. **Safe:** the trough (x 9–16) is 2 tiles deep. Falling in costs nothing; you hop back out. The player discovers that lit platforms let you fall through, unlit ones hold you.
2. **Require:** the pit (x 21–26) kills. G2 is the only way to the exit ledge, and it's 2 tiles below the ledge, so the player must stand on it in the dark.

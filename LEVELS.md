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

## Design rules for Shy Ghosts rooms

1. **One new idea per room.** Show it safely, then require it. A room that teaches two things teaches neither.
2. **Safe first, deadly second.** The first time a player meets an idea, failing costs a few seconds (a shallow trough), not a death.
3. **The player must see before they risk.** Every jump into the dark must be one they could have scouted. No platform the light or a flash can't reveal.
4. **Give memory anchors.** Line ghost platforms up with something solid: the edge of a pillar, a window in the background, a gap in the ceiling. "Third platform, under the window" is memorable; "somewhere in the middle" is not.
5. **Rest spots set the rhythm.** A solid pad between hard sections says "breathe, look again". In storm rooms, pads are where you wait for the flash.
6. **Short rooms, many attempts.** Respawn is instant, so a room should take 10-30 seconds when you know it. Long rooms make deaths frustrating.
7. **Every room has an "aha".** Before building, write one sentence: "This room is fun because ___." If you can't finish it, the room isn't ready.
8. **Teach with layout, not text.** The trough in 1-1 teaches the rule without a single word.

Storm rooms (Chapter 2) specifically:
- A flash makes ghost platforms non-solid. **Standing on a platform when lightning strikes = falling.** This is the chapter's core tension: hop between flashes, be on solid ground or in the air when it hits.
- Flash Interval 3 s gives about 2.75 s of dark per window. A 3-tile platform takes about 0.5 s to cross, so 1-3 platforms per window is fair; more needs a rest pad between.
- The first flash comes one full interval after entering, so spawn on solid ground: the player waits, then sees the room.

## Room 2-1 "First Storm" (introduce the storm)

Root position (210, 0). Room settings: Has Storm on, Flash Interval 3, Jitter 0, Flash Duration 0.25, Lantern Broken on.
This room is fun because: the first flash reveals a path you couldn't see, and the first fall into the trough teaches "don't stand on them when it flashes" without killing you.

```
        012345678901234567890123456789
16      ##############################
15      #                            #
 .. (empty rows) ..
 7      #                            #
 6      #                               <- doorway: x 29, y 4-6 open
 5      #
 4      #  S              ===           <- G2 (x 18-20), top at y 5
 3      ######  ===   ###        #####  <- G1 (x 8-10), top at y 4, flush with floor
 2      ######        ###        #####
 1      #################        #####
 0      #################        #####
                         ^^^^^^^^ x 17-24: deadly pit, open to the bottom
```

| Object | Position (center) | Notes |
|---|---|---|
| Spawn | (3, 4) | |
| GhostPlatform G1 | (9.5, 3.75) | Over the safe trough (x 6-13, 2 deep) |
| GhostPlatform G2 | (19.5, 4.75) | Over the deadly pit, 1 tile up from the pad |
| KillZone_Pit | (21, -0.5), size (8, 1) | |
| Rest pad | x 14-16 (solid) | Wait here for a flash before the pit |

Beats:
1. **Wait:** 3 s of darkness, then the first flash shows G1 over the trough.
2. **Safe lesson:** cross on G1. Linger on it and the next flash drops you into the trough. Hop out, try again.
3. **Rest:** the pad at x 14-16. Wait for a flash to see G2.
4. **Require:** right after a flash, hop onto G2, then jump the 4-tile gap to the right floor before the next flash.

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>Turns the forest around the spawn into a flat asphalt boulevard with a neon skyline behind it.</summary>
public static class LosSantos
{
    private static readonly int[] Gems = {
        WallID.AmberGemspark, WallID.TopazGemspark, WallID.DiamondGemspark, WallID.AmethystGemspark, WallID.SapphireGemspark };

    public static void Build()
    {
        int cx = (int)(Mix.Center.X / 16f);
        int sy = (int)(Mix.Center.Y / 16f) - 6;
        while (sy < Main.maxTilesY - 10 && !(Main.tile[cx, sy].HasTile && Main.tileSolid[Main.tile[cx, sy].TileType])) sy++;
        const int half = 80;

        // 1) clear everything standing above the street (trees, bushes, hills)
        for (int x = cx - half; x <= cx + half; x++)
            for (int y = sy - 45; y < sy; y++)
                if (WorldGen.InWorld(x, y, 5) && Main.tile[x, y].HasTile) WorldGen.KillTile(x, y, false, false, true);

        // 2) flat road: asphalt on top, dirt filling any valley below it
        for (int x = cx - half; x <= cx + half; x++) {
            for (int y = sy + 1; y < sy + 40; y++) {
                Tile t = Main.tile[x, y];
                if (t.HasTile && Main.tileSolid[t.TileType]) break;
                SetTile(x, y, TileID.Dirt);
            }
            SetTile(x, sy, TileID.Asphalt);
            if (x % 8 < 4) { Tile r = Main.tile[x, sy]; r.TileColor = PaintID.WhitePaint; }
        }

        // 3) skyline of wall-buildings behind the street (no collision, pure backdrop)
        int[] starts = { -78, -66, -55, -43, -30, -19, 10, 22, 34, 46, 58, 68 };
        int[] widths = { 10, 9, 10, 11, 9, 8, 10, 11, 10, 9, 8, 9 };
        int[] heights = { 22, 14, 28, 17, 24, 12, 19, 30, 15, 24, 13, 20 };
        int[] bodies = { WallID.GrayBrick, WallID.RedBrick, WallID.GrayBrick, WallID.Stone, WallID.RedBrick, WallID.GrayBrick };
        for (int b = 0; b < starts.Length; b++) {
            int x0 = cx + starts[b], body = bodies[b % bodies.Length];
            for (int dx = 0; dx < widths[b]; dx++)
                for (int dy = 0; dy < heights[b]; dy++) {
                    int x = x0 + dx, y = sy - 1 - dy;
                    bool window = dx % 3 == 1 && dx < widths[b] - 1 && dy % 4 >= 1 && dy % 4 <= 2 && dy > 1 && dy < heights[b] - 1;
                    int wall = body;
                    if (window) wall = (x * 7 + y * 3 + b) % 5 < 2 ? WallID.ObsidianBrick : Gems[(x + y + b) % Gems.Length];
                    PutWall(x, y, wall);
                }
            // neon roof stripe
            for (int dx = 0; dx < widths[b]; dx++)
                PutWall(x0 + dx, sy - 1 - heights[b], b % 2 == 0 ? WallID.RubyGemspark : WallID.SapphireGemspark);
        }
    }

    private static void SetTile(int x, int y, int type)
    {
        Tile t = Main.tile[x, y];
        t.HasTile = true;
        t.TileType = (ushort)type;
        t.TileColor = 0;
        WorldGen.SquareTileFrame(x, y, true);
    }

    private static void PutWall(int x, int y, int wall)
    {
        if (!WorldGen.InWorld(x, y, 5) || Main.tile[x, y].HasTile) return;
        Tile t = Main.tile[x, y];
        t.WallType = (ushort)wall;
        WorldGen.SquareWallFrame(x, y, true);
    }
}

using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>Shared state and helpers of the Los Santos takeover: wanted heat, cash, shooting, craters.</summary>
public static class Gta
{
    public static float Heat;
    public static int Cash;
    public static double ChopperReadyAt;
    public static int Stars => Math.Min(5, (int)Math.Ceiling(Heat / 20f));

    public static void AddHeat(float amount)
    {
        int before = Stars;
        Heat = Math.Min(100f, Heat + amount);
        if (Stars > before) {
            Mix.Sound("siren", Mix.Host.Center);
            Mix.Popup(Mix.Host.Center + new Vector2(0, -70), Stars + (Stars == 1 ? " STAR" : " STARS"), Color.Gold);
        }
    }

    public static void AddCash(int amount, Vector2 pos)
    {
        Cash += amount;
        Mix.Popup(pos, "+$" + amount, Color.LimeGreen);
    }

    /// <summary>Armed pedestrians and cops: fire a bullet at the player when close and in sight.</summary>
    public static void ShootAtPlayer(NPC npc, int damage, float speed)
    {
        Player target = Main.player[npc.target];
        if (target == null || !target.active) return;
        Vector2 from = npc.Center + new Vector2(npc.direction * 18, -6);
        float dist = Vector2.Distance(from, target.Center);
        if (dist > 520f || !Collision.CanHitLine(from, 4, 4, target.position, target.width, target.height)) return;
        Mix.Shoot<BulletShot>(from, Mix.Aim(from, target.Center, speed), damage, 2f, hostile: true);
        Mix.Sound("gunshot", from);
        Mix.Burst(from + new Vector2(npc.direction * 6, 0), DustID.Torch, 6, 4f, 2f);
        Mix.Light(from, Color.Orange);
    }

    /// <summary>Blows a hole in the ground (dirt, grass, stone, sand) around pos.</summary>
    public static void Crater(Vector2 pos, int radiusTiles)
    {
        int cx = (int)(pos.X / 16f), cy = (int)(pos.Y / 16f);
        for (int x = cx - radiusTiles; x <= cx + radiusTiles; x++)
            for (int y = cy - radiusTiles; y <= cy + radiusTiles; y++) {
                if (!WorldGen.InWorld(x, y, 5)) continue;
                if ((x - cx) * (x - cx) + (y - cy) * (y - cy) > radiusTiles * radiusTiles) continue;
                Tile t = Main.tile[x, y];
                if (!t.HasTile) continue;
                if (t.TileType == TileID.Dirt || t.TileType == TileID.Grass || t.TileType == TileID.Stone
                    || t.TileType == TileID.Sand || t.TileType == TileID.ClayBlock || t.TileType == TileID.Mud)
                    WorldGen.KillTile(x, y, false, false, true);
            }
    }

    public static void Explosion(Vector2 pos, float radiusPx, int damage, bool craters)
    {
        Mix.Burst(pos, DustID.Torch, 50, radiusPx * 0.5f, 7f, null, 2.2f);
        Mix.Burst(pos, DustID.Smoke, 30, radiusPx * 0.5f, 4f, null, 2f);
        Mix.Burst(pos, DustID.Fireworks, 14, radiusPx * 0.4f, 5f, Color.Yellow, 1.2f);
        Mix.Sound(SoundID.Item14, pos);
        Mix.Shake(7, 0.35);
        foreach (NPC n in Mix.Hostiles(pos, radiusPx / 16.0)) {
            int dir = n.Center.X >= pos.X ? 1 : -1;
            n.SimpleStrikeNPC(damage, dir, false, 9f, DamageClass.Ranged);
            n.velocity += new Vector2(dir * 5f, -4f);
        }
        if (craters) Crater(pos, 2);
    }
}

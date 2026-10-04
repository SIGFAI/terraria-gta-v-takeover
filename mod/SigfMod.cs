using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

public class SigfMod : ModSystem
{
    private static int Count(int type) => Main.npc.Count(n => n.active && n.type == type);

    private static void SpawnSide<T>(float side, double radius = 10) where T : ModNPC
    {
        Vector2 at = Mix.Ground(Mix.Host.Center + new Vector2(side * 650, 0), radius);
        Mix.Spawn<T>(at);
    }

    private static void FireRocketAtNearest()
    {
        NPC n = Mix.Nearest(Mix.Host.Center, 30);
        Vector2 from = Mix.Host.Center + new Vector2(0, -10);
        Vector2 to = n != null ? n.Center : Mix.Ahead(10);
        Mix.Shoot<RocketShot>(from, Mix.Aim(from, to, 9f), 75, 8f);
        Mix.Sound(SoundID.Item92, from);
        Mix.Burst(from, DustID.Smoke, 12, 6f, 3f, null, 1.8f);
    }

    private static void FireRocketAtChopper()
    {
        NPC c = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<PoliceChopper>());
        if (c == null) return;
        Vector2 from = Mix.Host.Center + new Vector2(0, -10);
        Mix.Shoot<RocketShot>(from, Mix.Aim(from, c.Center, 9f), 75, 8f);
        Mix.Sound(SoundID.Item92, from);
        Mix.Burst(from, DustID.Smoke, 12, 6f, 3f, null, 1.8f);
    }

    public override void OnWorldLoad()
    {
        Gta.Heat = 0; Gta.Cash = 0; Gta.ChopperReadyAt = 0;
        Mix.After(0.3, LosSantos.Build);

        // always on: street life
        Mix.Every(4, () => {
            if (Count(ModContent.NPCType<Gangster>()) < 4) SpawnSide<Gangster>(Main.rand.NextBool() ? 1 : -1);
            if (Gta.Stars >= 1 && Count(ModContent.NPCType<Cop>()) < Gta.Stars * 2) SpawnSide<Cop>(Main.rand.NextBool() ? 1 : -1);
            if (Gta.Stars >= 3 && Mix.Seconds >= Gta.ChopperReadyAt && Count(ModContent.NPCType<PoliceChopper>()) < 1)
                Mix.Spawn(ModContent.NPCType<PoliceChopper>(), Mix.Host.Center + new Vector2(Main.rand.NextBool() ? 700 : -700, -250));
        });
        Mix.Every(15, () => {
            if (Count(ModContent.NPCType<SportsCar>()) < 1) SpawnSide<SportsCar>(Main.rand.NextBool() ? 1 : -1, 6);
        });

        // demo script
        Mix.Demo(0.5, () => {
            Mix.Arm<Pistol>();
            Mix.Title("GTA V TAKEOVER", "Welcome to Los Santos", 3);
            SpawnSide<Gangster>(1, 6); SpawnSide<Gangster>(1, 8); SpawnSide<Gangster>(-1, 8);
            Mix.Shoot<BulletShot>(Mix.Host.Center, new Vector2(14, 0), 24);
        });
        Mix.Demo(7, () => {
            SpawnSide<SportsCar>(-1, 4);
            Mix.Spawn(NPCID.BlueSlime, Mix.Ahead(9));
            Mix.Spawn(NPCID.GreenSlime, Mix.Ahead(7));
            SpawnSide<Gangster>(1, 8);
        });
        Mix.Demo(16, () => { Gta.AddHeat(30); SpawnSide<Cop>(1, 6); SpawnSide<Cop>(-1, 6); });
        Mix.Demo(24, () => {
            Mix.Arm<RocketLauncher>();
            Mix.Spawn<Gangster>(Mix.Ahead(9)); Mix.Spawn<Gangster>(Mix.Ahead(10)); Mix.Spawn<Cop>(Mix.Ahead(11));
        });
        Mix.Demo(26, FireRocketAtNearest);
        Mix.Demo(31, FireRocketAtNearest);
        Mix.Demo(30, () => { Gta.AddHeat(70); });
        Mix.Demo(33, () => { Mix.Arm<RocketLauncher>(); });
        Mix.Demo(38, FireRocketAtChopper);
        Mix.Demo(41, FireRocketAtChopper);
        Mix.Demo(44, FireRocketAtChopper);
        Mix.Demo(47, FireRocketAtChopper);
        Mix.Demo(50, FireRocketAtChopper);
        Mix.Demo(53, () => { Mix.Arm<Pistol>(); SpawnSide<SportsCar>(1, 4); });
        Mix.Demo(58, () => { SpawnSide<Gangster>(1, 6); SpawnSide<Gangster>(-1, 6); });
        Mix.Demo(63, () => { Mix.Arm<RocketLauncher>(); });
        Mix.Demo(65, FireRocketAtNearest);
        Mix.Demo(69, FireRocketAtNearest);
    }
}

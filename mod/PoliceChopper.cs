using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>Police helicopter: circles above the player with a searchlight and strafes with bullets.</summary>
public class PoliceChopper : ModNPC
{
    private int age;
    private int burst;

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 2;

    public override void SetDefaults()
    {
        NPC.scale = 1.3f; NPC.width = 143; NPC.height = 65;
        NPC.lifeMax = 320; NPC.damage = 20; NPC.defense = 6; NPC.knockBackResist = 0f; NPC.value = 2000;
        NPC.aiStyle = -1; NPC.noGravity = true; NPC.noTileCollide = true;
        NPC.HitSound = SoundID.NPCHit4; NPC.DeathSound = SoundID.Item14;
    }

    public override void AI()
    {
        age++;
        NPC.TargetClosest(false);
        Player pl = Main.player[NPC.target];
        Vector2 goal = pl.Center + new Vector2((float)System.Math.Sin(age / 110f) * 300f, -230f + (float)System.Math.Sin(age / 37f) * 25f);
        NPC.velocity = Vector2.Lerp(NPC.velocity, (goal - NPC.Center) * 0.05f, 0.08f);
        NPC.direction = pl.Center.X >= NPC.Center.X ? 1 : -1; NPC.spriteDirection = NPC.direction;
        NPC.rotation = NPC.velocity.X * 0.012f;
        NPC.timeLeft = 600;
        if (age % 30 == 0) Mix.Sound(SoundID.Item24, NPC.Center);

        // searchlight cone toward the player
        Vector2 belly = NPC.Center + new Vector2(NPC.direction * 26, 26);
        for (int i = 1; i <= 8; i++) Mix.Light(Vector2.Lerp(belly, pl.Center, i / 8f), Color.White * 0.8f);
        Dust d = Dust.NewDustPerfect(Vector2.Lerp(belly, pl.Center, Main.rand.NextFloat()), DustID.WhiteTorch, Vector2.Zero, 150, default, 0.8f);
        d.noGravity = true;

        // strafing bursts: 3 bullets every 2 s
        if (age % 130 == 0) burst = 3;
        if (burst > 0 && age % 8 == 0) {
            burst--;
            Vector2 from = belly;
            Vector2 aim = pl.Center + new Vector2(Main.rand.Next(-30, 30), 0);
            Mix.Shoot<BulletShot>(from, Mix.Aim(from, aim, 11f), 14, 2f, hostile: true);
            Mix.Sound("gunshot", from);
            Mix.Burst(from, DustID.Torch, 5, 4f, 2f);
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        NPC.frame.Y = (int)(NPC.frameCounter / 3 % 2) * frameHeight;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        Mix.Burst(NPC.Center, DustID.Electric, 8, 40f, 4f);
        if (NPC.life <= 0) {
            Gta.Explosion(NPC.Center, 200f, 80, false);
            Mix.Shake(14, 0.8);
            Mix.Burst(NPC.Center, DustID.Torch, 80, 60f, 9f, null, 2.5f);
        }
    }

    public override void OnKill()
    {
        Gta.AddHeat(40);
        Gta.ChopperReadyAt = Mix.Seconds + 30;
        Gta.AddCash(2500, NPC.Center);
        Mix.Drop<CashStack>(NPC.Center, 6);
        Mix.Title("MISSION PASSED", "Chopper down  +$2,500", 3);
    }
}

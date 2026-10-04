using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

/// <summary>A joyriding red sports car: it speeds across the map and launches whatever it hits.</summary>
public class SportsCar : ModNPC
{
    private int dir;
    private int age;
    private readonly int[] hitCooldown = new int[Main.maxNPCs];

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 2;

    public override void SetDefaults()
    {
        NPC.scale = 1.4f; NPC.width = 168; NPC.height = 56;
        NPC.lifeMax = 260; NPC.damage = 30; NPC.defense = 5; NPC.knockBackResist = 0.05f; NPC.value = 300;
        NPC.aiStyle = -1;
        NPC.HitSound = SoundID.NPCHit4; NPC.DeathSound = SoundID.Item14;
    }

    public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
    {
        Mix.Sound("vroom", NPC.Center);
    }

    public override void AI()
    {
        age++;
        Player pl = Mix.Host;
        float dx = pl.Center.X - NPC.Center.X;
        if (dir == 0) dir = dx >= 0 ? 1 : -1;
        if (dir * dx < -650f) dir = -dir;          // overshot the player: U-turn
        NPC.direction = dir; NPC.spriteDirection = dir;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dir * 10f, 0.05f);
        if (NPC.collideX && NPC.velocity.Y == 0f) NPC.velocity.Y = -7.5f;   // hop over blocks
        NPC.timeLeft = 600;
        if (Math.Abs(dx) > 2200f || age > 60 * 40) { NPC.active = false; return; }

        // exhaust, headlight, tire smoke
        Vector2 rear = NPC.Center + new Vector2(-dir * 78, 10);
        Dust ex = Dust.NewDustPerfect(rear, DustID.Smoke, new Vector2(-dir * 2f, -0.5f), 100, default, 1.2f);
        ex.noGravity = true;
        Mix.Light(NPC.Center + new Vector2(dir * 84, -2), Color.LightYellow);

        // run over anything on foot
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (hitCooldown[i] > 0) hitCooldown[i]--;
            NPC n = Main.npc[i];
            if (!n.active || n.whoAmI == NPC.whoAmI || n.friendly || n.type == Type || n.type == ModContent.NPCType<PoliceChopper>()) continue;
            if (hitCooldown[i] > 0 || Math.Abs(NPC.velocity.X) < 4f || !NPC.Hitbox.Intersects(n.Hitbox)) continue;
            hitCooldown[i] = 40;
            n.SimpleStrikeNPC(45, dir, false, 10f, DamageClass.Melee);
            n.velocity = new Vector2(dir * 11f, -7f);
            Mix.Burst(n.Center, DustID.Blood, 14, 14f, 4f);
            Mix.Sound(SoundID.NPCHit2, n.Center);
            Mix.Shake(4, 0.2);
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        NPC.frame.Y = (int)(NPC.frameCounter / 4 % 2) * frameHeight;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        Mix.Burst(NPC.Center, DustID.Electric, 8, 30f, 4f);
        if (NPC.life <= 0) Gta.Explosion(NPC.Center, 140f, 60, false);
    }

    public override void OnKill()
    {
        Gta.AddHeat(25);
        Mix.Drop<CashStack>(NPC.Center, 4);
    }
}

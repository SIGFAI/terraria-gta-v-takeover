using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

public class Gangster : ModNPC
{
    private int shotTimer;

    public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 4;

    public override void SetDefaults()
    {
        NPC.scale = 1.3f; NPC.width = 31; NPC.height = 65;
        NPC.lifeMax = 50; NPC.damage = 12; NPC.defense = 2; NPC.knockBackResist = 0.7f; NPC.value = 60;
        NPC.aiStyle = NPCAIStyleID.Fighter; AIType = NPCID.Zombie;
        NPC.HitSound = SoundID.NPCHit1; NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override void AI()
    {
        if (++shotTimer >= 150) {
            shotTimer = Main.rand.Next(0, 50);
            Gta.ShootAtPlayer(NPC, 10, 9f);
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter += System.Math.Abs(NPC.velocity.X) > 0.3f ? 1 : 0;
        NPC.frame.Y = (int)(NPC.frameCounter / 6 % 4) * frameHeight;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        Mix.Burst(NPC.Center, DustID.Blood, 10, 12f, 3f);
        if (NPC.life <= 0) Mix.Burst(NPC.Center, DustID.Blood, 30, 20f, 6f);
    }

    public override void OnKill()
    {
        Gta.AddHeat(12);
        Mix.Drop<CashStack>(NPC.Center, Main.rand.Next(1, 3));
    }
}

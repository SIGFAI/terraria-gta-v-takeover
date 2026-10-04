using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

public class BulletShot : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 8; Projectile.height = 8;
        Projectile.friendly = true; Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 1; Projectile.timeLeft = 80; Projectile.extraUpdates = 1;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
        Mix.Light(Projectile.Center, new Color(255, 200, 80) * 0.6f);
        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Vector2.Zero, 0, default, 0.9f);
        d.noGravity = true; d.velocity *= 0.1f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Mix.Burst(target.Center, DustID.Blood, 8, 10f, 3f);
    }

    public override void OnKill(int timeLeft)
    {
        Mix.Burst(Projectile.Center, DustID.Torch, 6, 4f, 3f, null, 1f);
        Mix.Burst(Projectile.Center, DustID.Smoke, 3, 4f, 1f, null, 0.8f);
    }
}

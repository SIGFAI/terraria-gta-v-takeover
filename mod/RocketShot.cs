using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

public class RocketShot : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 14; Projectile.height = 14;
        Projectile.friendly = true; Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = 1; Projectile.timeLeft = 180;
        Projectile.ignoreWater = true;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
        if (Projectile.velocity.Length() < 15f) Projectile.velocity *= 1.03f;
        if (Projectile.ai[0]++ > 8) {
            NPC target = Mix.Nearest(Projectile.Center, 22);
            if (target != null) {
                float speed = Projectile.velocity.Length();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Mix.Aim(Projectile.Center, target.Center, speed), 0.07f).SafeNormalize(Vector2.UnitX) * speed;
            }
        }
        Mix.Light(Projectile.Center, Color.OrangeRed);
        Vector2 tail = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 10f;
        Dust f = Dust.NewDustPerfect(tail, DustID.Torch, -Projectile.velocity * 0.15f, 0, default, 1.8f);
        f.noGravity = true;
        Dust s = Dust.NewDustPerfect(tail, DustID.Smoke, -Projectile.velocity * 0.05f, 120, default, 1.5f);
        s.noGravity = true;
    }

    public override void OnKill(int timeLeft)
    {
        Gta.Explosion(Projectile.Center, 110f, Projectile.damage, true);
    }
}

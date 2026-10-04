using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

public class RocketLauncher : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 44; Item.height = 14;
        Item.damage = 75; Item.DamageType = DamageClass.Ranged; Item.knockBack = 8f;
        Item.useTime = 45; Item.useAnimation = 45; Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true; Item.autoReuse = true;
        Item.UseSound = SoundID.Item92;
        Item.rare = ItemRarityID.Red;
        Item.shoot = ModContent.ProjectileType<RocketShot>(); Item.shootSpeed = 9f;
    }

    public override Vector2? HoldoutOffset() => new Vector2(-8, 0);

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Vector2 muzzle = position + velocity.SafeNormalize(Vector2.UnitX) * 30f;
        Mix.Burst(muzzle, DustID.Smoke, 12, 6f, 3f, null, 1.8f);
        Mix.Burst(muzzle, DustID.Torch, 10, 5f, 4f, null, 1.6f);
        Mix.Shake(3, 0.15);
        return true;
    }
}

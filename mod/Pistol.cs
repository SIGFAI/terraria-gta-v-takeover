using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

public class Pistol : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28; Item.height = 18;
        Item.damage = 24; Item.DamageType = DamageClass.Ranged; Item.knockBack = 3f;
        Item.useTime = 14; Item.useAnimation = 14; Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true; Item.autoReuse = true;
        Item.UseSound = Mix.Style("gunshot");
        Item.rare = ItemRarityID.Green;
        Item.shoot = ModContent.ProjectileType<BulletShot>(); Item.shootSpeed = 15f;
    }

    public override Vector2? HoldoutOffset() => new Vector2(-2, 0);

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Vector2 muzzle = position + velocity.SafeNormalize(Vector2.UnitX) * 22f;
        Mix.Burst(muzzle, DustID.Torch, 8, 4f, 3f, null, 1.4f);
        Mix.Light(muzzle, Color.Orange);
        return true;
    }
}

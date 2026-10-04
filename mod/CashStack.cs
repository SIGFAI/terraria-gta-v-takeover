using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Sigf.Content;

public class CashStack : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 24; Item.height = 22;
        Item.maxStack = 999; Item.value = 0; Item.rare = ItemRarityID.Green;
    }

    // picked up cash goes straight to the wallet on screen
    public override bool OnPickup(Player player)
    {
        Gta.AddCash(100 * Item.stack, player.Center + new Microsoft.Xna.Framework.Vector2(0, -40));
        Mix.Sound(SoundID.CoinPickup, player.Center);
        return false;
    }
}

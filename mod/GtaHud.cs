using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Sigf.Content;

/// <summary>Sunset sky, warm Los Santos color grade, wanted stars, cash counter and police-light flashes.</summary>
public class GtaHud : ModSystem
{
    public override void PostUpdateTime()
    {
        // permanent golden-hour sunset
        Main.dayTime = true;
        Main.time = 46500;
    }

    public override void PostUpdateEverything()
    {
        // heat cools off slowly when nobody is shooting
        if (Gta.Heat > 0) Gta.Heat = Math.Max(0f, Gta.Heat - 0.025f);
        if (Gta.Stars > 0 && Main.GameUpdateCount % 420 == 0) Mix.Sound("siren", Mix.Host.Center);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        layers.Insert(0, new LegacyGameInterfaceLayer("Gta: Grade", () => { DrawGrade(Main.spriteBatch); return true; }, InterfaceScaleType.None));
        layers.Add(new LegacyGameInterfaceLayer("Gta: Hud", () => { DrawHud(Main.spriteBatch); return true; }, InterfaceScaleType.None));
    }

    private static void DrawGrade(SpriteBatch sb)
    {
        if (Main.gameMenu) return;
        Texture2D px = TextureAssets.MagicPixel.Value;
        int w = Main.screenWidth, h = Main.screenHeight;
        sb.Draw(px, new Rectangle(0, 0, w, h), new Color(255, 120, 40) * 0.10f);
        // vignette: dark bands on the edges
        for (int i = 0; i < 14; i++) {
            float a = 0.30f * (1f - i / 14f);
            int t = 14;
            sb.Draw(px, new Rectangle(0, i * t, w, t), Color.Black * (a * 0.7f));
            sb.Draw(px, new Rectangle(0, h - (i + 1) * t, w, t), Color.Black * (a * 0.7f));
            sb.Draw(px, new Rectangle(i * t, 0, t, h), Color.Black * (a * 0.7f));
            sb.Draw(px, new Rectangle(w - (i + 1) * t, 0, t, h), Color.Black * (a * 0.7f));
        }
        // police lights when wanted
        if (Gta.Stars > 0) {
            bool phase = (Main.GameUpdateCount / 18) % 2 == 0;
            Color left = phase ? Color.Red : Color.RoyalBlue, right = phase ? Color.RoyalBlue : Color.Red;
            for (int i = 0; i < 20; i++) {
                float a = 0.22f * (1f - i / 20f);
                sb.Draw(px, new Rectangle(i * 10, 0, 10, h), left * a);
                sb.Draw(px, new Rectangle(w - (i + 1) * 10, 0, 10, h), right * a);
            }
        }
    }

    private static void DrawHud(SpriteBatch sb)
    {
        if (Main.gameMenu) return;
        int x0 = 80, y0 = 200;
        Texture2D star = TextureAssets.Item[ItemID.FallenStar].Value;
        Rectangle frame = Main.itemAnimations[ItemID.FallenStar] != null ? Main.itemAnimations[ItemID.FallenStar].GetFrame(star) : star.Bounds;
        bool blink = (Main.GameUpdateCount / 20) % 2 == 0;
        for (int i = 0; i < 5; i++) {
            Vector2 pos = new Vector2(x0 + i * 46, y0);
            bool on = i < Gta.Stars;
            Color c = on ? (blink ? Color.White : new Color(255, 215, 80)) : new Color(30, 30, 30) * 0.9f;
            sb.Draw(star, pos, frame, c, 0f, frame.Size() / 2f, on ? 1.7f : 1.4f, SpriteEffects.None, 0f);
        }
        if (Gta.Stars > 0)
            Utils.DrawBorderStringBig(sb, "WANTED", new Vector2(x0 - 24, y0 + 28), blink ? Color.Red : Color.White, 0.6f);
        Utils.DrawBorderStringBig(sb, "$" + Gta.Cash.ToString("N0"), new Vector2(x0 - 24, y0 + (Gta.Stars > 0 ? 58 : 28)), new Color(110, 220, 90), 0.9f);
    }
}

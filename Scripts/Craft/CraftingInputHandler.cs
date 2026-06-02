using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PharmaCat.Scripts
{
    public partial class CraftGreyboxSystem
    {
        private void CreateJarsFromInventory()
        {
            jars.Clear();
            int index = 0;

            foreach (var herb in inventory.CollectedHerbs)
            {
                if (herb.Value <= 0)
                {
                    continue;
                }
               
                Rectangle rect = new Rectangle(
                    150 + (index % 7) * 150, 
                    100,                     
                    100,
                    115
                );

                jars.Add(new JarBox(
                    herb.Key,
                    GetHerbColor(herb.Key),
                    rect,
                    herb.Value
                ));

                index++;
            }
        }

        private void CreateGlassesFromInventory()
{
    glasses.Clear();

    int index = 0;

    foreach (var potion in inventory.CraftedPotions)
    {
        if (potion.Value <= 0)
            continue;

        for (int i = 0; i < potion.Value; i++)
        {
            PotionGlassBox glass = new PotionGlassBox(NewGlassPosition(index));

            glass.IsFilled = true;
            glass.PotionName = potion.Key;
            glass.FillColor = GetPotionColor(potion.Key);

            glasses.Add(glass);
            index++;
        }
    }

    for (int i = 0; i < inventory.EmptyBottleCount; i++)
    {
        glasses.Add(new PotionGlassBox(NewGlassPosition(index)));
        index++;
    }
}

private Color GetPotionColor(string potionName)
{
    switch (potionName)
    {
        case "Sleep Potion":
            return Mix(GetHerbColor("Lavender"), GetHerbColor("Blue Lotus"));

        case "Love Potion":
            return Mix(GetHerbColor("Love Rose"), GetHerbColor("Lavender"));

        case "Anti-Curse Potion":
            return Mix(GetHerbColor("Anti-Curse Clover"), GetHerbColor("Sage"));

        case "Memory Potion":
            return Mix(GetHerbColor("Sage"), GetHerbColor("Blue Lotus"));

        case "Pain Relief Potion":
            return Mix(GetHerbColor("Red Poppy"), GetHerbColor("Marigold"));

        case "Persuasion Potion":
            return Mix(GetHerbColor("Love Rose"), GetHerbColor("Sage"));

        case "Purification Potion":
            return Mix(GetHerbColor("Lavender"), GetHerbColor("Anti-Curse Clover"));

        case "Relaxation Potion":
            return Mix(GetHerbColor("Lavender"), GetHerbColor("Sage"));

        case "Soothing Potion":
            return Mix(GetHerbColor("Lavender"), GetHerbColor("Red Poppy"));

        case "Mystic Romance Potion":
            return Mix(GetHerbColor("Blue Lotus"), GetHerbColor("Love Rose"));

        case "Holy Water Potion":
            return Mix(GetHerbColor("Blue Lotus"), GetHerbColor("Anti-Curse Clover"));

        case "Heart Protection Potion":
            return Mix(GetHerbColor("Love Rose"), GetHerbColor("Anti-Curse Clover"));

        case "Passion Potion":
            return Mix(GetHerbColor("Love Rose"), GetHerbColor("Red Poppy"));

        case "Vitality Potion":
            return Mix(GetHerbColor("Anti-Curse Clover"), GetHerbColor("Red Poppy"));

        case "Focus Potion":
            return Mix(GetHerbColor("Sage"), GetHerbColor("Red Poppy"));

        case "Enlightenment Potion":
            return Mix(GetHerbColor("Sage"), GetHerbColor("Marigold"));

        case "Calm Potion":
            return GetHerbColor("Lavender");

        case "Clarity Potion":
            return GetHerbColor("Blue Lotus");

        case "Charm Potion":
            return GetHerbColor("Love Rose");

        case "Ward Potion":
            return GetHerbColor("Anti-Curse Clover");

        case "Wisdom Potion":
            return GetHerbColor("Sage");

        case "Rage Potion":
            return GetHerbColor("Red Poppy");

        case "Bright Potion":
            return GetHerbColor("Marigold");

        default:
            return Color.Purple;
    }
}

        private Rectangle NewGlassPosition(int index)
        {
            float scale = 0.15f; 

            int width = (int)(texEmptyGlass.Width * scale);
            int height = (int)(texEmptyGlass.Height * scale);

            int x = 750 + (index % 6) * (width + 20);
            int y = 750 + (index / 6) * (height + 20);

            return new Rectangle(x, y, width, height);
        }

        private void HandleJarDrag(Point mp)
        {
            if (LeftPressed())
            {
                foreach (var jar in jars)
                {
                    if (jar.Bounds.Contains(mp))
                    {
                        draggedJar = jar;
                        break;
                    }
                }
            }

            if (draggedJar != null && mouse.LeftButton == ButtonState.Pressed)
            {
                draggedJar.DragPosition = new Vector2(
                    mp.X - draggedJar.Bounds.Width / 2,
                    mp.Y - draggedJar.Bounds.Height / 2
                );
            }

            if (draggedJar != null && LeftReleased())
            {
                Rectangle dragRect = new Rectangle(
                    (int)draggedJar.DragPosition.X,
                    (int)draggedJar.DragPosition.Y,
                    draggedJar.Bounds.Width,
                    draggedJar.Bounds.Height
                );

                if (dragRect.Intersects(mortar.Bounds))
                {
                    PourHerb(draggedJar);
                }

                draggedJar.DragPosition = Vector2.Zero;
                draggedJar = null;
            }
        }

        private void PourHerb(JarBox jar)
        {
            if (jar.Amount <= 0)
            {
                return;
            }

            if (!mortar.HasBottom)
            {
                mortar.BottomColor = jar.HerbColor;
                mortar.BottomHerbName = jar.Name;
                mortar.HasBottom = true;

                inventory.RemoveHerb(jar.Name, 1);
                CreateJarsFromInventory();
            }
            else if (!mortar.HasTop)
            {
                mortar.TopColor = jar.HerbColor;
                mortar.TopHerbName = jar.Name;
                mortar.HasTop = true;

                inventory.RemoveHerb(jar.Name, 1);
                CreateJarsFromInventory();
            }
        }

        private void HandleWater(Point mp)
        {
            if (!LeftPressed()) 
            {
                return;
            }

            if (!waterBox.Contains(mp)) 
            {
                return;
            }

            if (!mortar.HasBottom || !mortar.HasTop) 
            {
                return;
            }

            hasWater = true;
        }

        private void HandleGrinderDrag(GameTime gameTime, Point mp)
        {
            if (LeftPressed() && grinderCurrentRect.Contains(mp))
            {
                draggingGrinder = true;

                grinderDragOffset = new Vector2(
                    mp.X - grinderCurrentRect.X,
                    mp.Y - grinderCurrentRect.Y
                );
            }

            if (draggingGrinder && mouse.LeftButton == ButtonState.Pressed)
            {
                Rectangle desiredRect = new Rectangle(
                    (int)(mp.X - grinderDragOffset.X),
                    (int)(mp.Y - grinderDragOffset.Y),
                    grinderCurrentRect.Width,
                    grinderCurrentRect.Height
                );

                desiredRect = ApplyMortarSideWallCollision(desiredRect);

                grinderCurrentRect = desiredRect;

                TryGrindWithEnterExit();
            }

            if (draggingGrinder && LeftReleased())
            {
                draggingGrinder = false;
                grinderWasInsideMortar = false;
            }
        }

        private Rectangle ApplyMortarSideWallCollision(Rectangle desiredRect)
        {
            Rectangle outerCollider = mortar.Bounds;

            int leftWallThickness = 35;
            int rightWallThickness = 55;

            int bottomWallThickness = 35;

            if (!desiredRect.Intersects(outerCollider))
            {
                return desiredRect;
            }

            int innerLeft = outerCollider.Left + leftWallThickness;
            int innerRight = outerCollider.Right - rightWallThickness;

            int innerBottom = outerCollider.Bottom - bottomWallThickness;

            int clampedX = (int)MathHelper.Clamp(
                desiredRect.X,
                innerLeft,
                innerRight - desiredRect.Width
            );

            int clampedY = desiredRect.Y;

            if (desiredRect.Bottom > innerBottom)
            {
                clampedY = innerBottom - desiredRect.Height;
            }

            return new Rectangle(
                clampedX,
                clampedY,
                desiredRect.Width,
                desiredRect.Height
            );
        }

        private void TryGrindWithEnterExit()
        {
            if (!mortar.HasBottom || !mortar.HasTop || !hasWater)
            {
                return;
            }

            if (hasMixedPotion)
            {
                return;
            }

            bool grinderInsideMortar = grinderCurrentRect.Intersects(mortarGrindArea);
            
            if (grinderInsideMortar && !grinderWasInsideMortar)
            {
                float grindAmount = 35f + inventory.MortarLevel * 15f;
                grindProgress += grindAmount;

                if (grindProgress >= grindNeeded)
                {
                    FinishGrinding();
                }
            }

            grinderWasInsideMortar = grinderInsideMortar;
        }

        private void FinishGrinding()
        {
            mixedColor = Mix(mortar.BottomColor, mortar.TopColor);
            hasMixedPotion = true;

            craftedPotionName = PotionRecipeDatabase.GetPotionResult(mortar.BottomHerbName, mortar.TopHerbName);

            mortar.HasBottom = false;
            mortar.HasTop = false;
            mortar.BottomHerbName = "";
            mortar.TopHerbName = "";

            grindProgress = 0f;
        }

        private void HandleMortarDrag(Point mp)
        {
            if (LeftPressed() && mortar.Bounds.Contains(mp) && hasMixedPotion)
            {
                draggingMortar = true;
            }

            if (draggingMortar && LeftReleased())
            {
                if (binBox.Contains(mp))
                {
                    hasMixedPotion = false;
                    hasWater = false;
                    mixedColor = Color.Transparent;
                    craftedPotionName = "";
                    grindProgress = 0f;

                    draggingMortar = false;
                    return;
                }

                foreach (var glass in glasses)
                {
                    if (glass.Bounds.Contains(mp) && !glass.IsFilled)
                    {
                        glass.IsFilled = true;
                        glass.FillColor = mixedColor;
                        glass.PotionName = craftedPotionName;

                        inventory.AddPotion(craftedPotionName, 1);

                        if (inventory.EmptyBottleCount > 0)
                        {
                        inventory.EmptyBottleCount--;
                        }

                        hasMixedPotion = false;
                        hasWater = false;
                        mixedColor = Color.Transparent;
                        craftedPotionName = "";

                        break;
                    }
                }

                draggingMortar = false;
            }
        }

        private Texture2D GetJarTexture(string herbName)
        {
            switch (herbName)
            {
                case "Lavender": 
                    return texLavender;
                case "Blue Lotus": 
                    return texBlueLotus;
                case "Anti-Curse Clover": 
                    return texAntiCurse;
                case "Sage": 
                    return texSage;
                case "Love Rose": 
                    return texLoveRose;
                case "Red Poppy": 
                    return texRedPoppy;
                case "Marigold": 
                    return texMarigold;
                default: 
                    return texLavender;
            }
        }

        private Color GetHerbColor(string herbName)
        {
            switch (herbName)
            {
                case "Lavender": 
                    return Color.MediumPurple;
                case "Blue Lotus": 
                    return Color.DeepSkyBlue;
                case "Love Rose": 
                    return Color.HotPink;
                case "Anti-Curse Clover": 
                    return Color.LimeGreen;
                case "Sage": 
                    return Color.DarkSeaGreen;
                case "Red Poppy": 
                    return Color.Red;
                case "Marigold": 
                    return Color.Orange;
                default: 
                    return Color.White;
            }
        }

        private Color Mix(Color a, Color b)
        {
            return new Color((a.R + b.R) / 2, (a.G + b.G) / 2, (a.B + b.B) / 2);
        }
    }

}

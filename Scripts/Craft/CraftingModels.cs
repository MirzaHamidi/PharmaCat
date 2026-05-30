using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace PharmaCat.Scripts
{
    public class JarBox
    {
        public string Name;
        public Color HerbColor;
        public Rectangle Bounds;
        public Vector2 DragPosition;
        public int Amount;

        public JarBox(string name, Color herbColor, Rectangle bounds, int amount)
        {
            Name = name;
            HerbColor = herbColor;
            Bounds = bounds;
            Amount = amount;
        }
    }

    public class PotionGlassBox
    {
        public string PotionName;
        public Rectangle Bounds;
        public bool IsFilled;
        public Color FillColor;
        public Vector2 DragPosition; 

        public PotionGlassBox(Rectangle bounds)
        {
            Bounds = bounds;
            PotionName = "";
        }
    }

    public class MortarBox
    {
        public Rectangle Bounds;
        public Rectangle Grinder;

        public string BottomHerbName;
        public string TopHerbName;

        public bool HasBottom;
        public bool HasTop;

        public Color BottomColor;
        public Color TopColor;

        public MortarBox(Rectangle bounds)
        {
            Bounds = bounds;
            Grinder = new Rectangle(bounds.Right + 25, bounds.Y - 15, 35, bounds.Height + 65);

            BottomHerbName = "";
            TopHerbName = "";
        }
    }

}

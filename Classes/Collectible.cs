using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace OobiMobile.Classes
{
    class Collectible
    {
        public int Lives { set; get; }
        public Vector2 Position { set; get; }
        public Vector2 Velc { set; get; }
        public int Type { set; get; }
        public float ColRadius { set; get; }

        public Collectible(int t, Vector2 v, Vector2 p)
        {
            Position = p;
            Velc = v;
            Type = t;
        }

        public void Move(GameTime gameTime)
        {
            Position += Velc * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
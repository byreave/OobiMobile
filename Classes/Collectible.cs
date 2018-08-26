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
        public int lives { set; get; }
        public Vector2 position { set; get; }
        public Vector2 velc { set; get; }
        public int type { set; get; }
        public float ColRadius { set; get; }

        public Collectible(int t, Vector2 v, Vector2 p, int l = 1)
        {
            lives = l;
            position = p;
            velc = v;
            type = t;
        }

        public void Move(GameTime gameTime)
        {
            position += velc * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Oobi.Classes
{
    class MainCharacter
    {
        public int Lives { set; get; }
        public Vector2 Position { set; get; }
        public Vector2 Velc { set; get; }
        public Texture2D Texture { set; get; }
        public float ColRadius { set; get; }

        public bool IsDragged { set; get; }
        public MainCharacter(Texture2D t, Vector2 p, Vector2 v, int l = 0)
        {
            Texture = t;
            Lives = l;
            Position = p;
            Velc = v;
            ColRadius = (Texture.Width + Texture.Height) / 4.0f;
            IsDragged = false;
        }

        public bool BorderCheck(float ViewportWidth, float ViewportHeight, float BorderRadius, Vector2 pivotCenter)
        {
            if (Position.X < (ColRadius - 0.01f) / 2 && Velc.X <= 0)
            {
                Vector2.Subtract(Velc, new Vector2(2 * Velc.X, 0.0f));
                return true;
            }
            if(Position.X > (ViewportWidth - ColRadius + 0.01f) / 2 && Velc.X >= 0)
            {
                Vector2.Subtract(Velc, new Vector2(2 * Velc.X, 0.0f));
                return true;
            }
            if(Vector2.Distance(Position, pivotCenter) >= BorderRadius)
            {
                Vector2 norm = Vector2.Normalize(Vector2.Subtract(Position, pivotCenter));
                Velc = Vector2.Subtract(Velc, Vector2.Multiply(norm, Vector2.Dot(Velc, norm) * 2.0f));
                return true;
            }
            return false;
        }

        public void Move(GameTime gameTime)
        {
            Position = Vector2.Add(Position, Vector2.Multiply(Velc, (float)gameTime.ElapsedGameTime.TotalSeconds));
            //Gravity(1.0f);
        }
        public bool Gravity(float gravityAcc, GameTime gameTime)
        {
            Velc = Vector2.Add(Velc, new Vector2(0, gravityAcc * (float)gameTime.ElapsedGameTime.TotalSeconds));
            return true;
        }
    }
}

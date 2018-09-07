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
    class Button
    {
        public Texture2D ButtonTexture { set; get; }
        public Texture2D ButtonTextureAfterPressed { set; get; }
        public Vector2 Position { set; get; } //left top cornor
        public bool IsPressed { set; get; }
        public float Scale { set; get; }
        Rectangle ButtonArea;

        public Button(Texture2D ButtonTex, Vector2 Pos, float scale = 1.0f, Texture2D ButtonTexAfter = null)
        {
            ButtonTexture = ButtonTex;
            ButtonTextureAfterPressed = ButtonTexAfter;
            Position = Pos;
            Scale = scale;

            ButtonArea = new Rectangle(Position.ToPoint(), new Point((int)(ButtonTexture.Width * Scale), (int)(ButtonTexture.Height * Scale)));
            IsPressed = false;
        }
        public bool IsClicked(Vector2 InputPos)
        {
            if (ButtonArea.Contains(InputPos.ToPoint()))
            {
                return true;
            }
            return false;
        }

        public void SetPos(Vector2 pos)
        {
            Position = pos;
            ButtonArea = new Rectangle(Position.ToPoint(), new Point((int)(ButtonTexture.Width * Scale), (int)(ButtonTexture.Height * Scale)));
        }
    }
}
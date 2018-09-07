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
    class Animation
    {
        public string [] Sprites { set; get; }
        public string Name { set; get; }
        public int CurrentSprite { set; get; }
        public float TotalTimeSpan { set; get; }

        private float timePerFrame;
        private float CurrentTimePlayed;
        private bool isEnding;
        public Animation(string name, float totalTime, string [] sprites)
        {
            Name = name;
            TotalTimeSpan = totalTime;
            Sprites = sprites;
            timePerFrame = TotalTimeSpan / sprites.Length;
            CurrentSprite = 0;
            CurrentTimePlayed = 0.0f;
            isEnding = false;
        }

        public string GetCurrentSprite(GameTime gameTime)
        {
            CurrentTimePlayed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (CurrentTimePlayed > timePerFrame)
            {
                CurrentTimePlayed = 0.0f;
                CurrentSprite++;
                if (CurrentSprite == Sprites.Length)
                {
                    isEnding = true;
                    CurrentSprite = 0;
                }
                return Sprites[CurrentSprite];
            }
            else
                return Sprites[CurrentSprite];
        }

        public void Initialize()
        {
            CurrentSprite = 0;
            CurrentTimePlayed = 0.0f;
        }

        public bool IsEnding()
        {
            if (isEnding)
            {
                isEnding = false;
                return true;
            }
            else
                return false;
        }
    }
}
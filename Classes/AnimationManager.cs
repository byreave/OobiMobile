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
using TexturePackerLoader;
namespace OobiMobile.Classes
{
    class AnimationManager
    {
        public SpriteSheet SpriteSheet { set; get; }
        public Animation[] Anims { set; get; }
        public string CurrentAnimName{ set; get; }

        private int AnimIndex;
        public AnimationManager(string caName, Animation[] anims)
        {
            CurrentAnimName = caName;
            Anims = anims;
            AnimIndex = 0;
        }

        public void Play(string name)
        {
            for (int i = 0; i < Anims.Length; ++ i)
            {
                if(Anims[i].Name == name)
                {
                    AnimIndex = i;
                    CurrentAnimName = Anims[i].Name;
                    Anims[i].Initialize();
                    break;
                }
            }
        }

        public Texture2D Update(GameTime gameTime)
        {
            string sprite = Anims[AnimIndex].GetCurrentSprite(gameTime);
            if(Anims[AnimIndex].IsEnding())//Back to idle
            {
                AnimIndex = 0;
            }
            return SpriteSheet.Sprite(sprite).Texture;
        }
    }
}
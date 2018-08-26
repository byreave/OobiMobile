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
    class CollectibleGenerator
    {
        public double SecondsPerCollectible { set; get; } //duration of generating
        public Vector2 CollectibleVelc { set; get; } //starting speed
        public Vector2 StartPos { set; get; } //starting position
        public int[] TypesOfCollectible { set; get; } //types of enemies to generate
        public bool IsRandom { set; get; } //if the generating is random

        private bool isPause;
        private double timer;
        private int index;
        private float randomSpan;
        private Vector2 viewportSize;
        public CollectibleGenerator(int[] toe, Vector2 startP, Vector2 vel, Vector2 vSize, double seconds = 1.0f, float randSpan = 0.0f, bool isR = true)
        {
            TypesOfCollectible = toe;
            StartPos = startP;
            CollectibleVelc = vel;
            SecondsPerCollectible = seconds;
            IsRandom = isR;
            randomSpan = randSpan;
            viewportSize = vSize;

            isPause = false;
            timer = SecondsPerCollectible;
            index = 0;
        }


        public bool Generate(GameTime gameTime, List<Collectible> colList)
        {
            if (!isPause)
            {
                timer += gameTime.ElapsedGameTime.TotalSeconds; //to get time
                if (timer - SecondsPerCollectible < 0)
                    return false;
                else
                {
                    timer = 0;
                    if (IsRandom)
                    {
                        Random rand = new Random();
                        int ind = rand.Next(0, TypesOfCollectible.Length);
                        colList.Add(generateAnCollectible(ind));
                    }
                    else
                    {
                        if (index >= TypesOfCollectible.Length - 1)
                            index = 0;
                        colList.Add(generateAnCollectible(index++));
                    }
                    return true;
                }
            }
            return false;
        }

        private Collectible generateAnCollectible(int ind)
        {
            int CollectibleType = TypesOfCollectible[ind];
            Random ran = new Random();
            Vector2 randomFactor = StartPos;
            if (randomSpan != 0.0f)
            {
                float randStart = 0.0f;
                float randEnd = 0.0f;
                if (StartPos.X == 0.0f)
                {
                    randStart = (StartPos.Y - randomSpan) < 0 ? 0 : (StartPos.Y - randomSpan);
                    randEnd = (StartPos.Y + randomSpan) > viewportSize.Y ? viewportSize.Y : (StartPos.Y + randomSpan);
                    randomFactor = new Vector2(0.0f, randStart + (float)ran.NextDouble() * (randEnd - randStart));
                }
                if (StartPos.Y == 0.0f)
                {
                    randStart = (StartPos.X - randomSpan) < 0 ? 0 : (StartPos.X - randomSpan);
                    randEnd = (StartPos.X + randomSpan) > viewportSize.X ? viewportSize.X : (StartPos.X + randomSpan);
                    randomFactor = new Vector2(randStart + (float)ran.NextDouble() * (randEnd - randStart), 0.0f);
                }
            }


            Collectible gCollectible = new Collectible(CollectibleType, CollectibleVelc, randomFactor);
            return gCollectible;
        }

        public void Pause()
        {
            isPause = true;
        }

        public void UnPause()
        {
            isPause = false;
        }
    }
}
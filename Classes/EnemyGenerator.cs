﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OobiMobile.Classes
{
    class EnemyGenerator
    {
        public double SecondsPerEnemy { set; get; } //duration of generating
        public Vector2 EnemyVelc { set; get; } //starting speed
        public Vector2 StartPos { set; get; } //starting position
        public int[] TypesOfEnemy { set; get; } //types of enemies to generate
        public bool IsRandom { set; get; } //if the generating is random

        private bool isPause;
        private double timer;
        private int index;
        private float randomSpan;
        private Vector2 viewportSize;
        public EnemyGenerator(int [] toe, Vector2 startP, Vector2 vel, Vector2 vSize, double seconds = 1.0f, float randSpan = 0.0f, bool isR = true)
        {
            TypesOfEnemy = toe;
            StartPos = startP;
            EnemyVelc = vel;
            SecondsPerEnemy = seconds;
            IsRandom = isR;
            randomSpan = randSpan;
            viewportSize = vSize;

            isPause = false;
            timer = SecondsPerEnemy;
            index = 0;
        }

        
        public bool Generate(GameTime gameTime, List<Enemy> eneList)
        {
            if(!isPause)
            {
                timer += gameTime.ElapsedGameTime.TotalSeconds; //to get time
                if (timer - SecondsPerEnemy < 0)
                    return false;
                else
                {
                    timer = 0;
                    if (IsRandom)
                    {
                        Random rand = new Random();
                        int ind = rand.Next(0, TypesOfEnemy.Length);
                        eneList.Add(generateAnEnemy(ind));
                    }
                    else
                    {
                        if (index >= TypesOfEnemy.Length-1)
                            index = 0;
                        eneList.Add(generateAnEnemy(index++));
                    }
                    return true;
                }
            }
            return false;
        }

        private Enemy generateAnEnemy(int ind)
        {
            int enemyType = TypesOfEnemy[ind];
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
            
            
            Enemy gEnemy = new Enemy(enemyType, EnemyVelc, randomFactor);
            return gEnemy;
        }

        public void pause()
        {
            isPause = true;
        }

        public void unPause()
        {
            isPause = false;
        }
    }
}

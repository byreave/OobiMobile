using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Oobi.Classes
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
        public EnemyGenerator(int [] toe, Vector2 startP, Vector2 vel, double seconds = 1.0f, bool isR = true)
        {
            TypesOfEnemy = toe;
            StartPos = startP;
            EnemyVelc = vel;
            SecondsPerEnemy = seconds;
            IsRandom = isR;

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
                        int ind = rand.Next(0, TypesOfEnemy.Length-1);
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
            Enemy gEnemy = new Enemy(enemyType, EnemyVelc, StartPos);
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

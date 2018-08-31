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
    class RopeUnit
    {
        public Vector2 Position { set; get; }

        public float Radius { set; get; }
        public PhysicsSystem PhysicsSystem;
        public Vector2 Speed { set; get; }
        public RopeUnit RopeNodePrev { set; get; }
        public RopeUnit RopeNodeNext { set; get; }

        public RopeUnit(Vector2 pos, Vector2 velc, float rad)
        {
            Position = pos;
            Speed = velc;
            PhysicsSystem = new PhysicsSystem(1.0f, Vector2.Zero);
            Radius = rad;
        }
        public void Move(GameTime gTime)
        {
            Position += Speed * (float)gTime.ElapsedGameTime.TotalSeconds;
            if (Speed.X == 0)
                PhysicsSystem.Force = new Vector2(PhysicsSystem.Force.X / 2.0f, PhysicsSystem.Force.Y);
            if (Speed.Y == 0)
                PhysicsSystem.Force = new Vector2(PhysicsSystem.Force.X, PhysicsSystem.Force.Y / 2.0f);
        }
    }
}
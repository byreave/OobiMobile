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
    class PhysicsSystem
    {
        public const float GravAcc = 10.0f;
        public const float TensionK = 10.0f;
        public float Mass { set; get; }

        public Vector2 Force { set; get; }

        public PhysicsSystem(float m, Vector2 f)
        {
            Mass = m;
            Force = f;
        }
    }
}
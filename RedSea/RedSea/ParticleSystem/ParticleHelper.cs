﻿#region File Description
//-----------------------------------------------------------------------------
// ParticleHelper.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace RedSeaGame
{
    public static class ParticleHelper
    {
       public static readonly Random Random = new Random();

         public static float RandomBetween(float min, float max)
         {
             return min + (float)Random.NextDouble() * (max - min);
         }
     }
}

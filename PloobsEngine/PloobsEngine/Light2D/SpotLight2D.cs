﻿#region License
/*
    PloobsEngine Game Engine Version 0.3 Beta
    Copyright (C) 2011  Ploobs

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PloobsEngine.Light2D
{
    /// <summary>
    /// 2D sport light
    /// </summary>
    public class SpotLight2D : Light2D
    {
        public Vector2 Direction;
        public float Angle;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpotLight2D"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="color">The color.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="angle">The angle. IN RADIANS !!!!!!!!!!!!!!!</param>
        /// <param name="intensity">The intensity.</param>
        /// <param name="ShadowmapSize">Size of the shadowmap.</param>
        public SpotLight2D(Vector2 position, Color color, Vector2 direction ,float angle,float intensity = 1, ShadowmapSize ShadowmapSize = ShadowmapSize.Size512)
            : base(position, color, intensity, ShadowmapSize)
        {
            this.Angle = angle;
            this.Direction = direction;            
        }
    }
}

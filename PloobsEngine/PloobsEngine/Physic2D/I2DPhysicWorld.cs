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

namespace PloobsEngine.Physic2D
{
    /// <summary>
    /// 2D physic world specification
    /// </summary>
    public abstract class I2DPhysicWorld
    {

        /// <summary>
        /// Updates 
        /// </summary>
        /// <param name="gt">The gt.</param>
        protected abstract void Update(GameTime gt);
        internal void iUpdate(GameTime gt)
        {
            Update(gt);
        }

        /// <summary>
        /// Adds the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public abstract void AddObject(I2DPhysicObject obj);

        /// <summary>
        /// Removes the object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public abstract void RemoveObject(I2DPhysicObject obj);

        /// <summary>
        /// Test a Point agains the world
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public abstract I2DPhysicObject Picking(Vector2 point);


        /// <summary>
        /// Tests the AABB against the world.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        public abstract List<I2DPhysicObject> TestAABB(Vector2 min, Vector2 max);
    }
}

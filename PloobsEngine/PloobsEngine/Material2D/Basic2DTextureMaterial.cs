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
using Microsoft.Xna.Framework.Graphics;
using PloobsEngine.Physics2D.Farseer;
using FarseerPhysics.Common;
using PloobsEngine.Engine;

namespace PloobsEngine.Material2D
{
    public class Basic2DTextureMaterial : I2DMaterial
    {

        public Basic2DTextureMaterial()
        {
            CastShadow = true;

        }

        /// <summary>
        /// Gets or sets a value indicating whether [cast shadow].
        /// Default true
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cast shadow]; otherwise, <c>false</c>.
        /// </value>
        public bool CastShadow
        {
            set;
            get;
        }


        public override void Draw(GameTime gt, SceneControl._2DScene.I2DObject obj, SceneControl.RenderHelper render)
        {
            render.RenderTexture(obj.Modelo.Texture, obj.PhysicObject.Position, obj.Modelo.SourceRectangle, Color.White, obj.PhysicObject.Rotation + obj.Modelo.Rotation, obj.Modelo.Origin + obj.PhysicObject.Origin, 1, SpriteEffects.None, obj.Modelo.LayerDepth);
        }
        
        public override void LightDraw(GameTime gt, SceneControl._2DScene.I2DObject obj, SceneControl.RenderHelper render, Color color, PloobsEngine.Light2D.Light2D light)
        {
            if(CastShadow)
                render.RenderTexture(obj.Modelo.Texture, light.ToRelativePosition(obj.PhysicObject.Position), obj.Modelo.SourceRectangle, color, obj.PhysicObject.Rotation + obj.Modelo.Rotation, obj.Modelo.Origin + obj.PhysicObject.Origin, 1);
        }
    }

    public class Basic2DTextureMaterialProcessor : IMaterialProcessor
    {
        #region IMaterialProcessor Members

        public void ProcessDraw(GameTime gameTime, SceneControl.RenderHelper render, SceneControl._2DScene.ICamera2D camera, List<SceneControl._2DScene.I2DObject> objects)
        {
            render.RenderBegin(camera.View, null);
            foreach (var iobj in objects)
            {
                if (iobj.PhysicObject.Enabled == true)
                {
                    iobj.Material.Draw(gameTime, iobj, render);
                }
            }
            render.RenderEnd();
        }

        public void ProcessPreDraw(GameTime gameTime, SceneControl.RenderHelper render, SceneControl._2DScene.ICamera2D camera, SceneControl._2DScene.I2DWorld world, List<SceneControl._2DScene.I2DObject> objects)
        {
            render.RenderBegin(camera.View, null);
            foreach (var iobj in objects)
            {
                if (iobj.PhysicObject.Enabled == true)
                {
                    iobj.Material.PreDrawnPhase(gameTime, world,iobj, render);
                }
            }
            render.RenderEnd();
        }

        public void ProcessLightDraw(GameTime gameTime, SceneControl.RenderHelper render, SceneControl._2DScene.ICamera2D camera, List<SceneControl._2DScene.I2DObject> objects, Color color, PloobsEngine.Light2D.Light2D light)
        {
            render.RenderBegin(camera.View, null);
            foreach (var iobj in objects)
            {
                if (iobj.PhysicObject.Enabled == true)
                {
                    iobj.Material.LightDraw(gameTime, iobj, render, color, light);
                }
            }
            render.RenderEnd();
        }

        #endregion
    }
}

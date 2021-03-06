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
using PloobsEngine.Components;

namespace PloobsEngine.Features.DebugDraw
{
    /// <summary>
    /// Component responsible for drawing 3D primitives on the top of the screen (with depth testing enabled)
    /// </summary>
    public class DebugDraw : IComponent
    {
        private List<DebugShapesDrawer> debugDrawers = new List<DebugShapesDrawer>();

        Engine.GraphicInfo GraphicInfo;
        Engine.GraphicFactory factory;
        
        protected override void LoadContent(Engine.GraphicInfo GraphicInfo, Engine.GraphicFactory factory)
        {
            this.factory = factory;
            this.GraphicInfo = GraphicInfo;
            base.LoadContent(GraphicInfo, factory);
        }

        internal void RegisterDebugDrawer(DebugShapesDrawer debugDrawer)
        {
            debugDrawer.Initialize(factory, GraphicInfo);
            debugDrawers.Add(debugDrawer);
        }
        
        public override ComponentType ComponentType
        {
            get { return Components.ComponentType.POS_WITHDEPTH_DRAWABLE; }
        }

        public override string getMyName()
        {
            return MyName;
        }

        public static readonly String MyName = "DebugDraw";

        protected override void PosWithDepthDraw(SceneControl.RenderHelper render, Microsoft.Xna.Framework.GameTime gt, ref Microsoft.Xna.Framework.Matrix activeView, ref Microsoft.Xna.Framework.Matrix activeProjection)
        {
            base.PosWithDepthDraw(render, gt, ref activeView, ref activeProjection);

            foreach (var item in debugDrawers)
            {
                foreach (var shape in item.GetShapes())
                {
                    shape.Draw(render, activeView, activeProjection);    
                }

                item.EndFrame();
            }

        }

    }
}

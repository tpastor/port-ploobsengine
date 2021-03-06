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
#if !WINDOWS_PHONE && !REACH
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PloobsEngine.SceneControl
{
    /// <summary>
    /// Wiggle Post effect
    /// </summary>
    public class WigglePostEffect : IPostEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WigglePostEffect"/> class.
        /// </summary>
        public WigglePostEffect() : base(PostEffectType.All) { }
        private Effect wiggle;
        private float m_Timer = 0;        

#region IPostEffect Members

        /// <summary>
        /// Draws 
        /// </summary>
        /// <param name="ImageToProcess">The image to process.</param>
        /// <param name="rHelper">The r helper.</param>
        /// <param name="gt">The gt.</param>
        /// <param name="GraphicInfo">The graphic info.</param>
        /// <param name="world">The world.</param>
        /// <param name="useFloatingBuffer">if set to <c>true</c> [use floating buffer].</param>
        public override void Draw(Texture2D ImageToProcess, RenderHelper rHelper, GameTime gt, Engine.GraphicInfo GraphicInfo, IWorld world, bool useFloatingBuffer)
        {

            m_Timer += (float)gt.ElapsedGameTime.Milliseconds / 500;

            wiggle.Parameters["fTimer"].SetValue(m_Timer);
            if (useFloatingBuffer)
                rHelper.RenderTextureToFullScreenSpriteBatch(ImageToProcess, wiggle, GraphicInfo.FullScreenRectangle, SamplerState.PointClamp);
            else
                rHelper.RenderTextureToFullScreenSpriteBatch(ImageToProcess, wiggle, GraphicInfo.FullScreenRectangle, GraphicInfo.SamplerState);
            
            
        }

        #endregion

#region IPostEffect Members        
        public override void Init(Engine.GraphicInfo ginfo, Engine.GraphicFactory factory)
        {            
            this.wiggle = factory.GetEffect("wiggle",false,true);         
        }

        #endregion
    }
}
#endif
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
using PloobsEngine.Engine.Logger;
using Microsoft.Xna.Framework.Graphics;
using PloobsEngine.Engine;

namespace PloobsEngine.SceneControl
{    

    public struct ForwardRenderTecnichDescription
    {
#if !WINDOWS_PHONE && !REACH
        public static ForwardRenderTecnichDescription Default()
        {
            return new ForwardRenderTecnichDescription(Color.Black, true);
        }
#else
        public static ForwardRenderTecnichDescription Default()
        {
            return new ForwardRenderTecnichDescription(Color.Black, false);
        }
        #endif

        internal ForwardRenderTecnichDescription(Color BackGroundColor,bool usePostEffect = true)
        {
            this.BackGroundColor = BackGroundColor;
            this.UsePostEffect = usePostEffect;
            UsePreDrawPhase = false;
            UsePostDrawPhase = false;
            OrderAllObjectsBeforeDraw = null;            
        }
        public bool UsePostEffect;
        public Color BackGroundColor;
        public bool UsePreDrawPhase ;
        public bool UsePostDrawPhase;


        /// <summary>
        /// Function called all frames to order all objects that are not culled
        /// Use this to sort objects by material and minimize gpu state changes
        /// </summary>
        public Func<List<IObject>, IWorld, List<IObject>> OrderAllObjectsBeforeDraw;

    }

    public class ForwardRenderTecnich : IRenderTechnic
    {
        public ForwardRenderTecnich(ForwardRenderTecnichDescription desc)
#if !WINDOWS_PHONE && !REACH
            : base(PostEffectType.Forward3D)
#else
         : base(PostEffectType.WindowsPhoneAndReach)
#endif

        {
            this.desc = desc;
        }

        ForwardRenderTecnichDescription desc;

        private RenderTarget2D target;
        private RenderTarget2D target2; ///for ping pong with target       
        private RenderTarget2D PostEffectTarget;                                       
        RenderTarget2D renderTarget;        
        Engine.GraphicFactory factory;

        Engine.GraphicInfo ginfo;
        #region IRenderTechnic Members

        protected override void AfterLoadContent(IContentManager manager, Engine.GraphicInfo ginfo, Engine.GraphicFactory factory)
        {
            this.ginfo = ginfo;
            this.factory = factory;
            this.ginfo.OnGraphicInfoChange+=new EventHandler(ginfo_OnGraphicInfoChange); ;

            if (desc.UsePostEffect)
            {
                renderTarget = factory.CreateRenderTarget(ginfo.BackBufferWidth,ginfo.BackBufferHeight,SurfaceFormat.Color,ginfo.UseMipMap,DepthFormat.Depth24Stencil8,ginfo.MultiSample,RenderTargetUsage.DiscardContents);
                target = factory.CreateRenderTarget(ginfo.BackBufferWidth, ginfo.BackBufferHeight, SurfaceFormat.Color, ginfo.UseMipMap, DepthFormat.Depth24Stencil8, ginfo.MultiSample, RenderTargetUsage.DiscardContents);
                target2 = factory.CreateRenderTarget(ginfo.BackBufferWidth, ginfo.BackBufferHeight, SurfaceFormat.Color, ginfo.UseMipMap, DepthFormat.Depth24Stencil8, ginfo.MultiSample, RenderTargetUsage.DiscardContents);
            }

            PostEffectTarget = target;                        
            base.AfterLoadContent(manager, ginfo, factory);
        }

        void ginfo_OnGraphicInfoChange(object sender, EventArgs e)
        {
            GraphicInfo newGraphicInfo = (GraphicInfo)sender;
            if (renderTarget != null && PostEffectTarget != null)
            {
                bool equal = false;
                if (PostEffectTarget == target)
                    equal = true;

                renderTarget.Dispose();                
                target.Dispose();
                target2.Dispose();
                renderTarget = factory.CreateRenderTarget(ginfo.BackBufferWidth, ginfo.BackBufferHeight, SurfaceFormat.Color, ginfo.UseMipMap, DepthFormat.Depth24Stencil8, ginfo.MultiSample, RenderTargetUsage.DiscardContents);
                target = factory.CreateRenderTarget(ginfo.BackBufferWidth, ginfo.BackBufferHeight, SurfaceFormat.Color, ginfo.UseMipMap, DepthFormat.Depth24Stencil8, ginfo.MultiSample, RenderTargetUsage.DiscardContents);
                target2 = factory.CreateRenderTarget(ginfo.BackBufferWidth, ginfo.BackBufferHeight, SurfaceFormat.Color, ginfo.UseMipMap, DepthFormat.Depth24Stencil8, ginfo.MultiSample, RenderTargetUsage.DiscardContents);
                if (equal)
                    PostEffectTarget = target;
                else
                    PostEffectTarget = target2;
            }
        }
        
        protected override void ExecuteTechnic(GameTime gameTime, RenderHelper render, IWorld world)
        {
            Matrix view = world.CameraManager.ActiveCamera.View;
            Matrix projection = world.CameraManager.ActiveCamera.Projection;

            world.Culler.StartFrame(ref view, ref projection, world.CameraManager.ActiveCamera.BoundingFrustum);
            List<IObject> objList = world.Culler.GetNotCulledObjectsList(Material.MaterialType.FORWARD,CullerComparer.ComparerFrontToBack,world.CameraManager.ActiveCamera.Position);

            if (desc.OrderAllObjectsBeforeDraw != null)
                objList = desc.OrderAllObjectsBeforeDraw(objList,world);

            if (desc.UsePreDrawPhase)
            {
                for (int i = 0; i < objList.Count; i++)
                {
                    objList[i].Material.PreDrawnPhase(gameTime, world, objList[i], world.CameraManager.ActiveCamera, world.Lights, render);    
                }
            }

            render.DettachBindedTextures(6);
            render.SetSamplerStates(ginfo.SamplerState);            

            if (desc.UsePostEffect)
            {
                render.PushRenderTarget(renderTarget);                
            }

            render.Clear(desc.BackGroundColor);
            render.RenderPreComponents(gameTime, ref view, ref projection);                        
            foreach (var item in objList)
            {
                System.Diagnostics.Debug.Assert(item.Material.MaterialType == Material.MaterialType.FORWARD, "This Technich is just for forward materials and shaders");
                if(item.Material.IsVisible)
                    item.Material.Drawn(gameTime,item, world.CameraManager.ActiveCamera, world.Lights, render);                
            }

            render.DettachBindedTextures(6);
            render.SetSamplerStates(ginfo.SamplerState);

            if (world.PhysicWorld.isDebugDraw)
            {
                world.PhysicWorld.iDebugDrawn(render, gameTime, world.CameraManager.ActiveCamera);
                render.DettachBindedTextures(6);
                render.SetSamplerStates(ginfo.SamplerState);            
            }

            if (desc.UsePostDrawPhase)
            {
                for (int i = objList.Count -1 ; i >= 0; i--)
                {
                    if(objList[i].Material.IsVisible)
                        objList[i].Material.PosDrawnPhase(gameTime, objList[i], world.CameraManager.ActiveCamera, world.Lights, render);
                }
            }

            if (world.ParticleManager != null)
            {
                world.ParticleManager.iDraw(gameTime, world.CameraManager.ActiveCamera.View, world.CameraManager.ActiveCamera.Projection, render);
                render.DettachBindedTextures();
                render.SetSamplerStates(ginfo.SamplerState);            
            }

            render.RenderPosWithDepthComponents(gameTime, ref view,ref projection);

            render.DettachBindedTextures();
            render.SetSamplerStates(ginfo.SamplerState);            

            if (desc.UsePostEffect)
            {
                render[PrincipalConstants.CurrentImage] = render.PopRenderTarget()[0].RenderTarget as Texture2D;
                render[PrincipalConstants.CombinedImage] = render[PrincipalConstants.CurrentImage];
                for (int i = 0; i < PostEffects.Count; i++)
                {
                    if (PostEffects[i].Enabled)
                    {
                        render.PushRenderTarget(PostEffectTarget);
                        render.Clear(Color.Black);
                        PostEffects[i].Draw(render[PrincipalConstants.CurrentImage], render, gameTime, ginfo, world, false);
                        Texture2D tex = render.PopRenderTarget()[0].RenderTarget as Texture2D;
                        render[PrincipalConstants.CurrentImage] = tex;
                        SwapTargetBuffers();
                    }
                }
                render.Clear(Color.Black);
                render.RenderTextureComplete(render[PrincipalConstants.CurrentImage], Color.White, ginfo.FullScreenRectangle, Matrix.Identity, null, true, SpriteSortMode.Deferred, ginfo.SamplerState);                                             
            }

            render.DettachBindedTextures(2);
            render.SetSamplerStates(ginfo.SamplerState);            

            render.RenderPosComponents(gameTime, ref view, ref projection);

            render.DettachBindedTextures(2);
            render.SetSamplerStates(ginfo.SamplerState) ;            
        }

        private void SwapTargetBuffers()
        {
            if (PostEffectTarget == target)
                PostEffectTarget = target2;
            else
                PostEffectTarget = target;
        }
        
        public override string TechnicName
        {
            get { return "ForwardTechnich"; }
        }

        #endregion

        public override void CleanUp()
        {
            this.ginfo.OnGraphicInfoChange -= ginfo_OnGraphicInfoChange;
            if (renderTarget != null && PostEffectTarget != null)
            {
                renderTarget.Dispose();
                target.Dispose();
                target2.Dispose();
            }

            for (int i = 0; i < PostEffects.Count; i++)
            {
                PostEffects[i].CleanUp();
            }
        }
    }
}


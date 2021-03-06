﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PloobsEngine.SceneControl;
using PloobsEngine.Physics;
using PloobsEngine;
using PloobsEngine.Cameras;
using PloobsEngine.Input;
using Microsoft.Xna.Framework;
using PloobsEngine.Physics.Bepu;
using PloobsEngine.Modelo;
using PloobsEngine.Material;
using Microsoft.Xna.Framework.Graphics;
using PloobsEngine.Utils;
using PloobsEngine.Features;
using PloobsEngine.Commands;
using PloobsEngine.Light;
using PloobsEngine.Physic.PhysicObjects.BepuObject;
using BEPUphysics.UpdateableSystems.ForceFields;
using PloobsEngine.Engine;
using EngineTestes;

namespace IntroductionDemo4._0
{
    /// <summary>
    /// Gravitation Screen
    /// Shows how to use a specific feature of Bepu Physics
    /// </summary>
    public class GravitationalBepuScreen : IScene
    {        
        ICamera cam;

        protected override void SetWorldAndRenderTechnich(out IRenderTechnic renderTech, out IWorld world)
        {
            world = new IWorld(new BepuPhysicWorld(0), new SimpleCuller());

            ///Create the deferred technich
            ForwardRenderTecnichDescription desc = new ForwardRenderTecnichDescription();
            desc.BackGroundColor = Color.AliceBlue;
            renderTech = new ForwardRenderTecnich(desc);
        }
        
        protected override void LoadContent(PloobsEngine.Engine.GraphicInfo GraphicInfo, PloobsEngine.Engine.GraphicFactory factory, IContentManager contentManager)
        {
            base.LoadContent(GraphicInfo, factory, contentManager);

            {
                SimpleModel sm = new SimpleModel(factory, "..\\Content\\Model\\ball");
                sm.SetTexture(factory.CreateTexture2DColor(1, 1, Color.Blue), TextureType.DIFFUSE);
                IPhysicObject pi = new SphereObject(new Vector3(0, 0, 0), 1, 10, 30, MaterialDescription.DefaultBepuMaterial());
                pi.isMotionLess = true;
                ForwardXNABasicShader shader = new ForwardXNABasicShader();
                IMaterial mat = new ForwardMaterial(shader);
                IObject obj3 = new IObject(mat, sm, pi);
                this.World.AddObject(obj3);
                shader.BasicEffect.EnableDefaultLighting();

                BepuPhysicWorld physicWorld;
                physicWorld = this.World.PhysicWorld as BepuPhysicWorld;
                System.Diagnostics.Debug.Assert(physicWorld != null);
                var field = new GravitationalFieldObject(new InfiniteForceFieldShape(), obj3.PhysicObject.Position, 66730 / 2f, 10000, physicWorld);
                ///This Method is from BepuPhysicWorld not from th IPhysicObject
                ///You can use everithing from Bepu using this object instead of the interface
                ///but take care, the engine dont know about THIS !!! it does not manage these things
                physicWorld.Space.Add(field);

            }

            int numColumns = 7;
            int numRows = 7;
            int numHigh = 7;
            float separation = 3;
            {
                ///reuse instances that does not change
                SimpleModel sm = new SimpleModel(factory, "..\\Content\\Model\\cubo");
                sm.SetTexture(factory.CreateTexture2DColor(1, 1, Color.White), TextureType.DIFFUSE);
                ForwardXNABasicShader shader = new ForwardXNABasicShader();
                IMaterial mat = new ForwardMaterial(shader);

                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numColumns; j++)
                    {
                        for (int k = 0; k < numHigh; k++)
                        {
                            BoxObject pi = new BoxObject(new Vector3(separation * i - numRows * separation / 2, 40 + k * separation, separation * j - numColumns * separation / 2), 1, 1, 1, 5, new Vector3(1), Matrix.Identity, MaterialDescription.DefaultBepuMaterial());
                            pi.Entity.LinearDamping = 0;
                            pi.Entity.AngularDamping = 0;                            
                            IObject obj3 = new IObject(mat, sm, pi);
                            this.World.AddObject(obj3);
                            pi.Entity.LinearVelocity = new Vector3(30, 0, 0);                            
                        }
                    }
                }

                shader.BasicEffect.EnableDefaultLighting();
            }

            //cam = new CameraFirstPerson(GraphicInfo);
            //cam.FarPlane = 3000;

            #region NormalLight
            DirectionalLightPE ld1 = new DirectionalLightPE(Vector3.Left, Color.White);
            DirectionalLightPE ld2 = new DirectionalLightPE(Vector3.Right, Color.White);
            DirectionalLightPE ld3 = new DirectionalLightPE(Vector3.Backward, Color.White);
            DirectionalLightPE ld4 = new DirectionalLightPE(Vector3.Forward, Color.White);
            DirectionalLightPE ld5 = new DirectionalLightPE(Vector3.Down, Color.White);
            float li = 0.4f;
            ld1.LightIntensity = li;
            ld2.LightIntensity = li;
            ld3.LightIntensity = li;
            ld4.LightIntensity = li;
            ld5.LightIntensity = li;
            this.World.AddLight(ld1);
            this.World.AddLight(ld2);
            this.World.AddLight(ld3);
            this.World.AddLight(ld4);
            this.World.AddLight(ld5);
            #endregion

            RotatingCamera cam = new RotatingCamera(this,new Vector3(0,0,-300));
            this.World.CameraManager.AddCamera(cam);

        }

        protected override void Draw(GameTime gameTime, RenderHelper render)
        {
            base.Draw(gameTime, render);
            render.RenderTextComplete("Demo: Planetary Gravity (BEPU)", new Vector2(GraphicInfo.Viewport.Width - 315, 15), Color.White,Matrix.Identity);                                    
        }
    }
}


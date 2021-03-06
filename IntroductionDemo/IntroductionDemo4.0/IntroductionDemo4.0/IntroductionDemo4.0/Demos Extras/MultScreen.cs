﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PloobsEngine.SceneControl;
using PloobsEngine.Physics;
using PloobsEngine.Modelo;
using PloobsEngine.Material;
using PloobsEngine.Engine;
using PloobsEngine.Physics.Bepu;
using Microsoft.Xna.Framework;
using PloobsEngine.Cameras;
using Microsoft.Xna.Framework.Graphics;
using PloobsEngine.Features;

namespace IntroductionDemo4._0
{
    public class MultScreen : IScene
    {
        protected override void SetWorldAndRenderTechnich(out IRenderTechnic renderTech, out IWorld world)
        {
            ///enable multiThread HERE
            world = new IWorld(new BepuPhysicWorld(-9.8f,false,1,true), new SimpleCuller(),null,true);

            ForwardRenderTecnichDescription desc = ForwardRenderTecnichDescription.Default();
            desc.BackGroundColor = Color.CornflowerBlue;
            renderTech = new ForwardRenderTecnich(desc);            
        }

        protected override void InitScreen(PloobsEngine.Engine.GraphicInfo GraphicInfo, PloobsEngine.Engine.EngineStuff engine)
        {
            FPSCounter fp = new FPSCounter();
            fp.CombinedFps += new FpsEvent(fp_CombinedFps);
            engine.AddComponent(fp);
            engine.IsFixedTimeStep = false;
            base.InitScreen(GraphicInfo, engine);
        }

        void fp_CombinedFps(float fps)
        {
            this.fps = fps;
        }
        float fps;

        protected override void LoadContent(GraphicInfo GraphicInfo, GraphicFactory factory ,IContentManager contentManager)
        {
            base.LoadContent(GraphicInfo, factory, contentManager);                      

            {
                SimpleModel simpleModel = new SimpleModel(factory, "Model//cenario");
                TriangleMeshObject tmesh = new TriangleMeshObject(simpleModel, Vector3.Zero, Matrix.Identity, Vector3.One, MaterialDescription.DefaultBepuMaterial());
                ForwardXNABasicShader shader = new ForwardXNABasicShader(ForwardXNABasicShaderDescription.Default());
                ForwardMaterial fmaterial = new ForwardMaterial(shader);
                IObject obj = new IObject(fmaterial, simpleModel, tmesh);
                this.World.AddObject(obj);
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {                   
                    SimpleModel simpleModel = new SimpleModel(factory, "Model//uzi");
                    TriangleMeshObject tmesh = new TriangleMeshObject(simpleModel, new Vector3(25 * j, 30,15 * i), Matrix.Identity, Vector3.One * 10, MaterialDescription.DefaultBepuMaterial());
                    ForwardXNABasicShader shader = new ForwardXNABasicShader(ForwardXNABasicShaderDescription.Default());
                    ForwardMaterial fmaterial = new ForwardMaterial(shader);
                    IObject obj = new IObject(fmaterial, simpleModel, tmesh);
                    obj.OnUpdate += new OnUpdate(obj_OnUpdate);
                    this.World.AddObject(obj);
                }
            }

            this.World.CameraManager.AddCamera(new CameraFirstPerson(GraphicInfo));
            
        }

        void obj_OnUpdate(IObject obj, GameTime gt, ICamera cam)
        {
            ///simulating heavy work ...
            for (int i = 0; i < 1000; i++)
            {
                double j = Math.Atan(i) * Math.Cos(i*i * 4);
            }          
        }

        protected override void Draw(GameTime gameTime, RenderHelper render)
        {        
            base.Draw(gameTime, render);

            Texture2D logo = GraphicFactory.GetTexture2D("Textures\\engine_logo");
            int wd = 64;
            int hg = 48;
            render.RenderTextureComplete(logo, new Rectangle(this.GraphicInfo.BackBufferWidth - wd, this.GraphicInfo.BackBufferHeight - hg, wd, hg));

            render.RenderTextComplete("Demo: Enabling MultiThreading", new Vector2(GraphicInfo.Viewport.Width - 515, 15), Color.White, Matrix.Identity);
            render.RenderTextComplete("FPS: " + fps, new Vector2(GraphicInfo.Viewport.Width - 515, 35), Color.White, Matrix.Identity);
            
        }
        protected override void CleanUp(EngineStuff engine)
        {
            base.CleanUp(engine);
            engine.RemoveComponent(FPSCounter.MyName);
        }

    }
}















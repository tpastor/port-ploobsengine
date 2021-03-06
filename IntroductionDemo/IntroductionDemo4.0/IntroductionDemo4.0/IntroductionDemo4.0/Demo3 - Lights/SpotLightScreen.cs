﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PloobsEngine;
using PloobsEngine.Cameras;
using PloobsEngine.Light;
using PloobsEngine.Material;
using PloobsEngine.Modelo;
using PloobsEngine.Physics;
using PloobsEngine.Physics.Bepu;
using PloobsEngine.SceneControl;
using PloobsEngine.Input;

namespace IntroductionDemo4._0
{
    /// <summary>
    /// SpotLightPEScreen
    /// Mostra o uso das SpotLightPE com um exemplo simples de movimentacao
    /// </summary>
    public class SpotLightScreen : IScene
    {        
        LightThrowBepu lt;


        protected override void SetWorldAndRenderTechnich(out IRenderTechnic renderTech, out IWorld world)
        {
            world = new IWorld(new BepuPhysicWorld(), new SimpleCuller());
            DeferredRenderTechnicInitDescription desc = DeferredRenderTechnicInitDescription.Default();            
            desc.UseFloatingBufferForLightMap = false;
            renderTech = new DeferredRenderTechnic(desc) ;   
        }


        protected override void LoadContent(PloobsEngine.Engine.GraphicInfo GraphicInfo, PloobsEngine.Engine.GraphicFactory factory, IContentManager contentManager)
        {
            base.LoadContent(GraphicInfo, factory, contentManager);
        
            ///Create a Simple Model
            IModelo sm = new SimpleModel(factory, "..\\Content\\Model\\cenario");            
            ///Create a Physic Object
            IPhysicObject pi = new TriangleMeshObject(sm, Vector3.Zero, Matrix.Identity, Vector3.One,MaterialDescription.DefaultBepuMaterial());
            ///Create a shader 
            IShader shader = new DeferredNormalShader();            
            ///Create a Material
            IMaterial mat = new DeferredMaterial(shader);
            ///Create a an Object that englobs everything and add it to the world
            IObject obj4 = new IObject(mat, sm, pi);
            this.World.AddObject(obj4);

            lt = new LightThrowBepu(this.World,factory);

            ///Create a FirstPerson Camera
            ///This is a special camera, used in the development
            ///You can move around using wasd / qz / and the mouse
            ICamera cam = new CameraFirstPerson(GraphicInfo);
            this.World.CameraManager.AddCamera(cam);

            {
                SpotLightPE sp1 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.YellowGreen, 1, 600, (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc1 = new SpotLightCircularUpdater(this, sp1, 0.01f, 1, 0, true);
                this.World.AddLight(sp1);

                SpotLightPE sp2 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0),Color.Red, 1, 600, (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc2 = new SpotLightCircularUpdater(this, sp2, 0.01f, 1, (float)Math.PI / 2, true);
                this.World.AddLight(sp2);

                SpotLightPE sp3 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.Red, 1, 600, (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc3 = new SpotLightCircularUpdater(this, sp3, 0.01f, 1, (float)Math.PI, true);
                this.World.AddLight(sp3);

                SpotLightPE sp4 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.Green, 1, 600, (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc4 = new SpotLightCircularUpdater(this, sp4, 0.01f, 1, (float)(Math.PI * 3) / 2, true);
                this.World.AddLight(sp4);
            }

            {
                SpotLightPE sp1 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.Purple, 1, 600, (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc1 = new SpotLightCircularUpdater(this, sp1, 0.02f, 2, 0, false);
                this.World.AddLight(sp1);

                SpotLightPE sp2 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.PowderBlue, 1, 600, (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc2 = new SpotLightCircularUpdater(this, sp2, 0.02f, 2, (float)Math.PI / 2, false);
                this.World.AddLight(sp2);

                SpotLightPE sp3 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0),Color.YellowGreen, 1, 600 , (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc3 = new SpotLightCircularUpdater(this, sp3, 0.02f, 2, (float)Math.PI, false);
                this.World.AddLight(sp3);

                SpotLightPE sp4 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.Maroon, 1, 600, (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc4 = new SpotLightCircularUpdater(this, sp4, 0.02f, 2, (float)(Math.PI * 3) / 2, false);
                this.World.AddLight(sp4);
            }


            {
                SpotLightPE sp1 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.PapayaWhip,1, 600 , (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc1 = new SpotLightCircularUpdater(this, sp1, 0.03f, 3, (float)Math.PI / 4, true);
                this.World.AddLight(sp1);

                SpotLightPE sp2 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.LightSeaGreen, 1, 600, (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc2 = new SpotLightCircularUpdater(this, sp2, 0.03f, 3, (float)Math.PI / 4 + (float)Math.PI / 2, true);
                this.World.AddLight(sp2);

                SpotLightPE sp3 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.Gold, 1, 600, (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc3 = new SpotLightCircularUpdater(this, sp3, 0.03f, 3, (float)Math.PI / 4 + (float)Math.PI, true);
                this.World.AddLight(sp3);

                SpotLightPE sp4 = new SpotLightPE(new Vector3(0, 150, 0), new Vector3(0, -1, 0), Color.Aqua,1, 600 , (float)Math.Cos(Math.PI / 7), 0.5f);
                SpotLightCircularUpdater spc4 = new SpotLightCircularUpdater(this, sp4, 0.03f, 3, (float)Math.PI / 4 + (float)(Math.PI * 3) / 2, true);
                this.World.AddLight(sp4);
            }

            this.RenderTechnic.AddPostEffect(new AntiAliasingPostEffect());
        }

        protected override void Draw(GameTime gameTime, RenderHelper render)
        {
            base.Draw(gameTime, render);


            Texture2D logo = GraphicFactory.GetTexture2D("Textures\\engine_logo");
            int wd = 64;
            int hg = 48;
            render.RenderTextureComplete(logo, new Rectangle(this.GraphicInfo.BackBufferWidth - wd, this.GraphicInfo.BackBufferHeight - hg, wd, hg));

            render.RenderTextComplete("Spot Lights", new Vector2(GraphicInfo.Viewport.Width - 315, 15), Color.White,Matrix.Identity);                   
        }

        protected override void CleanUp(PloobsEngine.Engine.EngineStuff engine)
        {
            lt.CleanUp();
        }
    }
}

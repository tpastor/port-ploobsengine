﻿using Microsoft.Xna.Framework;
using PloobsEngine.Cameras;
using PloobsEngine.Light;
using PloobsEngine.Physics;
using PloobsEngine.SceneControl;
using PloobsEngine.Modelo;
using PloobsEngine.Physics.Bepu;
using PloobsEngine.Material;
using Microsoft.Xna.Framework.Input;
using System;

namespace ProjectTemplate
{
    /// <summary>
    /// Basic Deferred Scene
    /// </summary>
    public class ShatterSample : IScene
    {
        DeferredShatterCustomShader shader; 
        /// <summary>
        /// Sets the world and render technich.
        /// </summary>
        /// <param name="renderTech">The render tech.</param>
        /// <param name="world">The world.</param>
        protected override void SetWorldAndRenderTechnich(out IRenderTechnic renderTech, out IWorld world)
        {
            ///create the world using bepu as physic api and a simple culler implementation
            ///IT DOES NOT USE PARTICLE SYSTEMS (see the complete constructor, see the ParticleDemo to know how to add particle support)
            world = new IWorld(new BepuPhysicWorld(), new SimpleCuller());

            ///Create the deferred description
            DeferredRenderTechnicInitDescription desc = DeferredRenderTechnicInitDescription.Default();
            ///Some custom parameter, this one allow light saturation. (and also is a pre requisite to use hdr)
            desc.UseFloatingBufferForLightMap = true;
            ///set background color, default is black
            desc.BackGroundColor = Color.CornflowerBlue;
            ///create the deferred technich
            renderTech = new DeferredRenderTechnic(desc);
        }

        /// <summary>
        /// Load content for the screen.
        /// </summary>
        /// <param name="GraphicInfo"></param>
        /// <param name="factory"></param>
        /// <param name="contentManager"></param>
        protected override void LoadContent(PloobsEngine.Engine.GraphicInfo GraphicInfo, PloobsEngine.Engine.GraphicFactory factory, IContentManager contentManager)
        {
            ///must be called before all
            base.LoadContent(GraphicInfo, factory, contentManager);

            {
                ///Create a simple object
                ///Geomtric Info and textures (this model automaticaly loads the texture)
                SimpleModel simpleModel = new SimpleModel(factory, "model/tank");
                //simpleModel.SetTexture(factory.CreateTexture2DColor(1,1,Color.Red), TextureType.DIFFUSE);
                ///Physic info (position, rotation and scale are set here)
                GhostObject pi = new GhostObject(new Vector3(200, 50, 0), Matrix.Identity, new Vector3(0.5f));
                ///Shader info (must be a deferred type)
                shader = new DeferredShatterCustomShader(false, false, false, false);
                ///Material info (must be a deferred type also)
                DeferredMaterial fmaterial = new DeferredMaterial(shader);
                ///The object itself
                IObject obj = new IObject(fmaterial, simpleModel, pi);
                ///Add to the world
                this.World.AddObject(obj);
            }

            {
                ///Create a simple object
                ///Geomtric Info and textures (this model automaticaly loads the texture)
                SimpleModel simpleModel = new SimpleModel(factory, "model/cenario");
                ///Physic info (position, rotation and scale are set here)
                TriangleMeshObject tmesh = new TriangleMeshObject(simpleModel, Vector3.Zero, Matrix.Identity, Vector3.One, MaterialDescription.DefaultBepuMaterial());
                ///Shader info (must be a deferred type)
                DeferredNormalShader shader = new DeferredNormalShader();
                ///Material info (must be a deferred type also)
                DeferredMaterial fmaterial = new DeferredMaterial(shader);
                ///The object itself
                IObject obj = new IObject(fmaterial, simpleModel, tmesh);
                ///Add to the world
                this.World.AddObject(obj);
            }

            ///Add some directional lights to completely iluminate the world
            #region Lights
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

            ///Add a AA post effect
            this.RenderTechnic.AddPostEffect(new AntiAliasingPostEffect());

            ///add a camera
            this.World.CameraManager.AddCamera(new CameraFirstPerson(GraphicInfo.Viewport));
        }

        const float duration = 2;

        protected override void Update(GameTime gameTime)
        {
            // Handle input
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds / 100;
            KeyboardState keyboardState = Keyboard.GetState();           


            // Pressing the Up arrow or the A button on controller will Shatter the 
            // model
            if (keyboardState.IsKeyDown(Keys.P) )
                
            {
                shader.Time = Math.Min(duration, shader.Time + elapsedTime);
            }


            // Pressing the Down arrow or the B button on controller will reverse the 
            // Shatter effect 
            if (keyboardState.IsKeyDown(Keys.O))
            {
                shader.Time  = Math.Max(0.0f, shader.Time - elapsedTime);
            }


            base.Update(gameTime);
        }



        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="render"></param>
        protected override void Draw(GameTime gameTime, RenderHelper render)
        {
            ///must be called before
            base.Draw(gameTime, render);

            ///Draw some text to the screen
            render.RenderTextComplete("Demo: Basic Screen Deferred", new Vector2(GraphicInfo.Viewport.Width - 315, 15), Color.White, Matrix.Identity);
        }
    }
}


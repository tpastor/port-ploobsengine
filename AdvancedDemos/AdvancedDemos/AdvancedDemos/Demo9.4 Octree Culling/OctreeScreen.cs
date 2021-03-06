﻿using Microsoft.Xna.Framework;
using PloobsEngine.Cameras;
using PloobsEngine.Light;
using PloobsEngine.Physics;
using PloobsEngine.SceneControl;
using PloobsEngine.Features.DebugDraw;
using PloobsEngine.Modelo;
using PloobsEngine.Physics.Bepu;
using PloobsEngine.Material;
using PloobsEngine.Commands;

namespace AdvancedDemo4._0
{
    /// <summary>
    /// Basic Deferred Scene
    /// </summary>
    [PloobsEngine.TestSuite.TesteVisualScreen]
    public class OctreeScreen : IScene
    {
        DebugShapesDrawer ddrawer = new DebugShapesDrawer();
        LightThrowBepu lt;

        /// <summary>
        /// Sets the world and render technich.
        /// </summary>
        /// <param name="renderTech">The render tech.</param>
        /// <param name="world">The world.</param>
        protected override void SetWorldAndRenderTechnich(out IRenderTechnic renderTech, out IWorld world)
        {
            ///create the world using bepu as physic api and a simple culler implementation
            ///IT DOES NOT USE PARTICLE SYSTEMS (see the complete constructor, see the ParticleDemo to know how to add particle support)
            world = new IWorld(new BepuPhysicWorld(-9.7f,true), new OctreeCuller(2000,1,5,new Vector3(0,-10,0),ddrawer));            

            ///Create the deferred description
            DeferredRenderTechnicInitDescription desc = DeferredRenderTechnicInitDescription.Default();
            ///Some custom parameter, this one allow light saturation. (and also is a pre requisite to use hdr)
            desc.UseFloatingBufferForLightMap = true;
            ///set background color, default is black
            desc.BackGroundColor = Color.CornflowerBlue;
            ///create the deferred technich
            renderTech = new DeferredRenderTechnic(desc);
        }

        protected override void InitScreen(PloobsEngine.Engine.GraphicInfo GraphicInfo, PloobsEngine.Engine.EngineStuff engine)
        {
            base.InitScreen(GraphicInfo, engine);

            engine.AddComponent(new DebugDraw());

        }

        protected override void CleanUp(PloobsEngine.Engine.EngineStuff engine)
        {
            lt.CleanUp();
            base.CleanUp(engine);
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

            RegisterDebugDrawCommand rc = new RegisterDebugDrawCommand(ddrawer);
            CommandProcessor.getCommandProcessor().SendCommandAssyncronous(rc);
            
            ///Create a simple object
            ///Geomtric Info and textures (this model automaticaly loads the texture)
            SimpleModel simpleModel = new SimpleModel(factory, "Model//cenario");
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

            lt = new LightThrowBepu(this.World, factory);

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
            this.World.CameraManager.AddCamera(new CameraFirstPerson(GraphicInfo));
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

            render.RenderTextComplete("Culling System Octree", new Vector2(20, 15), Color.White, Matrix.Identity);
            render.RenderTextComplete("Thow a ball and see in what Nodes (Boxes) it is in", new Vector2(20, 35), Color.White, Matrix.Identity);
            render.RenderTextComplete("Objects Rendered: " + this.World.Culler.RenderedObjectThisFrame, new Vector2(20, 55), Color.White, Matrix.Identity);
            
        }
    }
}


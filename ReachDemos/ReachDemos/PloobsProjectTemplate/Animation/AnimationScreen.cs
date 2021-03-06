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
using PloobsEngine.Modelo.Animation;

namespace ProjectTemplate
{
    public class AnimationScreen : IScene
    {
        protected override void SetWorldAndRenderTechnich(out IRenderTechnic renderTech, out IWorld world)
        {
            BepuPhysicWorld BepuPhysicWorld = new BepuPhysicWorld(-0.97f);
            SimpleCuller SimpleCuller = new SimpleCuller();
            world = new IWorld(BepuPhysicWorld, SimpleCuller);

            ForwardRenderTecnichDescription desc = ForwardRenderTecnichDescription.Default();
            desc.BackGroundColor = Color.CornflowerBlue;
            renderTech = new ForwardRenderTecnich(desc);
        }

        protected override void LoadContent(GraphicInfo GraphicInfo, GraphicFactory factory ,IContentManager contentManager)
        {
            base.LoadContent(GraphicInfo, factory, contentManager);

            {
                ///carrega o Modelo
                AnimatedModel am = new AnimatedModel(factory, "..\\Content\\Model\\PlayerMarine", "..\\Content\\Textures\\PlayerMarineDiffuse");
                ///Inicializa o Controlador (Idle eh o nome da animacao inicial)
                AnimatedController arobo = new AnimatedController(am, "Idle");

                ///Cria o shader e o material animados 
                ForwardSimpleAnimationShader sas = new ForwardSimpleAnimationShader(arobo);                
                ForwardAnimatedMaterial amat = new ForwardAnimatedMaterial(arobo, sas);
                IObject marine = new IObject(amat,am, new GhostObject(Vector3.Zero,Matrix.Identity,Vector3.One * 10));
                
                ///Adiciona no mundo
                this.World.AddObject(marine);

                //sas.EnableTexture = true;
            }

            var newCameraFirstPerson = new CameraFirstPerson(GraphicInfo);        
            this.World.CameraManager.AddCamera(newCameraFirstPerson);
        }

        protected override void Draw(GameTime gameTime, RenderHelper render)
        {        
            base.Draw(gameTime, render);
            render.RenderTextComplete("PloobsEngine 3D Bone Animation Demo on Reach Profile", new Vector2(20, 10), Color.Red, Matrix.Identity);
        }

    }
}

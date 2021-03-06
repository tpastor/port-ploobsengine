﻿using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PloobsEngine;
using PloobsEngine.Cameras;
using PloobsEngine.Entity;
using PloobsEngine.Input;
using PloobsEngine.Light;
using PloobsEngine.Material;
using PloobsEngine.MessageSystem;
using PloobsEngine.Modelo;
using PloobsEngine.Physics;
using PloobsEngine.Physics.Bepu;
using PloobsEngine.SceneControl;
using PloobsEngine.Utils;
using PloobsEngine.Engine;

namespace IntroductionDemo4._0
{    
    /// <summary>
    /// Objects Manipulation Screen
    /// Exemplos Mensagem e deteccao de colisao
    /// Envio de Mensagens, Deteccao e Resposta a colisao
    /// </summary>
    public class ChangingMessagesScreen : IScene
    {                
        ICamera cam;
        LightThrowBepu lt;

        protected override void SetWorldAndRenderTechnich(out IRenderTechnic renderTech, out IWorld world)
        {
            world = new IWorld(new BepuPhysicWorld(-9.8f, true), new SimpleCuller(), null, true);

            DeferredRenderTechnicInitDescription desc = DeferredRenderTechnicInitDescription.Default();
            desc.UseFloatingBufferForLightMap = true;
            desc.BackGroundColor = Color.CornflowerBlue;
            renderTech = new DeferredRenderTechnic(desc);
        }

        protected override void InitScreen(PloobsEngine.Engine.GraphicInfo GraphicInfo, PloobsEngine.Engine.EngineStuff engine)
        {
            base.InitScreen(GraphicInfo, engine);

            MessageDeliver md = new MessageDeliver();
            engine.AddComponent(md);
        }


        protected override void LoadContent(PloobsEngine.Engine.GraphicInfo GraphicInfo, PloobsEngine.Engine.GraphicFactory factory, IContentManager contentManager)
        {
            base.LoadContent(GraphicInfo, factory, contentManager);                        
        
            #region Models
            ///Paralelepipelo (cubo com scale ) enviara mensagens quando colidir com objetos
            ///Object that will send messages when something collides with it
            {
                SimpleModel sm = new SimpleModel(factory, "..\\Content\\Model\\cubo");
                sm.SetTexture(factory.CreateTexture2DColor(1,1, Color.White), TextureType.DIFFUSE);                
                BoxObject pi = new BoxObject(new Vector3(100, 40, 0), 1,1,1, 25,new Vector3(100, 10, 100),Matrix.Identity,MaterialDescription.DefaultBepuMaterial());                
                ///Adiciona um handler que sera chamada quando uma colisao acontecer
                ///Add a handler that will handle the colision =P
                pi.Entity.CollisionInformation.Events.InitialCollisionDetected += new BEPUphysics.Collidables.Events.InitialCollisionDetectedEventHandler<BEPUphysics.Collidables.MobileCollidables.EntityCollidable>(Events_InitialCollisionDetected);
                DeferredNormalShader shader = new DeferredNormalShader();                                
                IMaterial mat = new DeferredMaterial(shader);
                IObject obj3 = new IObject(mat, sm, pi);                
                this.World.AddObject(obj3);
            }

            ////CUBO Q VAI MUDAR DE COR
            ///Object that will recieve the message (collision message send from the above object) and will change color.
            {
                SimpleModel sm = new SimpleModel(factory,"..\\Content\\Model\\cubo");
                sm.SetTexture(factory.CreateTexture2DColor(1,1, Color.Yellow), TextureType.DIFFUSE);                
                BoxObject pi = new BoxObject(new Vector3(50, 50, 50), 1,1,1, 5, new Vector3(15),Matrix.Identity,MaterialDescription.DefaultBepuMaterial());
                pi.isMotionLess = true;
                DeferredNormalShader shader = new DeferredNormalShader();
                IMaterial mat = new DeferredMaterial(shader);
                IObject obj3 = new IObject(mat, sm, pi);                
                ///Adiciona um handler para tratar das mensagens (existe outra maneira mais robusta de fazer isto, conforme citado no exemplo sobre Triggers)
                ///hanlde incomming messages
                obj3.OnRecieveMessage += new OnRecieveMessage(obj3_OnRecieveMessage);
                ///Forcando um ID, normalmente ele eh setado automaticamente ao adicionar o objeto no mundo
                ///Set the Id, forcing it  (you can only force ids less than 1000)
                obj3.SetId(77);
                this.World.AddObject(obj3);
                long id = obj3.GetId();
                ///Testa se o Id atribuido eh o forcado 
                ///Internamente a Engine atribui Ids acima de 1000 (valores abaixo funcionarao, a menos que alguem ja tenha forcado este Id antes)                
                ///Como neste caso nao forcamos o id de ninguem para 77, entao o obj3 tera id 77
                ///Soh pra garantir ;)
                ///Just check
                Debug.Assert(id == 77);
             
            }

            ////cubo que escuta um canal de mensagens
            ///Cube that listen a channel
            {
                SimpleModel sm = new SimpleModel(factory, "..\\Content\\Model\\cubo");
                sm.SetTexture(factory.CreateTexture2DColor(1,1,Color.Red), TextureType.DIFFUSE);                
                BoxObject pi = new BoxObject(new Vector3(100, 50, 50), 1,1,1, 50, new Vector3(15),Matrix.Identity,MaterialDescription.DefaultBepuMaterial());
                pi.isMotionLess = true;
                DeferredNormalShader shader = new DeferredNormalShader();
                IMaterial mat = new DeferredMaterial(shader);
                IObject obj3 = new IObject(mat, sm, pi);                
                ///Adiciona um handler para tratar das mensagens (existe outra maneira mais robusta de fazer isto, conforme citado no exemplo sobre Triggers)
                ///OBSERVAR QUE FOI USADO O MESMO HANDLER QUE O OBJETO ANTERIOR (JA QUE DESEJA-SE TER O MESMO EFEITO)
                ///hanle messages
                obj3.OnRecieveMessage += new OnRecieveMessage(obj3_OnRecieveMessage);                
                this.World.AddObject(obj3);                

                ///Adiciona este objeto ao canal de comunicao chamado "cubo" (recebera mensagens deste grupo tb)
                ///REGISTER IN THIS MESSAGE CHANNEL
                EntityMapper.getInstance().AddgrouptagRecieveEntity("cubo", obj3);
            }


            {
                SimpleModel sm = new SimpleModel(factory,"..\\Content\\Model\\cenario");
                IPhysicObject pi = new TriangleMeshObject(sm, Vector3.Zero,Matrix.Identity,Vector3.One,MaterialDescription.DefaultBepuMaterial());
                DeferredNormalShader shader = new DeferredNormalShader();
                IMaterial mat = new DeferredMaterial(shader);
                IObject obj3 = new IObject(mat, sm, pi);                
                this.World.AddObject(obj3);
            }            
            

            #endregion            

            cam = new CameraFirstPerson(GraphicInfo);            
            cam.FarPlane = 3000;

            lt = new LightThrowBepu(this.World, factory);

            #region NormalLight
            ///Conjunto de luzes direcionais
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
            
            this.World.CameraManager.AddCamera(cam);
            
        }

        /// <summary>
        /// Collision happened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="other"></param>
        /// <param name="pair"></param>
        void Events_InitialCollisionDetected(BEPUphysics.Collidables.MobileCollidables.EntityCollidable sender, BEPUphysics.Collidables.Collidable other, BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            IObject send = BepuEntityObject.RecoverObjectFromEntity(sender.Entity);
            IObject obj = BepuEntityObject.RecoverObjectFromCollidable(other); 

            ///Verifica se esta bola ja foi considerada
            ///Consider just the first collision
            if (alreadProcessed.Contains(obj.GetId()))
                return;

            alreadProcessed.Add(obj.GetId());

            ///se o objeto colidido for diferente do cenario 
            ///dont consider the island model (triangle meshes in general)
            if (obj.PhysicObject.PhysicObjectTypes != PhysicObjectTypes.TRIANGLEMESHOBJECT)
            {
                shouldDraw = true;

                ///Envia uma mensagem para o canal de comunicacao CUBO
                ///Send a message to the channel
                Message m = new Message(send.GetId(), PrincipalConstants.InvalidId, "cubo", Priority.MEDIUM, -1, SenderType.OBJECT, null, "CHANGECOLOR");
                MessageDeliver.SendMessage(m);

                ///Esta mensagem foi enviada sem Sender (Quem receber a mensagem nao sabera quem enviou)
                ///Envia uma mensagem para o "CUBO QUE VAI MUDAR DE COR" (lembre que o id dele eh 77 !!)
                ///Send a message to the specific  id (first cube)
                m = new Message(PrincipalConstants.InvalidId, 77, null, Priority.MEDIUM, -1, SenderType.OBJECT, null, "CHANGECOLOR");
                MessageDeliver.SendMessage(m);
            }

            objNameTemp = obj.Name;
        }

        protected override void Draw(GameTime gameTime, RenderHelper render)
        {
            base.Draw(gameTime, render);

            Texture2D logo = GraphicFactory.GetTexture2D("Textures\\engine_logo");
            int wd = 64;
            int hg = 48;
            render.RenderTextureComplete(logo, new Rectangle(this.GraphicInfo.BackBufferWidth - wd, this.GraphicInfo.BackBufferHeight - hg, wd, hg));

            render.RenderTextComplete("Demo: Changing Messages", new Vector2(GraphicInfo.Viewport.Width - 515, 15), Color.White,Matrix.Identity);
            render.RenderTextComplete("Launch balls (Left Mouse Buttom) at the grey platform", new Vector2(GraphicInfo.Viewport.Width - 515, 40), Color.White, Matrix.Identity);

            if (shouldDraw)
            {
                render.RenderTextComplete("Collision With Object: " + objNameTemp, new Vector2(20, 60), Color.White, Matrix.Identity); 
            }
            
        }

        /// <summary>
        /// Handler que trata das mensagens recebidas pelos dois cubos
        /// handle Message
        /// </summary>
        /// <param name="Reciever"></param>
        /// <param name="mes"></param>
        void obj3_OnRecieveMessage(IObject Reciever, Message mes)
        {
            if (mes.Cod == "CHANGECOLOR")
            {
                ///Muda a Textura do objeto
                Texture2D tex = GraphicFactory.CreateTexture2DColor(1,1, StaticRandom.RandomColor());
                (Reciever.Modelo as SimpleModel).SetTexture(tex, TextureType.DIFFUSE);

                ///Recuperar quem enviou a mensagem 
                long senderId = mes.Sender;
                if (senderId != PrincipalConstants.InvalidId)
                {
                    if (mes.SenderType == SenderType.OBJECT)
                    {
                        IObject sender = (EntityMapper.getInstance().getEntity(senderId) as IObject);
                    }

                }
            }
        }


        /// <summary>
        /// O movimento da bola nao eh completamente continuo em cima do paralelepipedo, 
        /// entao a bola fica subindo e descendo e a cada movimento deste uma nova colisao eh criada
        /// Para evitar isto, apenas a primeira iteracao das bolas com a superficies sera considerada
        /// </summary>
        List<long> alreadProcessed = new List<long>();

        bool shouldDraw;
        string objNameTemp;
         
        
        protected override void CleanUp(EngineStuff engine)
        {
            base.CleanUp(engine);

            ///EntityMapper is a global system, used by all demos 
            ///Releasing this time because of the specific id 77 used
            EntityMapper.getInstance().ClearAllEntries();

            engine.RemoveComponent("MessageDeliver");

            lt.CleanUp();
        }
    }
}


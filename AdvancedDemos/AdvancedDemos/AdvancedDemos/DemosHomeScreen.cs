﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PloobsEngine.Commands;
using PloobsEngine.Engine;
using PloobsEngine.Input;
using PloobsEngine.SceneControl;
using PloobsEngine.SceneControl.GUI;
using TomShane.Neoforce.Controls;
using System.Collections.Generic;

namespace AdvancedDemo4._0
{
    public class DemosHomeScreen : IScreen
    {
        public DemosHomeScreen() : base(new NeoforceGui()) {

            Height = 600;
            Width = 800;
            mipmap = anisotropic = fullscreen = multisampling = verticalsync = false;        
        }

        int index = 0;

        ComboBox lb1;
        int Height, Width;
        bool fullscreen,multisampling, verticalsync,anisotropic,mipmap;


        EngineStuff engine;

        private int[] screenList = new int[40];

        private IScreen GetScreen(int screenNumber)
        {
            switch (screenNumber)
            {
                case 0:
                    return new DeferredLoadScreen();         
                case 1:
                    return new BumpSpecularDemo();
                case 2:
                    return new EnvMapScreen();
                case 3:
                    return new ParalaxScreen();
                case 4:
                    return new TransparentDeferredScreen();
                case 5:
                    return new SoundScreen();
                case 6:
                    return new FollowerObjectSoundScreen();
                case 7:
                    return new TerrainScreen();
                case 8:
                    return new ParticleScreen();
                case 9:
                    return new AnimatedBilboardScreen();
                case 10:
                    return new InstancedBilboardScreen();
                case 11:
                    return new NormalBilboardScreen();
                case 12:
                    return new ProceduralAnimatedBilboardScreen();
                case 13:
                    return new DeferredAnimatedScreen();
                case 14:
                    return new DGUIScreen();
                case 15:
                    return new FGUIScreen();
                case 16:
                    return new NoiseScreen();
                case 17:
                    return new OceanScreen();
                case 18:
                    return new WaterCompleteScreen();
                case 19:
                    return new DeferredDirectionaldShadowScreen();                
                case 20:
                    return new SSAOScreen();
                case 21:
                    return new OctreeScreen();
                case 22:
                    return new CPUBillboardScreen();
                case 23:
                    return new DecalScreen();
                case 24:
                    return new ExplosionScreen();
                case 25:
                    return new StealthEffectScreen();
                case 26:
                    return new AmbientEnvironmenpScreen();
                case 27:
                    return new ShaderIDScreen();
                case 28:          
                    return new Physx28Screen();
                case 29:       
                    return new Physx28MaterialScreen();
                case 30:       
                    return new Physx28TriggerScreen();
                case 31:       
                    return new PhysxCharacter28Screen();
                case 32:       
                    return new BasicCloth28Screen();
                case 33:       
                    return new FlagCloth28Screen();
                case 34:       
                    return new PressureCloth28Screen();
                case 35:
                    return new Fluids28Screen();
                case 36:
                    return new DeferredEmitterFluids28Screen();
                case 37:
                    return new AnimScreen();
                    case 38:
                return new XnaSkinnedScreen();
                    case 39:
                return new BoxCloth28Screen();
                    
                    
                default:
                    break;
            }
            return null;                       

        }

        IScreen active = null;

        protected override void InitScreen(PloobsEngine.Engine.GraphicInfo GraphicInfo, PloobsEngine.Engine.EngineStuff engine)
        {
            base.InitScreen(GraphicInfo, engine);

            this.engine = engine;

            InputAdvanced inp = new InputAdvanced();
            engine.AddComponent(inp);

            engine.IsMouseVisible = true;

        }

        protected override void  LoadContent(PloobsEngine.Engine.GraphicInfo GraphicInfo, PloobsEngine.Engine.GraphicFactory factory, IContentManager contentManager)
        {
 	        base.LoadContent(GraphicInfo, factory, contentManager);
             
            for (int i = 0; i < screenList.Length; i++)
            {
                screenList.SetValue(i, i);
            }

            {
                SimpleConcreteKeyboardInputPlayable ik = new SimpleConcreteKeyboardInputPlayable(StateKey.PRESS, Keys.F1, ChangeDemo);
                BindKeyCommand bk = new BindKeyCommand(ik, BindAction.ADD);
                CommandProcessor.getCommandProcessor().SendCommandAssyncronous(bk);
            }
            {
                SimpleConcreteKeyboardInputPlayable ik = new SimpleConcreteKeyboardInputPlayable(StateKey.PRESS, Keys.Escape, LeaveGame);
                BindKeyCommand bk = new BindKeyCommand(ik, BindAction.ADD);
                CommandProcessor.getCommandProcessor().SendCommandAssyncronous(bk);
            }

            ConfigMenu();

        }

        protected override void Draw(GameTime gameTime, RenderHelper render)
        {            
            render.Clear(Color.Black);

            render.RenderTextComplete("Welcome to the Ploobs Game Engine Advanced Demos", new Vector2(40, 30), Color.White,Matrix.Identity);
            render.RenderTextComplete("The Focus here were in the funcionalities, not in the visual", new Vector2(40, 50), Color.Red, Matrix.Identity);
            render.RenderTextComplete("As you will see, we need Modelers =P Join us =P", new Vector2(40, 70), Color.Red, Matrix.Identity);
            render.RenderTextComplete("(Press F1 to cycle through demos)", new Vector2(40, 95), Color.White, Matrix.Identity);
            render.RenderTextComplete("(Press Escape to exit)", new Vector2(40, 115), Color.White, Matrix.Identity);           

        }


        private void ConfigMenu()
        {            
            
            NeoforceGui guiManager = this.Gui as NeoforceGui;
            System.Diagnostics.Debug.Assert(guiManager != null);

            // Create and setup Window control.
            Window window = new Window(guiManager.Manager);
            window.Init();
            window.Text = "PloobsEngine Config";
            window.Width = 350;
            window.Height = 300;
            window.Center();
            window.Visible = true;


            Label lab1 = new Label(guiManager.Manager);
            lab1.Text = "Resolução";
            lab1.Top = 20;
            lab1.Left = 20;
            lab1.Parent = window;

            List<string> colors = new List<string>();

            foreach (var item in GraphicInfo.GraphicsAdapter.SupportedDisplayModes)
            {
                if (item.Format == Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color)
                {
                    colors.Add(item.Width + "x" + item.Height);
                }
            }

            lb1 = new ComboBox(guiManager.Manager);
            lb1.Init();
            lb1.Parent = window;
            lb1.Left = lab1.Left;
            lb1.Top = lab1.Top + lab1.Height;
            lb1.Width = 200;
            lb1.ItemIndex = 0;
            lb1.Height = 20;

            
            lb1.ItemIndexChanged += new TomShane.Neoforce.Controls.EventHandler(lb1_ItemIndexChanged);
            lb1.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            lb1.Items.AddRange(colors);

            lb1.Text = "800x600";
            lb1.SelectionStart = 0;            



            // Create Button control and set the previous window as its parent.
            Button button = new Button(guiManager.Manager);
            button.Init();
            button.Text = "Apply";
            button.Width = button.Text.Length * 10;
            button.Height = 24;
            button.Left = (window.ClientWidth / 2) - (button.Width / 2);
            button.Top = window.ClientHeight - button.Height - 8;
            button.Anchor = Anchors.Bottom;
            button.Parent = window;
            button.Click += new TomShane.Neoforce.Controls.EventHandler(button_Click);


            CheckBox ck1 = new CheckBox(guiManager.Manager);
            ck1.Text = "FullScreen";
            ck1.Checked = false;
            ck1.Click += new TomShane.Neoforce.Controls.EventHandler(ck1_Click);
            ck1.Top = lb1.Top + lb1.Height + 10;
            ck1.Parent = window;
            ck1.Left = lb1.Left;
            ck1.Width = ck1.Text.Length*10;


            CheckBox ms1 = new CheckBox(guiManager.Manager);
            ms1.Text = "MultiSampling";
            ms1.Checked = false;
            ms1.Top = ck1.Top + ck1.Height + 10;
            ms1.Parent = window;
            ms1.Click += new TomShane.Neoforce.Controls.EventHandler(ms1_Click);
            ms1.Left = lb1.Left;
            ms1.Width = ms1.Text.Length * 10;

            CheckBox vsy = new CheckBox(guiManager.Manager);
            vsy.Text = "Vertical Sincronization";
            vsy.Checked = false;
            vsy.Top = ms1.Top + ms1.Height + 10;
            vsy.Parent = window;
            vsy.Click += new TomShane.Neoforce.Controls.EventHandler(vsy_Click);
            vsy.Left = lb1.Left;
            vsy.Width = vsy.Text.Length * 10;

            CheckBox mip = new CheckBox(guiManager.Manager);
            mip.Text = "Use MipMap";
            mip.Checked = false;
            mip.Top = vsy.Top + vsy.Height + 10;
            mip.Parent = window;
            mip.Click += new TomShane.Neoforce.Controls.EventHandler(mip_Click);
            mip.Left = lb1.Left;
            mip.Width = mip.Text.Length * 10;

            CheckBox ans = new CheckBox(guiManager.Manager);
            ans.Text = "Use Anisotropic Filtering";
            ans.Checked = false;
            ans.Top = mip.Top + mip.Height + 10;
            ans.Parent = window;
            ans.Click += new TomShane.Neoforce.Controls.EventHandler(ans_Click); 
            ans.Left = lb1.Left;
            ans.Width = ans.Text.Length * 10;           


            // Add the window control to the manager processing queue.
            guiManager.Manager.Add(window);
        
        }

        void ans_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            anisotropic = !anisotropic;
        }

        void mip_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            mipmap = !mipmap;
        }

        void lb1_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {

            ComboBox bb = (ComboBox)sender;

            String sel = bb.Text;
            string[] vals = sel.Split('x');

            Width = int.Parse(vals[0]);
            Height = int.Parse(vals[1]);
        }

        void button_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            InitialEngineDescription ini = engine.GetEngineDescription();
            ini.BackBufferWidth = Width;
            ini.BackBufferHeight= Height;
            ini.isFullScreen = fullscreen;
            ini.UseVerticalSyncronization  = verticalsync;
            ini.isMultiSampling = multisampling;
            ini.useMipMapWhenPossible = mipmap;
            if (anisotropic)
                ini.DefaultFiltering = Microsoft.Xna.Framework.Graphics.SamplerState.AnisotropicWrap;
            else
                ini.DefaultFiltering = Microsoft.Xna.Framework.Graphics.SamplerState.LinearWrap;
            engine.ApplyEngineDescription(ref ini);
        }        

        void vsy_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            verticalsync = !verticalsync;
        }

        void ms1_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            multisampling = !multisampling;
        }

        void ck1_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            fullscreen = !fullscreen;
        }


        public void ChangeDemo(InputPlayableKeyBoard ipk)
        {

            ///lazy ways of cycling between demos
            ///just remove everything and load the righ screen

            if (active is LoadingScreen || (active != null && active.IsLoaded == false))
                return;            

            //if(this.ScreenState == PloobsEngine.SceneControl.ScreenState.Active)
            //    this.ScreenState = ScreenState.Hidden;


            foreach (var item in ScreenManager.GetScreens())
            {
                ScreenManager.RemoveScreen(item);
            }
                

            active = GetScreen(screenList[index % screenList.GetLength(0)]);
            ScreenManager.AddScreen(active,new LoadingScreen());
            index++;            
        }

        public void LeaveGame(InputPlayableKeyBoard ipk)
        {
            engine.Exit();
        }
    }
}
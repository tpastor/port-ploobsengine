using System;
using PloobsEngine.Engine;
using PloobsEngine.SceneControl;
using EngineTestes.LoadingScreen;
using ProjectTemplate;
using EngineTestes._2DSamples;
using AdvancedDemo4._0;
using EngineTestes.ScreenTests;
using PloobsEngine.TestSuite;

namespace EngineTestes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //TestProcessor TestProcessor = new TestProcessor();
            //TestProcessor.EvaluateTestes("EngineTestes.exe");
            //return;

            InitialEngineDescription desc = InitialEngineDescription.Default();
            //desc.useMipMapWhenPossible = true;            
            //desc.isMultiSampling = true;

            using (EngineStuff engine = new EngineStuff(ref desc, LoadScreen))
            {
                engine.Run();
            }

        }

        static void LoadScreen(ScreenManager manager)
        {
            manager.AddScreen(new Basic2D());                                 
            //manager.AddScreen(new DynamicEnvMapScreen());                                 
            //manager.AddScreen(new PRTScreen());                                 
            //manager.AddScreen(new DeferredEmitterFluids28Screen3());  
            //manager.AddScreen(new PrePassScreen());

            //manager.AddScreen(new OcclusionScreen());                       
            //manager.AddScreen(new BlurScreen());            
            //manager.AddScreen(new SSAOScreen()); 
            //manager.AddScreen(new BakerScreen());
            //manager.AddScreen(new DefPassScreen());
            //manager.AddScreen(new MixReflection());
            //manager.AddScreen(new ModeloScreen());
            //manager.AddScreen(new RVO3DScreen());
            //manager.AddScreen(new Physx28MaterialScreen());
            //manager.AddScreen(new Physx28VehicleScreen());            
            //manager.AddScreen(new Physx28TriggerScreen());
            //manager.AddScreen(new PhysxTerrain28Screen());
            //manager.AddScreen(new PhysxCharacter28Screen());
            //manager.AddScreen(new DeferredEmitterFluids28Screen());            
            //manager.AddScreen(new DeferredFluids28Screen2());
            //manager.AddScreen(new DeferredFluids28Screen());
            //manager.AddScreen(new Fluids28Screen());
            //manager.AddScreen(new PressureCloth28Screen());
            //manager.AddScreen(new FlagCloth28Screen());
            //manager.AddScreen(new BasicCloth28Screen());
            //manager.AddScreen(new Physx28Screen());
            //manager.AddScreen(new PhysxScreen());
            //manager.AddScreen(new BulletScreen());

            //manager.AddScreen(new RVOScreen());
            //manager.AddScreen(new AmbientEnvironmenpScreen());
            //manager.AddScreen(new ShaderIDScreen());
            //manager.AddScreen(new EnvironmentBumpScreen());
            //manager.AddScreen(new InstancedBilboardScreen());
            //manager.AddScreen(new DeferredDirectionaldShadowScreen());
            //manager.AddScreen(new BehaviorTreeScreen());
            //manager.AddScreen(new DefQuadTerrainScreen());
            //manager.AddScreen(new QuadTerrainScreen());
            //manager.AddScreen(new CPUBillboardScreen());            
            //manager.AddScreen(new DefPassScreen());
            //manager.AddScreen(new DecalScreen());
            //manager.AddScreen(new HBAOScreen());
            //manager.AddScreen(new MotionForwardScreen());
            //manager.AddScreen(new MotionBluScreen());
            //manager.AddScreen(new SerializationScreen());
            //manager.AddScreen(new ActionScriptScreen());
            //manager.AddScreen(new ScriptScreen());
            //manager.AddScreen(new Basic2DNeoforce());
            //manager.AddScreen(new Primitive2D());
            //manager.AddScreen(new Teste2D());
            //manager.AddScreen(new SponzaScreen());
            //manager.AddScreen(new ForwardMaterialsScreen());
            //manager.AddScreen(new PostEffectScreen());            
            //manager.AddScreen(new MobilePhysicScreen());
            //manager.AddScreen(new MultScreen());
            //manager.AddScreen(new FirstScreen());
            //manager.AddScreen(new DeferredScreen());
            //manager.AddScreen(new DeferredLoadScreen());
            //manager.AddScreen(new DeferredAnimatedScreen());
            //manager.AddScreen(new ForwardAnimatedScreen());
            //manager.AddScreen(new CharacterScreen());
            //manager.AddScreen(new ParticleScreen());
            //manager.AddScreen(new PostEffectScreen());
            //manager.AddScreen(new SoundScreen());
            //manager.AddScreen(new TransparentForwardScreen());
            //manager.AddScreen(new BilboardScreen());
            //manager.AddScreen(new TerrainScreen());
            //manager.AddScreen(new TransparentDeferredScreen());
            //manager.AddScreen(new AAScreen());
            //manager.AddScreen(new TechDemoScreen());
            //manager.AddScreen(new TechDemoScreenModel());
            //manager.AddScreen(new GUIScreen());
            //manager.AddScreen(new FGUIScreen());
            //manager.AddScreen(new ForwardLoadScreen());
            //manager.AddScreen(new ForwardMeshesFromModelLoadScreen());
            //manager.AddScreen(new DeferredLoadScreen(),new LoadScene());
            //manager.AddScreen(new DebugDrawScreen());
            //manager.AddScreen(new OctreeScreen());
            //manager.AddScreen(new HybridDeferred());
            //manager.AddScreen(new VegetationForwardScreen());
            //manager.AddScreen(new VegetationDeferredScreen());            
            //manager.AddScreen(new MixReflection());                        
            //manager.AddScreen(new Basic2D());
            //manager.AddScreen(new ConstraintScreen());
            //manager.AddScreen(new IaWaypoints());
            //manager.AddScreen(new IaPathFinding());
            //manager.AddScreen(new SteerScreen());
            //manager.AddScreen(new FSMScreen());
            //manager.AddScreen(new ParalaxScreen());
            //manager.AddScreen(new RadialBluScreen());
            //manager.AddScreen(new BoltSample());
            //manager.AddScreen(new ShatterSample());
            //manager.AddScreen(new ExplosionScreen());
            //manager.AddScreen(new StealthEffectScreen());
            //manager.AddScreen(new StealthShaderScreen());
            //manager.AddScreen(new GammaCorrectionScreen());
        }
    }

}


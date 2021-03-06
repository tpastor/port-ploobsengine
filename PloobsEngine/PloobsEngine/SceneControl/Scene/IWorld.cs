#region License
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using PloobsEngine.Light;
using PloobsEngine.Cameras;
using PloobsEngine.Trigger;
using PloobsEngine.Physics;
using PloobsEngine.Audio;
using System.Runtime.Serialization;
using System.Diagnostics;
using PloobsEngine.Engine.Logger;
using PloobsEngine.Entity;
using PloobsEngine.Engine;
using PloobsEngine.Particles;
using System;
#if WINDOWS
using System.Threading.Tasks;
#endif

namespace PloobsEngine.SceneControl
{
    /// <summary>
    /// Specification of a world
    /// </summary>
#if WINDOWS
    public class IWorld : ISerializable
#else
    public class IWorld 
#endif
    {
#if WINDOWS
        /// <summary>
        /// Initializes a new instance of the <see cref="IWorld"/> class.
        /// </summary>
        /// <param name="PhysicWorld">The physic world.</param>
        /// <param name="Culler">The culler.</param>
        /// <param name="particleManager">The particle manager.</param>
        /// <param name="multiThread">if set to <c>true</c> [mult thread].</param>
        public IWorld(IPhysicWorld PhysicWorld, ICuller Culler, IParticleManager particleManager = null,bool multiThread = false)
#else
        public IWorld(IPhysicWorld PhysicWorld, ICuller Culler, IParticleManager particleManager = null)
#endif
        {
            if (PhysicWorld == null)
            {
                ActiveLogger.LogMessage("Physic World cannot be null", LogLevel.FatalError);
                Debug.Assert(PhysicWorld != null);
                throw new Exception("Physic World cannot be null");
            }
            if (Culler == null)
            {
                ActiveLogger.LogMessage("Culler cannot be null", LogLevel.FatalError);
                Debug.Assert(Culler != null);
                throw new Exception("Culler cannot be null");
            }

            this.particleManager = particleManager;            
            this.PhysicWorld = PhysicWorld;            
            this.CameraManager = new CameraManager();            
            Dummies = new List<IDummy>();
            Lights = new List<ILight>();
            Objects = new List<IObject>();
            Triggers = new List<ITrigger>();
            SoundEmiters3D = new List<ISoundEmitter3D>();
            this.Culler = Culler;
            this.culler.world = this;
            CleanUpObjectsOnDispose = true;
#if WINDOWS
            this.multThreading = multiThread;
#endif
        }
#if WINDOWS
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IWorld"/> class.
        /// Desserialization
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        internal IWorld(SerializationInfo info, StreamingContext context)
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="IWorld"/> class.
        /// TO let the IWorld implementations free from the formal constructor
        /// </summary>
        protected IWorld()
        {
        }

        protected ICuller culler;
        protected GraphicInfo graphicsInfo;
        protected GraphicFactory graphicFactory;
        protected IContentManager contentManager;
        protected IParticleManager particleManager;

        /// <summary>
        /// Gets the particle manager.
        /// </summary>
        public IParticleManager ParticleManager
        {
            get {                
                return particleManager;
            }            
        }

        public bool CleanUpObjectsOnDispose
        {
            set;
            get;
        }

        protected virtual void InitWorld()
        {
            if (particleManager != null)
            {
                particleManager.GraphicInfo = graphicsInfo;
                particleManager.GraphicFactory = graphicFactory;
            }
        }
        internal void iInitWorld()
        {
            InitWorld();
        }

        public IContentManager ContentManager
        {
            get
            {
                return contentManager;
            }
            internal set
            {
                contentManager = value;
            }
        }

        /// <summary>
        /// Gets or sets the graphics info.
        /// </summary>
        /// <value>
        /// The graphics info.
        /// </value>
        internal GraphicInfo GraphicsInfo
        {
            get { return graphicsInfo; }
            set { graphicsInfo = value; }
        }

        /// <summary>
        /// Gets or sets the graphics factory.
        /// </summary>
        /// <value>
        /// The graphics factory.
        /// </value>
        internal GraphicFactory GraphicsFactory
        {
            get { return graphicFactory; }
            set { graphicFactory = value; }
        }



        /// <summary>
        /// Adds an object to the world.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="InitMaterial">if set to <c>true</c> [init material].</param>
        public virtual void AddObject(IObject obj, bool InitMaterial = true)
        {
            if (obj == null)
            {
                ActiveLogger.LogMessage("Cant add null obj", LogLevel.RecoverableError);
                return ;
            }

            EntityMapper.getInstance().AddEntity(obj);
            PhysicWorld.AddObject(obj.PhysicObject);
            obj.PhysicObject.ObjectOwner = obj;
            Objects.Add(obj);
            if (InitMaterial)
                obj.Material.Initialization(graphicsInfo, graphicFactory, obj);
            Culler.onObjectAdded(obj);
            obj.FireOnBeingAdd(this);

            obj.afterAddedToTheWorld();
        }

        /// <summary>
        /// Contains the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public virtual bool ContainsObject(IObject obj)
        {
            if (obj == null)
            {
                ActiveLogger.LogMessage("Cant compare with null obj", LogLevel.RecoverableError);
                return false;
            }
            return Objects.Contains(obj);
        }

        /// <summary>
        /// Removes an object from the world.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public virtual void RemoveObject(IObject obj)
        {
            if (obj == null)
            {
                ActiveLogger.LogMessage("Cant remove with null obj", LogLevel.RecoverableError);
                return;
            }
            obj.RemoveThisObject();
            EntityMapper.getInstance().RemoveEntity(obj);
            PhysicWorld.RemoveObject(obj.PhysicObject);            
            bool resp = Objects.Remove(obj);
            if (!resp)
            {
                ActiveLogger.LogMessage("Cant remove (not found) obj: " + obj.Name, LogLevel.RecoverableError);
            }
            Culler.onObjectRemoved(obj);

        }

#if WINDOWS
        /// <summary>
        /// Mulyi threading enabled ?
        /// </summary>
        protected bool multThreading = true;
        TaskFactory factory = new TaskFactory(TaskScheduler.Default);        
        List<Task> tasks = new List<Task>();
#endif
        /// <summary>
        /// Updates the world.
        /// </summary>
        /// <param name="gt">The gt.</param>
        protected virtual void UpdateWorld(GameTime gt)
        {
#if WINDOWS
            if (!multThreading)
            {
#endif
                PhysicWorld.iUpdate(gt);

                Debug.Assert(CameraManager.ActiveCamera != null);
                CameraManager.ActiveCamera.iUpdate(gt);

                IObject[] toPass = Objects.ToArray();
                for (int i = 0; i < toPass.Count(); i++)
                {
                    toPass[i].iUpdateObject(gt, CameraManager.ActiveCamera, this);
                }

                if (ParticleManager != null)
                    ParticleManager.iUpdate3D(gt, CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection, CameraManager.ActiveCamera.Position);

                foreach (ISoundEmitter3D item in SoundEmiters3D)
                {
                    item.iUpdate(gt, CameraManager.ActiveCamera);
                }
#if WINDOWS
            }
            else
            {
                Debug.Assert(CameraManager.ActiveCamera != null);
                CameraManager.ActiveCamera.iUpdate(gt);

                tasks.Clear();
                float simultaneous = Objects.Count <= Environment.ProcessorCount * 2 ? Objects.Count : Environment.ProcessorCount * 2;
                int perThread = (int)Math.Ceiling(((float)Objects.Count) / simultaneous);
                IObject[] toPass = Objects.ToArray();

                int num = Objects.Count - 1;
                ILight[] lights = Lights.ToArray();

                for (int j = 0; j < simultaneous; j++)
                {
                    int initial = num;
                    tasks.Add(factory.StartNew(
                        () =>
                        {
                            for (int i = initial; i > initial - perThread && i >= 0; i--)
                            {
                                toPass[i].iUpdateObject(gt, CameraManager.ActiveCamera,this );
                            }
                        }
                    ));
                    num -= perThread;
                }
                
                PhysicWorld.iUpdate(gt);

                if (ParticleManager != null)
                    ParticleManager.iUpdate3D(gt, CameraManager.ActiveCamera.View, CameraManager.ActiveCamera.Projection, CameraManager.ActiveCamera.Position);
                
                if (multThreading)
                {
                    Task.WaitAll(tasks.ToArray());                 
                }

                foreach (ISoundEmitter3D item in SoundEmiters3D)
                {
                    item.iUpdate(gt, CameraManager.ActiveCamera);
                }
            }
#endif
        }


        internal void iUpdateWorld(GameTime gt)
        {
            UpdateWorld(gt);
        }
        
        /// <summary>
        /// Adds the light.
        /// </summary>
        /// <param name="light">The light.</param>
        public virtual void AddLight(ILight light)
        {
            if (light == null)
            {
                ActiveLogger.LogMessage("Cant Add null Light", LogLevel.RecoverableError);
                return;
            }
            Lights.Add(light);
        }
        /// <summary>
        /// Removes the light.
        /// </summary>
        /// <param name="light">The light.</param>
        public virtual void RemoveLight(ILight light)
        {
            if (light == null)
            {
                ActiveLogger.LogMessage("Cant remove null Light", LogLevel.RecoverableError);
                return;
            }
            bool resp = Lights.Remove(light);
            if (!resp)
            {
                ActiveLogger.LogMessage("light not found: " + light.Name, LogLevel.Warning);
            }
        }
        /// <summary>
        /// Camera Managment
        /// </summary>
        public CameraManager CameraManager { get; internal set; }

        public IList<ITrigger> Triggers
        {
            internal set;
            get;
        }

        /// <summary>
        /// Adds the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public virtual void AddTrigger(ITrigger trigger)
        {
            if (trigger == null)
            {
                ActiveLogger.LogMessage("Cant Add null Trigger", LogLevel.RecoverableError);
                return;
            }

            if (String.IsNullOrEmpty(trigger.Name))
            {
                ActiveLogger.LogMessage("Trigger with no Name", LogLevel.Warning);
            }

            Triggers.Add(trigger);            

            if (trigger.GhostObject != null)
                PhysicWorld.AddObject(trigger.GhostObject);

        }
        /// <summary>
        /// Removes the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public virtual void RemoveTrigger(ITrigger trigger)
        {
            if (trigger == null)
            {
                ActiveLogger.LogMessage("Cant Remove Null Trigger", LogLevel.RecoverableError);
                return;                
            }
            bool resp = Triggers.Remove(trigger);
            if (!resp)
            {
                ActiveLogger.LogMessage("Trigger not found: " + trigger.Name, LogLevel.Warning);
            }

            if (trigger.GhostObject != null)
                PhysicWorld.RemoveObject(trigger.GhostObject);
        }
        /// <summary>
        /// Gets or sets the physic world.
        /// </summary>
        /// <value>
        /// The physic world.
        /// </value>
        public IPhysicWorld PhysicWorld { get; internal set; }        

        /// <summary>
        /// Add a Dummy to the world
        /// Its like a position, 
        /// usefull to serializable position from a world editor
        /// </summary>
        /// <param name="dummy">The dummy.</param>
        public virtual void AddDummy(IDummy dummy)
        {
            if (dummy == null)
            {
                ActiveLogger.LogMessage("Cant Add Null dummy", LogLevel.RecoverableError);
                return;
            }

            if (String.IsNullOrEmpty(dummy.Name))
            {
                ActiveLogger.LogMessage("Dummy with no Name", LogLevel.Warning);
            }
            Dummies.Add(dummy);
        }
        /// <summary>
        /// Removes the dummy.
        /// </summary>
        /// <param name="dummy">The dummy.</param>
        public virtual void RemoveDummy(IDummy dummy)
        {
            if (dummy == null)
            {
                ActiveLogger.LogMessage("Cant Remove Null dummy", LogLevel.RecoverableError);
                return;                
            }
            bool resp = Dummies.Remove(dummy);
            if (!resp)
            {
                ActiveLogger.LogMessage("Dummy not found: " + dummy.Name, LogLevel.Warning);
            }
        }
        /// <summary>
        /// Gets all the dummyes.
        /// </summary>
        /// <returns></returns>
        public IList<IDummy> Dummies
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the lights.
        /// </summary>
        public IList<ILight> Lights
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the culler.
        /// </summary>
        /// <value>
        /// The culler.
        /// </value>
        public ICuller Culler
        {
            set
            {
                if (value == null)
                {
                    ActiveLogger.LogMessage("Cant add null culler", LogLevel.FatalError);
                    Debug.Assert(value != null);
                    throw new Exception("Cant add null culler");
                }

                this.culler = value;
                foreach (var item in Objects)
                {
                    this.culler.onObjectAdded(item);
                }
            }
            get
            {
                return culler;
            }
        }        
        
        /// <summary>
        /// Gets the objects.
        /// </summary>
        public IList<IObject> Objects
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the objects.
        /// </summary>
        public List<ISoundEmitter3D> SoundEmiters3D
        {
            get;
            protected set;
        }


        /// <summary>
        /// Adds the sound emitter.
        /// </summary>
        /// <param name="em">The em.</param>
        /// <param name="play">if set to <c>true</c> [play].</param>
        public virtual void AddSoundEmitter(ISoundEmitter3D em, bool play = false)
        {
            if (em == null)
            {
                ActiveLogger.LogMessage("Emitter is Null " + em.ToString(), LogLevel.RecoverableError);
                return;
            }
            
            SoundEmiters3D.Add(em);
            em.Apply3D();
            if(play)
                em.Play();
        }

        /// <summary>
        /// Removes the sound emitter.
        /// </summary>
        /// <param name="em">The em.</param>
        public virtual void RemoveSoundEmitter(ISoundEmitter3D em)
        {
            if (em == null)
            {
                ActiveLogger.LogMessage("Emitter is Null " + em.ToString(), LogLevel.RecoverableError);
                return;
            }
            em.Stop();
            bool resp = SoundEmiters3D.Remove(em);
            if (!resp)
            {
                ActiveLogger.LogMessage("Emitter not found: " + em.ToString(), LogLevel.Warning);
            }

        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        public virtual void CleanUp()
        {
            foreach (var item in this.CameraManager.GetCamerasDescription())
            {
                item.cam.CleanUp();
            }
            
            foreach (var item in SoundEmiters3D.ToArray())
            {
                this.RemoveSoundEmitter(item);
            }

            foreach (var item in Triggers.ToArray())
            {
                this.RemoveTrigger(item);
            }

            foreach (var item in Lights.ToArray())
            {
                this.RemoveLight(item);
            }


            if (CleanUpObjectsOnDispose)
            {
                foreach (var item in Objects.ToArray())
                {
                    this.RemoveObject(item);
                    item.CleanUp(graphicFactory);

                }
            }
            else
            {
                foreach (var item in Objects.ToArray())
                {
                    this.RemoveObject(item);
                }
            }

            

            foreach (var item in SoundEmiters3D)
            {
                item.CleanUp(graphicFactory);
            }

            Objects.Clear();
            Lights.Clear();            
            Dummies.Clear();
            this.culler = null;
            SoundEmiters3D.Clear();
            CameraManager = null;
            Triggers.Clear();
            if (particleManager != null)
            {
                particleManager.iCleanUp();
                particleManager = null;
            }
            this.PhysicWorld.iCleanUp();
            this.PhysicWorld = null;
        }

        #region ISerializable Members
#if WINDOWS
        /// <summary>
        /// TODO
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ActiveLogger.LogMessage("Serialization not implemented", LogLevel.RecoverableError);
            //info.AddValue("Objects", Objects, Objects.GetType());
            //info.AddValue("Lights", Lights, Lights.GetType());
            //info.AddValue("Triggers", Triggers, Triggers.GetType());
            //info.AddValue("SoundEmitter3D", SoundEmiters3D, SoundEmiters3D.GetType());
            //info.AddValue("Culler", Culler, Culler.GetType());
            //info.AddValue("PhysicWorld", PhysicWorld, PhysicWorld.GetType());
        }        

#endif
        #endregion
    }
}

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
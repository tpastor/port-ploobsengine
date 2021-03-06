﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PloobsEngine.SceneControl;
using PloobsEngine.Modelo;
using PloobsEngine.Physics;
using PloobsEngine.Material;
using DPSF;

namespace AdvancedDemo4._0
{
    class ThrowParticlesObject : IObject
    {
        DefaultSprite3DBillboardParticleSystem particleSystem;
        public ThrowParticlesObject(DefaultSprite3DBillboardParticleSystem particleSystem, IMaterial material, IModelo modelo, IPhysicObject physicObject)
            : base(material, modelo, physicObject)
        {
            this.particleSystem = particleSystem;
        }

        protected override void UpdateObject(Microsoft.Xna.Framework.GameTime gt, PloobsEngine.Cameras.ICamera cam, IWorld world)
        { 	 
            particleSystem.Emitter.PositionData.Position = this.PhysicObject.Position;
            particleSystem.Emitter.PositionData.Velocity = this.PhysicObject.Velocity;
            base.UpdateObject(gt, cam, world);
        }
    }
}

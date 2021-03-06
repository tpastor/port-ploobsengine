﻿#if !MONO
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PloobsEngine.SceneControl;
using PloobsEngine.Engine;
using PloobsEngine.Engine.Logger;

namespace PloobsEngine.Modelo
{

    /// <summary>
    /// Represent each billboard
    /// </summary>
    public struct BilboardInstance
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// Scale
        /// </summary>
        public Vector2 Scale;            
    }

    public class InstancedBilboardModel : IModelo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstancedBilboardModel"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="BilboardsName">Name of the bilboards.</param>
        /// <param name="diffuseTextureName">Name of the diffuse texture.</param>
        /// <param name="instances">The instances.</param>
        /// <param name="dynamicBufferSize">Size of the dynamic buffer, use this if you want something different from number of instances</param>
        public InstancedBilboardModel(GraphicFactory factory, String BilboardsName, String diffuseTextureName, BilboardInstance[] instances, int  dynamicBufferSize = -1 )
            : base(factory, BilboardsName  ,false)
        {
            this.instances = instances;
            this.diffuseTextureName = diffuseTextureName;            
            this.dynamicBufferSize = dynamicBufferSize;
            LoadModelo(factory);
        }

        string diffuseTextureName;
        private float modelRadius = -1;
        BilboardInstance[] instances;
        int dynamicBufferSize = -1;

        /// <summary>
        /// Sets the bilboard instances.
        /// </summary>
        /// <param name="instances">The instances.</param>
        public void SetBilboardInstances(BilboardInstance[] instances)
        {
            if(instances.Count() < dynamicBufferSize)
            {
                this.instances = instances;
                BatchInformations[0][0].InstanceCount = instances.Count();
                BatchInformations[0][0].InstancedVertexBuffer.SetData(instances);
            }
            else
            {
                ActiveLogger.LogMessage("Calling SetBilboardInstances with different BilboardInstance size is not recomended, lot of performance penalty here", LogLevel.Warning);
                this.instances = instances;
                VertexBuffer InstancedvertexBufferS = factory.CreateDynamicVertexBuffer(vd, instances.Count(), BufferUsage.WriteOnly);
                InstancedvertexBufferS.SetData(instances);
                BatchInformations[0][0].InstanceCount = instances.Count();
                BatchInformations[0][0].InstancedVertexBuffer = InstancedvertexBufferS;
            }            
        }

        /// <summary>
        /// Gets the bilboard instances.
        /// </summary>
        /// <returns></returns>
        public BilboardInstance[] GetBilboardInstances()
        {
            return instances;
        }

        VertexDeclaration vd;
        /// <summary>
        /// Loads the model.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="BatchInformations">The batch informations.</param>
        /// <param name="TextureInformations">The texture informations.</param>
        protected override void LoadModel(GraphicFactory factory, out BatchInformation[][] BatchInformations, out TextureInformation[][] TextureInformations)
        {
            VertexPositionTexture[] billboardVertices = new VertexPositionTexture[4];
            int i = 0;
            
            billboardVertices[i++] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 0));
            billboardVertices[i++] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 0));
            billboardVertices[i++] = new VertexPositionTexture(Vector3.Zero, new Vector2(0, 1));
            billboardVertices[i++] = new VertexPositionTexture(Vector3.Zero, new Vector2(1, 1));

            VertexElement v0 = new VertexElement(0,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,1);
            VertexElement v1 = new VertexElement(sizeof(float) * 3,VertexElementFormat.Vector2,VertexElementUsage.TextureCoordinate,2);

             vd = new VertexDeclaration(v0, v1);

             VertexBuffer InstancedvertexBufferS;
             if (dynamicBufferSize != -1)
             {
                 InstancedvertexBufferS = factory.CreateDynamicVertexBuffer(vd, dynamicBufferSize, BufferUsage.WriteOnly);
             }
             else
             {
                 InstancedvertexBufferS = factory.CreateDynamicVertexBuffer(vd, instances.Count(), BufferUsage.WriteOnly);
             }            
        
            InstancedvertexBufferS.SetData(instances);

            VertexBuffer vertexBufferS = factory.CreateVertexBuffer(VertexPositionTexture.VertexDeclaration, billboardVertices.Count(), BufferUsage.WriteOnly);
            vertexBufferS.SetData(billboardVertices);

            short[] indices = new short[] { 0,2,1,1,2,3};

            IndexBuffer indexBufferS = factory.CreateIndexBuffer(IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            indexBufferS.SetData<short>(indices);
            
            BatchInformations = new BatchInformation[1][];
            BatchInformation[] b = new BatchInformation[1];
            b[0] = new BatchInformation(0, 4, 2, 0, 0, VertexPositionTexture.VertexDeclaration, VertexPositionTexture.VertexDeclaration.VertexStride,instances.Count());
            b[0].VertexBuffer = vertexBufferS;
            b[0].IndexBuffer = indexBufferS;
            b[0].InstancedVertexBuffer = InstancedvertexBufferS;
            BatchInformations[0] = b;

            TextureInformations = new TextureInformation[1][];
            TextureInformations[0] = new TextureInformation[1];
            TextureInformations[0][0] = new TextureInformation(isInternal, factory, diffuseTextureName, null, null, null);
            TextureInformations[0][0].LoadTexture();
        }


        /// <summary>
        /// Gets the Total mesh number.
        /// </summary>
        public override int MeshNumber
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the model radius.
        /// </summary>
        /// <returns></returns>
        public override float GetModelRadius()
        {
            return modelRadius;
        }
    }
    
}
#endif
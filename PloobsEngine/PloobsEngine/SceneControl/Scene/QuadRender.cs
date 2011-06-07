﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace PloobsEngine.SceneControl
{
    /// <summary>
    /// Helper class to render full screen quads
    /// VertexPositionTexture
    /// </summary>
    internal sealed class QuadRender
    {
        private VertexPositionTexture[] verts;                 
        private GraphicsDevice myDevice;
        private short[] ib = null;

        /// <summary>
        /// Loads the quad.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public QuadRender(GraphicsDevice device) 
        {
            
            myDevice = device;         
            
            verts = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,0))
                        };

             ib = new short[] { 0, 1, 2, 2, 3, 0 };


        }             

        /// <summary>
        /// Draws the quad.
        /// </summary>
        /// <param name="effect">The effect.</param>
        public void DrawQuad(Effect effect)
        {
            effect.CurrentTechnique.Passes[0].Apply();
            Render(Vector2.One * -1, Vector2.One);
        }

        public void Render(Vector2 v1, Vector2 v2)
        {          
          
            verts[0].Position.X = v2.X;
            verts[0].Position.Y = v1.Y;

            verts[1].Position.X = v1.X;
            verts[1].Position.Y = v1.Y;

            verts[2].Position.X = v1.X;
            verts[2].Position.Y = v2.Y;

            verts[3].Position.X = v2.X;
            verts[3].Position.Y = v2.Y;

            myDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
                (PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
        }
    
    }
}

using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoPhysics
{
    /// <summary>
    /// DrawVertDeclaration class
    /// </summary>
    public static class DrawVertDeclaration
    {
        #region Fields

        /// <summary>
        /// The declaration of a vertex
        /// </summary>
        public static readonly VertexDeclaration Declaration;

        /// <summary>
        /// The size of the vertex
        /// </summary>
        public static readonly int Size;

        #endregion Fields

        #region Public Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static DrawVertDeclaration()
        {
            unsafe { Size = sizeof(ImDrawVert); }

            Declaration = new VertexDeclaration(
                Size,

                // Position
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),

                // UV
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),

                // Color
                new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );
        }

        #endregion Public Constructors
    }
}
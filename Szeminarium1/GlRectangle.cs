using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafikaSzeminarium
{
    internal class GlRectangle
    {
        public uint Vao { get; }
        public uint Vertices { get; }
        public uint Colors { get; }
        public uint Indices { get; }
        public uint IndexArrayLength { get; }

        private GL Gl;

        private GlRectangle(uint vao, uint vertices, uint colors, uint indeces, uint indexArrayLength, GL gl)
        {
            this.Vao = vao;
            this.Vertices = vertices;
            this.Colors = colors;
            this.Indices = indeces;
            this.IndexArrayLength = indexArrayLength;
            this.Gl = gl;
        }

        public static unsafe GlRectangle CreateGlRectangle(GL Gl, float[] color)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            float[] vertexArray = new float[] {
                -0.5f, 1.0f, 0.0f, 0f, 0f, 1f,	   //0 BAL FELSO
				-0.5f, -1.0f, 0.0f, 0f, 0f, 1f,    //1 BAL ALSO
				0.5f, -1.0f, 0.0f, 0f, 0f, 1f,     //2 JOBB ALSO
				0.5f, 1.0f, 0.0f, 0f, 0f, 1f, 	   //3 JOBB FELSO
            };

            List<float> colorsList = new List<float>();
            colorsList.AddRange(color);
            colorsList.AddRange(color);
            colorsList.AddRange(color);
            colorsList.AddRange(color);

            float[] colorArray = colorsList.ToArray();

            uint[] indexArray = new uint[] {
                0, 1, 2,
                0, 2, 3,
            };

            uint offsetPos = 0;
            uint offsetNormal = offsetPos + (3 * sizeof(float));
            uint vertexSize = offsetNormal + (3 * sizeof(float));

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetPos);
            Gl.EnableVertexAttribArray(0);

            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetNormal);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);

            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            uint indexArrayLength = (uint)indexArray.Length;

            return new GlRectangle(vao, vertices, colors, indices, indexArrayLength, Gl);
        }

        public static unsafe GlRectangle CreateRectangleXZNormalVector(GL Gl, float[] color)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            float angle = 10 * (float)(Math.PI / 180);
            Vector3D<float>[] normalVector = new Vector3D<float>[4];

            for (int i = 0; i < 4; i++)
            {
                normalVector[i].X = (float)Math.Sin(angle);
                normalVector[i].Y = 0f;
                normalVector[i].Z = (float)Math.Cos(angle);
            }

            float[] vertexArray = new float[] {
                -0.5f, 1.0f, 0.0f, -normalVector[0].X, normalVector[0].Y, normalVector[0].Z,	   //0 BAL FELSO
				-0.5f, -1.0f, 0.0f,  -normalVector[1].X, normalVector[1].Y, normalVector[1].Z,     //1 BAL ALSO
				0.5f, -1.0f, 0.0f,  normalVector[2].X, normalVector[2].Y, normalVector[2].Z,       //2 JOBB ALSO
				0.5f, 1.0f, 0.0f,  normalVector[3].X, normalVector[3].Y, normalVector[3].Z, 	   //3 JOBB FELSO
			};

            List<float> colorList = new List<float>();
            colorList.AddRange(color);
            colorList.AddRange(color);
            colorList.AddRange(color);
            colorList.AddRange(color);

            float[] colorArray = colorList.ToArray();

            uint[] indexArray = new uint[] {
                0, 1, 2,
                0, 2, 3,
            };

            uint offsetPos = 0;
            uint offsetNormal = offsetPos + (3 * sizeof(float));
            uint vertexSize = offsetNormal + (3 * sizeof(float));

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetPos);
            Gl.EnableVertexAttribArray(0);

            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetNormal);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);

            // release array buffer
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            uint indexArrayLength = (uint)indexArray.Length;

            return new GlRectangle(vao, vertices, colors, indices, indexArrayLength, Gl);
        }

        internal void ReleaseGlRectangle()
        {
            // always unbound the vertex buffer first, so no halfway results are displayed by accident
            Gl.DeleteBuffer(Vertices);
            Gl.DeleteBuffer(Colors);
            Gl.DeleteBuffer(Indices);
            Gl.DeleteVertexArray(Vao);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using static System.Formats.Asn1.AsnWriter;

namespace GrafikaSzeminarium
{
    internal class FbxResourceReader
    {
        public static unsafe GlObject CreateRedBallFromFbx(GL Gl, string fbxPath, float[] faceColor)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            if (string.IsNullOrEmpty(fbxPath))
                throw new ArgumentException("FBX path is null or empty.");


            List<float> glVertices = new List<float>();
            List<float> glColors = new List<float>();
            List<uint> glIndices = new List<uint>();

            LoadFbxAndCreateGlArrays(fbxPath, faceColor, glVertices, glColors, glIndices);

            return CreateOpenGlObject(Gl, vao, glVertices, glColors, glIndices);
        }

        private static unsafe void LoadFbxAndCreateGlArrays(string fbxPath, float[] faceColor, List<float> glVertices, List<float> glColors, List<uint> glIndices)
        {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(fbxPath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals);

            uint vertexOffset = 0;

            foreach (var mesh in scene.Meshes)
            {
                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    var pos = mesh.Vertices[i];
                    var norm = mesh.HasNormals ? mesh.Normals[i] : new Assimp.Vector3D(0, 1, 0);

                    glVertices.AddRange(new float[]
                    {
                        pos.X, pos.Y, pos.Z,
                        norm.X, norm.Y, norm.Z
                    });

                    glColors.AddRange(faceColor); // Add the color per vertex
                }

                foreach (var face in mesh.Faces)
                {
                    if (face.IndexCount == 3)
                    {
                        glIndices.Add((uint)(vertexOffset + face.Indices[0]));
                        glIndices.Add((uint)(vertexOffset + face.Indices[1]));
                        glIndices.Add((uint)(vertexOffset + face.Indices[2]));
                    }
                }

                vertexOffset += (uint)mesh.Vertices.Count;
            }
        }

        private static unsafe GlObject CreateOpenGlObject(GL Gl, uint vao, List<float> glVertices, List<float> glColors, List<uint> glIndices)
        {
            uint offsetPos = 0;
            uint offsetNormal = offsetPos + (3 * sizeof(float));
            uint vertexSize = offsetNormal + (3 * sizeof(float));

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)glVertices.ToArray(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetPos);
            Gl.EnableVertexAttribArray(0);

            Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetNormal);
            Gl.EnableVertexAttribArray(2);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)glColors.ToArray(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)glIndices.ToArray(), GLEnum.StaticDraw);

            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            return new GlObject(vao, vertices, colors, indices, (uint)glIndices.Count, Gl);
        }
    }
}

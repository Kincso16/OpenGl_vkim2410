using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace GrafikaSzeminarium
{
    internal class FbxResourceReaderTextured
    {
        public static unsafe GlObject CreateTexturedRedBall(GL gl, string fbxPath, string texturePath)
        {
            if (string.IsNullOrEmpty(fbxPath))
                throw new ArgumentException("FBX path is null or empty.");

            if (!File.Exists(texturePath))
                throw new FileNotFoundException("Texture image not found.", texturePath);

            uint vao = gl.GenVertexArray();
            gl.BindVertexArray(vao);

            var glVertices = new List<float>();
            var glTexCoords = new List<float>();
            var glNormals = new List<float>();
            var glIndices = new List<uint>();

            LoadFbxData(fbxPath, glVertices, glTexCoords, glNormals, glIndices);

            var finalVertexData = new List<float>();
            for (int i = 0; i < glVertices.Count / 3; i++)
            {
                finalVertexData.AddRange(glVertices.GetRange(i * 3, 3));
                finalVertexData.AddRange(glNormals.GetRange(i * 3, 3));
                finalVertexData.AddRange(glTexCoords.GetRange(i * 2, 2));
            }

            uint vertexSize = (3 + 3 + 2) * sizeof(float);
            uint offsetPos = 0;
            uint offsetNormal = 3 * sizeof(float);
            uint offsetTexCoord = offsetNormal + 3 * sizeof(float);

            uint vbo = gl.GenBuffer();
            gl.BindBuffer(GLEnum.ArrayBuffer, vbo);
            fixed (float* v = finalVertexData.ToArray())
            {
                gl.BufferData(GLEnum.ArrayBuffer, (nuint)(finalVertexData.Count * sizeof(float)), v, GLEnum.StaticDraw);
            }

            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetPos);
            gl.EnableVertexAttribArray(0);

            gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetNormal);
            gl.EnableVertexAttribArray(2);

            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetTexCoord);
            gl.EnableVertexAttribArray(1);

            uint ebo = gl.GenBuffer();
            gl.BindBuffer(GLEnum.ElementArrayBuffer, ebo);
            fixed (uint* i = glIndices.ToArray())
            {
                gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(glIndices.Count * sizeof(uint)), i, GLEnum.StaticDraw);
            }

            uint texture = LoadTexture(gl, texturePath);

            gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            gl.BindVertexArray(0);

            return new GlObject(vao, vbo, texture, ebo, (uint)glIndices.Count, gl);
        }

        private static void LoadFbxData(string fbxPath, List<float> vertices, List<float> texCoords, List<float> normals, List<uint> indices)
        {
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(fbxPath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.FlipUVs);

            uint vertexOffset = 0;

            foreach (var mesh in scene.Meshes)
            {
                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    var pos = mesh.Vertices[i];
                    var norm = mesh.HasNormals ? mesh.Normals[i] : new Assimp.Vector3D(0, 1, 0);
                    var uv = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : new Assimp.Vector3D(0, 0, 0);

                    vertices.AddRange(new float[] { pos.X, pos.Y, pos.Z });
                    normals.AddRange(new float[] { norm.X, norm.Y, norm.Z });
                    texCoords.AddRange(new float[] { uv.X, uv.Y });
                }

                foreach (var face in mesh.Faces)
                {
                    if (face.IndexCount == 3)
                    {
                        indices.Add(vertexOffset + (uint)face.Indices[0]);
                        indices.Add(vertexOffset + (uint)face.Indices[1]);
                        indices.Add(vertexOffset + (uint)face.Indices[2]);
                    }
                }

                vertexOffset += (uint)mesh.Vertices.Count;
            }
        }

        private static unsafe uint LoadTexture(GL gl, string path)
        {
            using var stream = File.OpenRead(path);
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            uint texture = gl.GenTexture();
            gl.BindTexture(GLEnum.Texture2D, texture);

            fixed (byte* ptr = image.Data)
            {
                gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba, (uint)image.Width, (uint)image.Height, 0,
                    GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
            }

            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.Repeat);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.Repeat);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);

            gl.GenerateMipmap(GLEnum.Texture2D);

            return texture;
        }
    }
}

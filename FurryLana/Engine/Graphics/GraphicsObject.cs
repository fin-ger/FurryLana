//
//  GraphicsObject.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
#define Vertex

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics.Interfaces;
using FurryLana.Engine.Graphics.Shader;
using FurryLana.Engine.Graphics.VertexBuffer;
using FurryLana.Engine.Texture.Interfaces;
using ShaderType = FurryLana.Engine.Graphics.Shader.ShaderType;

namespace FurryLana.Engine.Graphics
{
    public class GraphicsObject : IGraphicsResource
    {
        void Create (ShaderProgram shader, Vertex[] vertices, int[] indices, ITexture[] textures)
        {
            Shader = shader;
#if Vertex
            VBO = new VertexBuffer<Vertex> (VertexBufferType.Static, vertices.Length,
                                            new VertexFormatInfo 
                                            { VertexParams = new VertexAttribParam[] {
                    new VertexAttribParam (0, 4, Marshal.SizeOf (typeof (Vertex)), 0),
                    new VertexAttribParam (1, 3, Marshal.SizeOf (typeof (Vertex)), 4 * sizeof (float)),
                    new VertexAttribParam (2, 2, Marshal.SizeOf (typeof (Vertex)), 7 * sizeof (float))
                }});

            VBO.LoadData (vertices);
#else
            VBO = new VertexBuffer<Vector4> (VertexBufferType.Static, vertices.Length,
                                             new VertexFormatInfo {
                VertexParams = new VertexAttribParam[] {
                    new VertexAttribParam (0, 4, 16, 0)
                }
            });

            Vector4[] vrt = vertices.Select (s => s.Position).ToArray ();
            VBO.LoadData (vrt);
#endif

            IBO = new VertexBuffer<int> (VertexBufferType.Static, indices.Length,
                                         new VertexFormatInfo (), VertexBufferTarget.IndiceBuffer);
            IBO.LoadData (indices);
            VAO = new VertexArrayObject ();

            if (textures.Length > 32)
                throw new IndexOutOfRangeException ("You must not bind more than 32 textures to one graphics object!");

            Textures = textures;
        }

        void LoadModel (string modelPath, out Vertex[] vertices, out int[] indices)
        {
            Vector4[] vrt;
            Vector3[] norm;
            Vector2[] texcoord;
            GL.Utils.LoadModel (modelPath, out vrt, out norm, out texcoord, out indices, false);

            if (norm.Length == 0 && texcoord.Length == 0)
                vertices = Enumerable.Select (vrt, (v, i) => new Vertex (v, new Vector3 (0, 0, 0), new Vector2 (0, 0)))
                    .ToArray ();
            else if (norm.Length == 0)
                vertices = Enumerable.Select (vrt, (v, i) => new Vertex (v, new Vector3 (0, 0, 0), texcoord[i]))
                    .ToArray ();
            else if (texcoord.Length == 0)
                vertices = Enumerable.Select (vrt, (v, i) => new Vertex (v, norm[i], new Vector2 (0, 0))).ToArray ();
            else
                vertices = Enumerable.Select (vrt, (v, i) => new Vertex (v, norm[i], texcoord[i])).ToArray ();
        }

        ITexture[] LoadTextures (string[] texturePaths)
        {
            ITexture[] textures = new ITexture[texturePaths.Length];
            int i = 0;
            foreach (var path in texturePaths)
            {
                Engine.Application.Application.Instance.ResourceManager.TextureManager.LoadFromLocation (path);
                textures[i++] = Engine.Application.Application.Instance.ResourceManager.TextureManager.GetByName (path);
            }

            return textures;
        }

        public GraphicsObject (ShaderProgram shader, Vertex[] vertices, int[] indices, ITexture[] textures)
        {
            DoLoad = () => {
                Create (shader, vertices, indices, textures);
            };
        }

        public GraphicsObject (ShaderProgram shader, string modelPath, string[] texturePaths)
        {
            DoLoad = () => {
                Vertex[] vertices;
                int[] indices;
                LoadModel (modelPath, out vertices, out indices);

                ITexture[] textures = LoadTextures (texturePaths);

                Create (shader, vertices, indices, textures);
            };
        }

        public GraphicsObject (string fragmentShader, string vertexShader, Vertex[] vertices, int[] indices,
                               string[] texturePaths)
        {
            DoLoad = () => {
                ShaderProgram shader = new ShaderProgram (new Shader.Shader (ShaderType.FragmentShader, fragmentShader),
                                                          new Shader.Shader (ShaderType.VertexShader, vertexShader));

                ITexture[] textures = LoadTextures (texturePaths);

                Create (shader, vertices, indices, textures);
            };
        }

        public GraphicsObject (string fragmentShader, string vertexShader, string modelPath, ITexture[] textures)
        {
            DoLoad = () => {
                ShaderProgram shader = new ShaderProgram (new Shader.Shader (ShaderType.FragmentShader, fragmentShader),
                                                          new Shader.Shader (ShaderType.VertexShader, vertexShader));

                Vertex[] vertices;
                int[] indices;
                LoadModel (modelPath, out vertices, out indices);

                Create (shader, vertices, indices, textures);
            };
        }

        public GraphicsObject (ShaderProgram shader, Vertex[] vertices, int[] indices, string[] texturePaths)
        {
            DoLoad = () => {
                ITexture[] textures = LoadTextures (texturePaths);

                Create (shader, vertices, indices, textures);
            };
        }

        public GraphicsObject (string fragmentShader, string vertexShader, Vertex[] vertices, int[] indices, ITexture[] textures)
        {
            DoLoad = () => {
                ShaderProgram shader = new ShaderProgram (new Shader.Shader (ShaderType.FragmentShader, fragmentShader),
                                                          new Shader.Shader (ShaderType.VertexShader, vertexShader));

                Create (shader, vertices, indices, textures);
            };
        }

        public GraphicsObject (ShaderProgram shader, string modelPath, ITexture[] textures)
        {
            DoLoad = () => {
                Vertex[] vertices;
                int[] indices;
                LoadModel (modelPath, out vertices, out indices);

                Create (shader, vertices, indices, textures);
            };
        }

        public GraphicsObject (string fragmentShader, string vertexShader, string modelPath, string[] texturePaths)
        {
            DoLoad = () => {
                ShaderProgram shader = new ShaderProgram (new Shader.Shader (ShaderType.FragmentShader, fragmentShader),
                                                          new Shader.Shader (ShaderType.VertexShader, vertexShader));

                Vertex[] vertices;
                int[] indices;
                LoadModel (modelPath, out vertices, out indices);

                ITexture[] textures = LoadTextures (texturePaths);

                Create (shader, vertices, indices, textures);
            };
        }

        public ShaderProgram     Shader         { get; set; }
        public VertexArrayObject VAO            { get; set; }
        public ITexture[]        Textures       { get; set; }
        public Vector3           LightDirection { get; set; }

#if Vertex
        protected VertexBuffer<Vertex> VBO;
#else
        protected VertexBuffer<Vector4> VBO;
#endif
        protected VertexBuffer<int> IBO;
        protected Matrix ProjMatrix = Matrix.CreatePerspectiveFieldOfView ((float) (System.Math.PI / 3), 16f / 9, 0.1f, 200);
        protected Matrix ViewMatrix = Matrix.LookAt (2,3,2,
                                                     0,0,0,
                                                     0,1,0);
        protected Matrix ModelMatrix = Matrix.Identity;
        protected Action DoLoad;
        public int Sampler;

        #region IResource implementation

        public void Init ()
        {
            Loaded = false;
            LightDirection = new Vector3 (0.6f, 0.6f, 0.6f);
        }

        public List<Action> GetInitJobs (List<Action> list)
        {
            list.Add (Init);
            return list;
        }

        public void Load ()
        {
            Loaded = false;

            ViewMatrix *= Matrix.CreateRotationY (0.75f);

            DoLoad ();

            VBO.Load ();
            IBO.Load ();
            VAO.AttachVBO (VBO);
            VAO.AttachVBO (IBO);
            VAO.Load ();
            VertexArrayObject.UnbindVAO ();
            Shader.Load ();

            //Sampler = GL.GenSampler ();
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureWrapS, (int) TextureWrapMode.Repeat);
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureWrapT, (int) TextureWrapMode.Repeat);
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureWrapR, (int) TextureWrapMode.Repeat);
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureMinFilter, (int) TextureMinFilter.Linear);
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureMagFilter, (int) TextureMagFilter.Linear);
            //GL.BindSampler (0, Sampler);
            
            Shader["ProjMatrix"] = ProjMatrix;
            Shader["ViewMatrix"] = ViewMatrix;
            Shader["ModelMatrix"] = ModelMatrix;
            Shader["Ambient"] = 0.2f;
            Shader["LightDirection"] = LightDirection;
            //Shader["DiffuseTexture"] = Sampler;

            Loaded = true;
        }

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            list.Add (Load);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy ()
        {
            VAO.Destroy ();
            VBO.Destroy ();
            IBO.Destroy ();
            Shader.Destroy ();
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        public void FrameSyncedUpdate (float deltaTime)
        {
            ViewMatrix *= Matrix.CreateRotationY (-0.2f * deltaTime);
            Shader["ViewMatrix"] = ViewMatrix;
        }

        #endregion

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {
        }

        #endregion

        protected string[] textureUnitNames = new string[]
        {
            "DiffuseTexture",
            "AmbientTexture",
            "NormalHeightTexture",
            "SpecularTexture",
            "GeneralTexture00",
            "GeneralTexture01",
            "GeneralTexture02",
            "GeneralTexture03",
            "GeneralTexture04",
            "GeneralTexture05",
            "GeneralTexture06",
            "GeneralTexture07",
            "GeneralTexture08",
            "GeneralTexture09",
            "GeneralTexture10",
            "GeneralTexture11",
            "GeneralTexture12",
            "GeneralTexture13",
            "GeneralTexture14",
            "GeneralTexture15",
            "GeneralTexture16",
            "GeneralTexture17",
            "GeneralTexture18",
            "GeneralTexture19",
            "GeneralTexture20",
            "GeneralTexture21",
            "GeneralTexture22",
            "GeneralTexture23",
            "GeneralTexture24",
            "GeneralTexture25",
            "GeneralTexture26",
            "GeneralTexture27"
        };

        public string[] TextureUnitNames
        {
            get
            {
                return textureUnitNames;
            }
        }

        #region IDrawable implementation

        public void Draw ()
        {
            for (int i = 0; i < Textures.Length; i++)
            {
                TextureUnit unit = (TextureUnit) ((int) TextureUnit.Texture0 + i);
                GL.ActiveTexture (unit);
                Textures[i].Bind ();
                Shader[TextureUnitNames[i]] = i;
            }

            VAO.Bind ();
            using (var handle = Shader.Use ())
            {
                GL.DrawElements (BeginMode.Triangles, IBO.Count, DrawElementsType.UnsignedInt, 0);
            }
            VertexArrayObject.UnbindVAO ();
        }

        #endregion
    }
}
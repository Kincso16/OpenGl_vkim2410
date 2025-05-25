using GrafikaSzeminarium;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Input.Extensions;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using ErrorCode = Silk.NET.GLFW.ErrorCode;
using System.Numerics;


namespace GrafikaSzeminarium
{
    internal static class Program
    {
        private static CameraDescriptor cameraDescriptor = new();

        private static CubeArrangementModel cubeArrangementModel = new();

        private static IWindow window;

        private static IInputContext inputContext;

        private static GL Gl;

        private static ImGuiController controller;

        private static uint program;

        private static GlObject plant;

        private static GlObject redball;

        private static GlObject table;

        private static GlObject enemy;

        private static GlCube skyBox;

        private static float Shininess = 50;

        private const string ModelMatrixVariableName = "uModel";
        private const string NormalMatrixVariableName = "uNormal";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private const string TextureUniformVariableName = "uTexture";

        private const string LightColorVariableName = "lightColor";
        private const string LightPositionVariableName = "lightPos";
        private const string ViewPosVariableName = "viewPos";
        private const string ShininessVariableName = "shininess";

        private static float enemyTime = 0f;
        private static float enemyRadius = 0.5f; 
        private static float redBallRadius = 0.5f;
        private static int collisionCount = 0;
        private static Vector3D<float> redBallPosition = new(0f, 0.65f, 0f); 
        private static Vector3D<float> movement = Vector3D<float>.Zero;


        private static Vector3D<float>[] enemyPositions = new Vector3D<float>[]
        {
            new Vector3D<float>(MathF.Sin(enemyTime) * 5f, 1f, 10f),
            new Vector3D<float>(MathF.Cos(enemyTime)* 5f + 10f, 1f, MathF.Sin(enemyTime)* 5f ),
            new Vector3D<float>(0f, 1f, MathF.Sin(enemyTime)* 5f - 10f),
        };
  
        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "2 szeminárium";
            windowOptions.Size = new Vector2D<int>(1000, 1000);

            // on some systems there is no depth buffer by default, so we need to make sure one is created
            windowOptions.PreferredDepthBufferBits = 24;

            InitializeCollisionFlags(); 

            window = Window.Create(windowOptions);

            window.Load += Window_Load;
            window.Update += Window_Update;
            window.Render += Window_Render;
            window.Closing += Window_Closing;

            window.Run();
        }

        private static void Window_Load()
        {
            //Console.WriteLine("Load");

            // set up input handling
            inputContext = window.CreateInput();
            foreach (var keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
                keyboard.KeyUp += Keyboard_KeyUp;
            }

            Gl = window.CreateOpenGL();

            controller = new ImGuiController(Gl, window, inputContext);

            // Handle resizes
            window.FramebufferResize += s =>
            {
                // Adjust the viewport to the new window size
                Gl.Viewport(s);
            };


            Gl.ClearColor(System.Drawing.Color.Black);

            SetUpObjects();

            LinkProgram();

            //Gl.Enable(EnableCap.CullFace);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);
        }

        private static void LinkProgram()
        {
            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, ReadShader("VertexShader.vert"));
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, ReadShader("FragmentShader.frag"));
            Gl.CompileShader(fshader);

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);
            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);
        }

        private static string ReadShader(string shaderFileName)
        {
            using (Stream shaderStream = typeof(Program).Assembly.GetManifestResourceStream("Szeminarium1.Shaders." + shaderFileName))
            using (StreamReader shaderReader = new StreamReader(shaderStream))
                return shaderReader.ReadToEnd();
        }

        private static void Keyboard_KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.Left:
                    cameraDescriptor.DecreaseZYAngle();
                    break;
                    ;
                case Key.Right:
                    cameraDescriptor.IncreaseZYAngle();
                    break;
                case Key.Down:
                    cameraDescriptor.IncreaseDistance();
                    break;
                case Key.Up:
                    cameraDescriptor.DecreaseDistance();
                    break;
                case Key.U:
                    cameraDescriptor.IncreaseZXAngle();
                    break;
                case Key.Y:
                    cameraDescriptor.DecreaseZXAngle();
                    break;
                case Key.Space:
                    cubeArrangementModel.AnimationEnabeld = !cubeArrangementModel.AnimationEnabeld;
                    break;
                case Key.F1:
                    cameraDescriptor.SetCameraMode(CameraDescriptor.CameraMode.Default);
                    break;
                case Key.F2:
                    cameraDescriptor.SetCameraMode(CameraDescriptor.CameraMode.RedBallFirstPerson);
                    break;
                case Key.F3:
                    cameraDescriptor.SetCameraMode(CameraDescriptor.CameraMode.RedBallThirdPerson);
                    break;

                case Key.W:
                    movement.Z += 1;
                    break;
                case Key.S:
                    movement.Z -= 1;
                    break;
                case Key.A:
                    movement.X += 1;
                    break;
                case Key.D:
                    movement.X -= 1;
                    break;
            }

        }
        private static void Keyboard_KeyUp(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.W:
                    movement.Z = 0;
                    break;
                case Key.S:
                    movement.Z = 0;
                    break;
                case Key.A:
                    movement.X = 0;
                    break;
                case Key.D:
                    movement.X = 0;
                    break;
            }
        }

        private static void Window_Update(double deltaTime)
        {
            //Console.WriteLine($"Update after {deltaTime} [s].");
            // multithreaded
            // make sure it is threadsafe
            // NO GL calls
            cubeArrangementModel.AdvanceTime(deltaTime);

            controller.Update((float)deltaTime);

            UpdateRedBall((float)deltaTime);
            enemyTime += (float)deltaTime;
            UpdateEnemies(enemyTime);
            CheckCollisionsWithFlags();
        }

        private static unsafe void Window_Render(double deltaTime)
        {
            //Console.WriteLine($"Render after {deltaTime} [s].");

            // GL here
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);


            Gl.UseProgram(program);

            SetViewMatrix();
            SetProjectionMatrix();

            SetLightColor();
            SetLightPosition();
            SetViewerPosition();
            SetShininess();

            DrawRedBall();

            DrawSkyBox();
        
            DrawEnemies();

            DrawPlant();

            ImGuiNET.ImGui.Begin("info", ImGuiWindowFlags.AlwaysAutoResize);
            ImGuiNET.ImGui.Text($"Ütközések száma: {collisionCount}");
            ImGuiNET.ImGui.End();

            controller.Render();
        }

        private static unsafe void DrawSkyBox()
        {
            Matrix4X4<float> modelMatrix = Matrix4X4.CreateScale(400f);
            SetModelMatrix(modelMatrix);
            Gl.BindVertexArray(skyBox.Vao);

            int textureLocation = Gl.GetUniformLocation(program, TextureUniformVariableName);
            if (textureLocation == -1)
            {
                throw new Exception($"{TextureUniformVariableName} uniform not found on shader.");
            }
            // set texture 0
            Gl.Uniform1(textureLocation, 0);

            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)GLEnum.Linear);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)GLEnum.Linear);
            Gl.BindTexture(TextureTarget.Texture2D, skyBox.Texture.Value);

            Gl.DrawElements(GLEnum.Triangles, skyBox.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);

            CheckError();
            Gl.BindTexture(TextureTarget.Texture2D, 0);
            CheckError();
        }

        private static unsafe void SetLightColor()
        {
            int location = Gl.GetUniformLocation(program, LightColorVariableName);

            if (location == -1)
            {
                throw new Exception($"{LightColorVariableName} uniform not found on shader.");
            }

            Gl.Uniform3(location, 1f, 1f, 1f);
            CheckError();
        }

        private static unsafe void SetLightPosition()
        {
            int location = Gl.GetUniformLocation(program, LightPositionVariableName);

            if (location == -1)
            {
                throw new Exception($"{LightPositionVariableName} uniform not found on shader.");
            }

            Gl.Uniform3(location, 0f, 10f, 0f);
            CheckError();
        }

        private static unsafe void SetViewerPosition()
        {
            int location = Gl.GetUniformLocation(program, ViewPosVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewPosVariableName} uniform not found on shader.");
            }

            Gl.Uniform3(location, cameraDescriptor.Position.X, cameraDescriptor.Position.Y, cameraDescriptor.Position.Z);
            CheckError();
        }

        private static unsafe void SetShininess()
        {
            int location = Gl.GetUniformLocation(program, ShininessVariableName);

            if (location == -1)
            {
                throw new Exception($"{ShininessVariableName} uniform not found on shader.");
            }

            Gl.Uniform1(location, Shininess);
            CheckError();
        }

        private static unsafe void DrawRedBall()
        {
            var modelMatrixForRedBall = Matrix4X4.CreateScale(0.5f) *
                                        Matrix4X4.CreateTranslation(redBallPosition);
            SetModelMatrix(modelMatrixForRedBall);

            Gl.BindVertexArray(redball.Vao);
            Gl.DrawElements(GLEnum.Triangles, redball.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);

            var modelMatrixForTable = Matrix4X4.CreateScale(1f, 1f, 1f);
            SetModelMatrix(modelMatrixForTable);
            Gl.BindVertexArray(table.Vao);
            Gl.DrawElements(GLEnum.Triangles, table.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);
        }

        
        private static void UpdateRedBall(float deltaTime)
        {
            
            float speed = 5f;

            if (movement.LengthSquared > 0)
            {
                movement = Vector3D.Normalize(movement) * speed * deltaTime;
                redBallPosition += movement;
                cameraDescriptor.UpdateRedBallPosition(redBallPosition);
            }
        }

        private static unsafe void DrawPlant()
        {
            var modelMatrix = Matrix4X4.CreateScale(0.5f) *
                  Matrix4X4.CreateRotationX(-MathF.PI / 2) *
                  Matrix4X4.CreateTranslation(new Vector3D<float>(-10f, 0.65f, 0f));

            SetModelMatrix(modelMatrix);
            Gl.BindVertexArray(plant.Vao);
            Gl.DrawElements(GLEnum.Triangles, plant.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);
        }


        private static unsafe void DrawEnemies()
        {
            foreach (var enemyPos in enemyPositions)
            {
                var modelMatrix = Matrix4X4.CreateScale(0.5f) *
                          Matrix4X4.CreateTranslation(enemyPos);

                SetModelMatrix(modelMatrix);

                Gl.BindVertexArray(enemy.Vao);
                Gl.DrawElements(GLEnum.Triangles, enemy.IndexArrayLength, GLEnum.UnsignedInt, null);
                Gl.BindVertexArray(0);
            }
        }
        private static void UpdateEnemies(float enemyTime)
        {
            enemyPositions[0] = new Vector3D<float>(MathF.Sin(enemyTime) * 5f, 1f, 10f);
            enemyPositions[1] = new Vector3D<float>(MathF.Cos(enemyTime) * 5f + 10f,1f, MathF.Sin(enemyTime) * 5f);
            enemyPositions[2] = new Vector3D<float>(0f, 1f,MathF.Sin(enemyTime) * 5f - 10f);
        }

        private static bool isCurrentlyColliding = false;

        private static bool IsColliding(Vector3D<float> pos1, float radius1, Vector3D<float> pos2, float radius2)
        {
            float distance = Vector3D.Distance(pos1, pos2);
            return distance <= (radius1 + radius2);
        }

        private static bool[] hasCollidedWithEnemy;

        private static void InitializeCollisionFlags()
        {
            hasCollidedWithEnemy = new bool[enemyPositions.Length];
        }

        private static void CheckCollisionsWithFlags()
        {
            for (int i = 0; i < enemyPositions.Length; i++)
            {
                bool isCollidingNow = IsColliding(redBallPosition, redBallRadius, enemyPositions[i], enemyRadius);
                if (isCollidingNow && !hasCollidedWithEnemy[i])
                {
                    collisionCount++;
                    hasCollidedWithEnemy[i] = true;
                    if (collisionCount >= 5)
                    {
                        Environment.Exit(0);
                    }
                }
                else if (!isCollidingNow)
                {
                    hasCollidedWithEnemy[i] = false;
                }
            }
        }

        private static unsafe void SetModelMatrix(Matrix4X4<float> modelMatrix)
        {
            int location = Gl.GetUniformLocation(program, ModelMatrixVariableName);
            if (location == -1)
            {
                throw new Exception($"{ModelMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&modelMatrix);
            CheckError();

            var modelMatrixWithoutTranslation = new Matrix4X4<float>(modelMatrix.Row1, modelMatrix.Row2, modelMatrix.Row3, modelMatrix.Row4);
            modelMatrixWithoutTranslation.M41 = 0;
            modelMatrixWithoutTranslation.M42 = 0;
            modelMatrixWithoutTranslation.M43 = 0;
            modelMatrixWithoutTranslation.M44 = 1;

            Matrix4X4<float> modelInvers;
            Matrix4X4.Invert<float>(modelMatrixWithoutTranslation, out modelInvers);
            Matrix3X3<float> normalMatrix = new Matrix3X3<float>(Matrix4X4.Transpose(modelInvers));
            location = Gl.GetUniformLocation(program, NormalMatrixVariableName);
            if (location == -1)
            {
                throw new Exception($"{NormalMatrixVariableName} uniform not found on shader.");
            }
            Gl.UniformMatrix3(location, 1, false, (float*)&normalMatrix);
            CheckError();
        }

        private static unsafe void SetUpObjects()
        {

            float[] face1Color = [1f, 1f, 0f, 1.0f];
            float[] face2Color = [0.0f, 1.0f, 0.0f, 1.0f];
            float[] face3Color = [0.0f, 0.0f, 1.0f, 1.0f];
            float[] face4Color = [1.0f, 0.0f, 1.0f, 1.0f];
            float[] face5Color = [0.0f, 1.0f, 1.0f, 1.0f];
            float[] face6Color = [0.0f, 0.0f, 1.0f, 1.0f];

            string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "redball.fbx");
            string texturePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "redball.jpg");

            redball = FbxResourceReaderTextured.CreateTexturedRedBall(Gl, modelPath, texturePath);

            float[] tableColor = [System.Drawing.Color.Azure.R/256f,
                                  System.Drawing.Color.Azure.G/256f,
                                  System.Drawing.Color.Azure.B/256f,
                                  1f];
            table = GlCube.CreateSquare(Gl, tableColor);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "bomb.fbx");
            enemy = FbxResourceReader.CreateRedBallFromFbx(Gl, path, face6Color);
            path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "plant.fbx");
            plant =FbxResourceReader.CreateRedBallFromFbx(Gl, path, face1Color);

            skyBox = GlCube.CreateInteriorCube(Gl, "");
        }
        private static void Window_Closing()
        {
            redball.ReleaseGlObject();
            table.ReleaseGlObject();    
            skyBox.ReleaseGlObject();
            enemy.ReleaseGlObject();
        }

        private static unsafe void SetProjectionMatrix()
        {
            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)Math.PI / 4f, 1024f / 768f, 0.1f, 1000);
            int location = Gl.GetUniformLocation(program, ProjectionMatrixVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&projectionMatrix);
            CheckError();
        }

        private static unsafe void SetViewMatrix()
        {
            var viewMatrix = Matrix4X4.CreateLookAt(cameraDescriptor.Position, cameraDescriptor.Target, cameraDescriptor.UpVector);
            int location = Gl.GetUniformLocation(program, ViewMatrixVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&viewMatrix);
            CheckError();
        }

        public static void CheckError()
        {
            var error = (ErrorCode)Gl.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }
}
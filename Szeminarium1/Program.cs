using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Szeminarium1
{
    internal static class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static uint program;

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;

		out vec4 outCol;
        
        void main()
        {
			outCol = vCol;
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";


        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
		
		in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
        ";

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "1. szeminárium - háromszög";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Load()
        {
            // egszeri beallitasokat
            //Console.WriteLine("Loaded");

            Gl = graphicWindow.CreateOpenGL();

            Gl.ClearColor(System.Drawing.Color.White);

            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

       

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(fshader);
            Gl.GetShader(fshader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + Gl.GetShaderInfoLog(fshader));



            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }

        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO GL
            // make it threadsave
            //Console.WriteLine($"Update after {deltaTime} [s]");
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            //gl
            //Console.WriteLine($"Render after {deltaTime} [s]");

            Gl.Clear(ClearBufferMask.ColorBufferBit);

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            float[] vertexArray = new float[] {   
                //2
                -0.33f,  -0.17f, 0.0f,  // jobb fent 0
                -0.33f,  0.16f, 0.0f, // jobb lent 1
                -0.66f, 0.32f, 0.0f, // bal fent 2
                -0.66f, -0.01f, 0.0f, //bal lent 3
                //3
                0.0f,  0.0f, 0.0f,  // jobb fent 4
                0.0f,  -0.33f, 0.0f, // jobb lent 5 
                -0.33f, 0.16f, 0.0f, // bal fent 6
                -0.33f, -0.17f, 0.0f,//bal lent 7
                //10
                0.0f,  0.0f, 0.0f,  // jobb fent 8
                0.0f,  -0.33f, 0.0f, // jobb lent 9 
                0.33f, 0.16f, 0.0f, // bal fent 10
                0.33f, -0.17f, 0.0f,//bal lent 11
                //6
                0.0f,  -0.33f, 0.0f,  // jobb fent 12
                0.0f,  -0.66f, 0.0f, // jobb lent 13
                -0.33f, -0.5f, 0.0f, // bal fent 14
                -0.33f, -0.17f, 0.0f,//bal lent 15
                //13
                0.0f,  -0.33f, 0.0f,  // jobb fent 16
                0.0f,  -0.66f, 0.0f, // jobb lent 17 
                0.33f, -0.5f, 0.0f, // bal fent 18
                0.33f, -0.17f, 0.0f,//bal lent 19
                //9
                0.0f,  -0.66f, 0.0f,  // jobb fent 20   
                0.0f,  -1.0f, 0.0f, // jobb lent 21 
                -0.33f, -0.5f, 0.0f, // bal fent 22
                -0.33f, -0.83f, 0.0f,//bal lent 23
                //16
                0.0f,  -0.66f, 0.0f,  // jobb fent 24
                0.0f,  -1.0f, 0.0f, // jobb lent 25 
                0.33f, -0.5f, 0.0f, // bal fent 26
                0.33f, -0.83f, 0.0f,//bal lent 27

                //11
                0.33f,  -0.17f, 0.0f,  // jobb fent 28
                0.33f,  0.16f, 0.0f, // jobb lent 29
                0.66f, 0.32f, 0.0f, // bal fent 30
                0.66f, -0.01f, 0.0f, //bal lent 31

                //5
                -0.33f, -0.17f, 0.0f,  // jobb fent 32
                -0.33f, -0.5f, 0.0f, // jobb lent 33
                -0.66f, -0.34f, 0.0f, // bal fent 34
                -0.66f, -0.01f, 0.0f, //bal lent 35

                //14
                0.33f,  -0.17f, 0.0f,  // jobb fent 36
                0.33f,  -0.5f, 0.0f, // jobb lent 37
                0.66f, -0.01f, 0.0f, // bal fent 38
                0.66f, -0.34f, 0.0f, //bal lent 39

                //8
                -0.33f,  -0.5f, 0.0f,  // jobb fent 40
                -0.33f,  -0.83f, 0.0f, // jobb lent 41
                -0.66f, -0.34f, 0.0f, // bal fent 42
                -0.66f, -0.67f, 0.0f, //bal lent 43

                //17
                0.33f,  -0.5f, 0.0f,  // jobb fent 44
                0.33f,  -0.83f, 0.0f, // jobb lent 45
                0.66f, -0.34f, 0.0f, // bal fent 46
                0.66f, -0.67f, 0.0f, //bal lent 47

                //1
                -0.99f,  0.5f, 0.0f,  // jobb fent 48
                -0.99f,  0.17f, 0.0f, // jobb lent 49
                -0.66f, 0.32f, 0.0f, // bal fent 50
                -0.66f, -0.01f, 0.0f, //bal lent 51

                //12
                0.99f,  0.5f, 0.0f,  // jobb fent 52
                0.99f,  0.17f, 0.0f, // jobb lent 53
                0.66f, 0.32f, 0.0f, // bal fent 54
                0.66f, -0.01f, 0.0f, //bal lent 55

                //4
                -0.99f,  -0.16f, 0.0f,  // jobb fent 56
                -0.99f,  0.17f, 0.0f, // jobb lent 57
                -0.66f, -0.34f, 0.0f, // bal fent 58
                -0.66f, -0.01f, 0.0f, //bal lent 59

                //15
                0.99f,  -0.16f, 0.0f,  // jobb fent 60
                0.99f,  0.17f, 0.0f, // jobb lent 61
                0.66f, -0.34f, 0.0f, // bal fent 62
                0.66f, -0.01f, 0.0f, //bal lent 63

                //7
                -0.99f,  -0.16f, 0.0f,  // jobb fent 64
                -0.99f,  -0.49f, 0.0f, // jobb lent 65
                -0.66f, -0.34f, 0.0f, // bal fent 66
                -0.66f, -0.67f, 0.0f, //bal lent 67

                //18
                0.99f,  -0.16f, 0.0f,  // jobb fent 68
                0.99f,  -0.49f, 0.0f, // jobb lent 69
                0.66f, -0.34f, 0.0f, // bal fent 70
                0.66f, -0.67f, 0.0f, //bal lent 71

                //25
                0.0f,  0.0f, 0.0f,  // jobb fent 72
                0.0f,  0.33f, 0.0f, // jobb lent 73
                0.33f, 0.16f, 0.0f, // bal fent 74
                -0.33f, 0.16f, 0.0f, //bal lent 75

                //22
                -0.66f,  0.32f, 0.0f,  // jobb fent 76
                0.0f,  0.33f, 0.0f, // jobb lent 77
                -0.33f, 0.16f, 0.0f, // bal fent 78
                -0.33f, 0.49f, 0.0f, //bal lent 79
        
                //26
                0.66f,  0.32f, 0.0f,  // jobb fent 80
                0.0f,  0.33f, 0.0f, // jobb lent 81
                0.33f, 0.16f, 0.0f, // bal fent 82
                0.33f, 0.49f, 0.0f, //bal lent 83

                //19
                -0.66f,  0.32f, 0.0f,  // jobb fent 84
                -0.99f,  0.5f, 0.0f, // jobb lent 85
                -0.66f, 0.65f, 0.0f, // bal fent 86
                -0.33f, 0.49f, 0.0f, //bal lent 87

                //23
                0.0f,  0.66f, 0.0f,  // jobb fent 88
                0.0f,  0.33f, 0.0f, // jobb lent 89
                -0.33f, 0.49f, 0.0f, // bal fent 90
                0.33f, 0.49f, 0.0f, //bal lent 91

                //27
                0.66f,  0.32f, 0.0f,  // jobb fent 92
                0.99f,  0.5f, 0.0f, // jobb lent 93
                0.66f, 0.65f, 0.0f, // bal fent 94
                0.33f, 0.49f, 0.0f, //bal lent 95

                //20
                -0.66f,  0.65f, 0.0f,  // jobb fent 96
                0.0f,  0.66f, 0.0f, // jobb lent 97
                -0.33f, 0.81f, 0.0f, // bal fent 98
                -0.33f, 0.49f, 0.0f, //bal lent 99

                //24
                0.66f,  0.65f, 0.0f,  // jobb fent 100
                0.0f,  0.66f, 0.0f, // jobb lent 101
                0.33f, 0.81f, 0.0f, // bal fent 102
                0.33f, 0.49f, 0.0f, //bal lent 103

                //21
                0.0f,  0.66f, 0.0f,  // jobb fent 104
                0.0f,  0.99f, 0.0f, // jobb lent 105
                0.33f, 0.81f, 0.0f, // bal fent 106
                -0.33f, 0.81f, 0.0f, //bal lent 107
            };

            float[] colorArray = new float[] {
                //(blue)2
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
        
                //(Green)3
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
        
                //(Blue)10
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                //(blue)6
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
        
                //(green)13
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,

                //(Green)9
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
        
                //(Blue)16
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                //(green)11
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,

                //(Green)5
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
        
                //(Blue)14
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                //(blue)8
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
        
                //(green)17
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,

                //(Green)1
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
        
                //(Blue)12
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                 //(blue)4
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
        
                //(green)15
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,

                //(Green)7
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
        
                //(Blue)18
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                //(Red)25
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                //(yellow)22
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,

                //(yellow)26
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,

                //(Red)19
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                //(Red)23
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                //(Red)27
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                //(yellow)20
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,

                //(yellow)24
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,

                //(Red)21
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

            };

            uint[] indexArray = new uint[] {
                //2
                1, 2, 3, 
                3, 0, 1,   
                //3
                4, 6, 5,  
                6, 7, 5,   
                //10
                8, 10, 9,
                10, 11, 9,  
                //6
                12,15,14,
                14,13,12,
                //13
                16,17,18,
                18,19,16,
                //9
                20,22,23,
                23,21,20,
                //16
                24,25,27,
                27,26,24,
                //11
                29,28,31,
                31,30,29,
                //5
                32,35,34,
                34,33,32,
                //14
                38,36,37,
                37,39,38,
                //8
                40,42,43,
                43,41,40,
                //17
                46,44,45,
                45,47,46,
                //1
                50,48,49,
                49,51,50,
                //12
                54,55,53,
                53,52,54,
                //4
                59,57,56,
                56,58,59,
                //15
                63,62,60,
                60,61,63,
                //7
                66,64,65,
                65,67,66,
                //18
                70,71,69,
                69,68,70,
                //25
                73,75,72,
                72,74,73,
                //22
                79,76,78,
                78,77,79,
                //26
                83,81,82,
                82,80,83,
                //19
                86,85,84,
                84,87,86,
                //23
                88,90,89,
                89,91,88,
                //27
                94,95,92,
                92,93,94,
                //20
                98,96,99,
                99,97,98,
                //24
                102,101,103,
                103,100,102,
                //21
                105,107,104,
                104,106,105,
            };

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);
            //Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            var error1 = Gl.GetError();
            if (error1 != GLEnum.NoError)
            {
                Console.WriteLine($"OpenGL Error ({"vertex atribute setup"}): {error1}");
            }

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);
            //Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            var error2 = Gl.GetError();
            if (error2 != GLEnum.NoError)
            {
                Console.WriteLine($"OpenGL Error ({"color attribute setup"}): {error2}");
            }

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);
            //Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            var error3 = Gl.GetError();
            if (error3 != GLEnum.NoError)
            {
                Console.WriteLine($"OpenGL Error ({"index buffer setup"}): {error3}");
            }

            Gl.UseProgram(program);
            var error = Gl.GetError();
            if (error != GLEnum.NoError)
            {
                Console.WriteLine($"OpenGL Error ({"using shader program"}): {error}");
            }

            Gl.DrawElements(GLEnum.Triangles, (uint)indexArray.Length, GLEnum.UnsignedInt, null); // we used element buffer
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            Gl.BindVertexArray(vao);

            // always unbound the vertex buffer first, so no halfway results are displayed by accident
            Gl.DeleteBuffer(vertices);
            Gl.DeleteBuffer(colors);
            Gl.DeleteBuffer(indices);
            Gl.DeleteVertexArray(vao);
        }
    }
}

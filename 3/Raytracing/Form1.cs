using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Runtime.InteropServices;

namespace Graph_Lab3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int RayTracingProgramID;
        int RayTracingVertexShader;
        int RayTracingFragmentShader;

        private bool InitShaders()
        {
            RayTracingProgramID = GL.CreateProgram();
            List<String> vertexShaderText = new List<string>();
            vertexShaderText.Add("..\\..\\Shaders\\raytracing.vert");
            List<String> fragmentShaderText = new List<string>();
            fragmentShaderText.Add("..\\..\\Shaders\\raytracing.frag");
            loadShader(vertexShaderText, ShaderType.VertexShader, RayTracingProgramID, out RayTracingVertexShader);
            loadShader(fragmentShaderText, ShaderType.FragmentShader, RayTracingProgramID, out RayTracingFragmentShader);
            GL.LinkProgram(RayTracingProgramID);
            Console.WriteLine(GL.GetProgramInfoLog(RayTracingProgramID));
            GL.Enable(EnableCap.Texture2D);
            return true;
        }

        void loadShader(List<String> filenames, ShaderType type, int program, out int address)
        {

            address = GL.CreateShader(type);
            String shaderText = "";
            for (int i = 0; i < filenames.Count; i++)
            {
                System.IO.StreamReader sr = new StreamReader(filenames[i]);
                shaderText += sr.ReadToEnd();
            }
            GL.ShaderSource(address, shaderText);
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        private static bool Init()
        {
            GL.Enable(EnableCap.ColorMaterial);
            GL.ShadeModel(ShadingModel.Smooth);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            return true;
        }

        private static void Resize(int width, int height)
        {
            if (height == 0)
            {
                height = 1;
            }
            GL.Viewport(0, 0, width, height);
        }

        private void fillTriangleArrays(Vector4[] points, Vector4[] indexes)
        {
            
            points[0].X = -5; points[0].Y = 5; points[0].Z = -8; points[0].W = 0;
            points[1].X = 5; points[1].Y = 5; points[1].Z = -8; points[1].W = 0;
            points[2].X = 5; points[2].Y = -5; points[2].Z = -8; points[2].W = 0;
            points[3].X = -5; points[3].Y = -5; points[3].Z = -8; points[3].W = 0;

            indexes[0].X = 3; indexes[0].Y = 0; indexes[0].Z = 1; indexes[0].W = 4;
            indexes[1].X = 3; indexes[1].Y = 1; indexes[1].Z = 2; indexes[1].W = 4;

            
            points[4].X = -5; points[4].Y = 5; points[4].Z = 8; points[4].W = 0;
            points[5].X = 5; points[5].Y = 5; points[5].Z = 8; points[5].W = 0;
            points[6].X = 5; points[6].Y = -5; points[6].Z = 8; points[6].W = 0;
            points[7].X = -5; points[7].Y = -5; points[7].Z = 8; points[7].W = 0;

            indexes[2].X = 4; indexes[2].Y = 7; indexes[2].Z = 5; indexes[2].W = 5;
            indexes[3].X = 5; indexes[3].Y = 7; indexes[3].Z = 6; indexes[3].W = 5;

            
            indexes[4].X = 2; indexes[4].Y = 1; indexes[4].Z = 5; indexes[4].W = 0;
            indexes[5].X = 2; indexes[5].Y = 5; indexes[5].Z = 6; indexes[5].W = 0;

            
            indexes[6].X = 4; indexes[6].Y = 0; indexes[6].Z = 3; indexes[6].W = 3;
            indexes[7].X = 4; indexes[7].Y = 3; indexes[7].Z = 7; indexes[7].W = 3;

            
            indexes[8].X = 3; indexes[8].Y = 2; indexes[8].Z = 6; indexes[8].W = 4;
            indexes[9].X = 3; indexes[9].Y = 6; indexes[9].Z = 7; indexes[9].W = 4;

            // top wall
            indexes[10].X = 0; indexes[10].Y = 4; indexes[10].Z = 5; indexes[10].W = 4;
            indexes[11].X = 0; indexes[11].Y = 5; indexes[11].Z = 1; indexes[11].W = 4;

            points[8].X = -1; points[8].Y =  0; points[8].Z =  15; points[8].W =  0;
            points[9].X =  0; points[9].Y =  2; points[9].Z =  15; points[9].W =  0;
            points[10].X = 0; points[10].Y = 0; points[10].Z = 15; points[10].W = 0;

            indexes[12].X = 8; indexes[12].Y = 10; indexes[12].Z = 9; indexes[12].W = 6;
        }

        private void setVec4BufferAsImage(Vector4[] array, BufferUsageHint bufferUsageHint, int unit)
        {
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
            int buf = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.TextureBuffer, buf);
            GL.BufferData(BufferTarget.TextureBuffer, (IntPtr)(sizeof(float) * 4 * array.Length), ptr, BufferUsageHint.StaticDraw);

            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureBuffer, tex);
            GL.TexBuffer(TextureBufferTarget.TextureBuffer, SizedInternalFormat.Rgba32f, buf);
            GL.BindImageTexture(unit, tex, 0, false, 0, TextureAccess.ReadOnly, SizedInternalFormat.Rgba32f);
        }

        private void initSceneBuffers()
        {
            Vector4[] points = new Vector4[11];
            Vector4[] indexes = new Vector4[13];
            fillTriangleArrays(points, indexes);
            Vector4[] spheres = new Vector4[2];
            spheres[0].X = -1; spheres[0].Y = -1; spheres[0].Z = -2; spheres[0].W = 2;
            spheres[1].X = 2; spheres[1].Y = 1; spheres[1].Z = 2; spheres[1].W = 1;

            setVec4BufferAsImage(points,  BufferUsageHint.StaticDraw, 2);
            setVec4BufferAsImage(indexes, BufferUsageHint.StaticDraw, 3);
            setVec4BufferAsImage(spheres, BufferUsageHint.StaticDraw, 4);
        }

        public const int DIFFUSE_REFLECTION = 1;
        public const int MIRROR_REFLECTION = 2;
        public const int REFRACTION = 3;

        Material[] fillMaterials()
        {
            Material[] materials = new Material[7];
            Vector4 lightCoefs = new Vector4(0.4f, 0.9f, 0.2f, 2.0f);
            materials[0] = new Material(new Vector3(0, 1, 0), lightCoefs, 0.5f, 1, DIFFUSE_REFLECTION);
            materials[1] = new Material(new Vector3(0, 0, 1), lightCoefs, 0.5f, 1, DIFFUSE_REFLECTION);
            materials[2] = new Material(new Vector3(0, 0.1f, 0.8f), lightCoefs, 0.5f, 1, DIFFUSE_REFLECTION);
            materials[3] = new Material(new Vector3(1, 0, 0), lightCoefs, 0.5f, 1, DIFFUSE_REFLECTION);
            materials[4] = new Material(new Vector3(1, 1, 1), lightCoefs, 0.5f, 1, DIFFUSE_REFLECTION);
            materials[5] = new Material(new Vector3(0, 1, 1), lightCoefs, 0.5f, 1, DIFFUSE_REFLECTION);
            materials[6] = new Material(new Vector3(0, 1, 1), new Vector4(0.4f, 0.9f, 0.9f, 50.0f), 0.8f, 1.5f, REFRACTION);
            return materials;
        }

        void initMaterials()
        {
            Material[] materials = fillMaterials();
            int location;
            for (int i = 0; i < materials.Length; i++)
            {
                location = GL.GetUniformLocation(RayTracingProgramID, "uMaterials[" + i + "].Color");
                GL.Uniform3(location, materials[i].Color);
                location = GL.GetUniformLocation(RayTracingProgramID, "uMaterials[" + i + "].LightCoeffs");
                GL.Uniform4(location, materials[i].LightCoeffs);
                location = GL.GetUniformLocation(RayTracingProgramID, "uMaterials[" + i + "].ReflectionCoef");
                GL.Uniform1(location, materials[i].ReflectionCoef);
                location = GL.GetUniformLocation(RayTracingProgramID, "uMaterials[" + i + "].RefractionCoef");
                GL.Uniform1(location, materials[i].RefractionCoef);
                location = GL.GetUniformLocation(RayTracingProgramID, "uMaterials[" + i + "].MaterialType");
                GL.Uniform1(location, materials[i].MaterialType);
            }

        }
        private void Draw()
        {
            GL.ClearColor(Color.AliceBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
;
            GL.UseProgram(RayTracingProgramID);
            initMaterials();
            initSceneBuffers();
            // Camera
            int location = GL.GetUniformLocation(RayTracingProgramID, "uCamera.Position");
            GL.Uniform3(location, new Vector3(0, 0, -7.5f));
            location = GL.GetUniformLocation(RayTracingProgramID, "uCamera.Up");
            GL.Uniform3(location, Vector3.UnitY);
            location = GL.GetUniformLocation(RayTracingProgramID, "uCamera.Side");
            GL.Uniform3(location, Vector3.UnitX);
            location = GL.GetUniformLocation(RayTracingProgramID, "uCamera.View");
            GL.Uniform3(location, Vector3.UnitZ);
            location = GL.GetUniformLocation(RayTracingProgramID, "uCamera.Scale");
            GL.Uniform2(location, new Vector2(1, (float)openGlControl.Height / openGlControl.Width));
            // Light
            location = GL.GetUniformLocation(RayTracingProgramID, "uLight.Position");
            GL.Uniform3(location, new Vector3(2.0f, 0.0f, -4.0f));

            // Quad
            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0, 1);
            GL.Vertex2(-1, -1);

            GL.TexCoord2(1, 1);
            GL.Vertex2(1, -1);

            GL.TexCoord2(1, 0);
            GL.Vertex2(1, 1);

            GL.TexCoord2(0, 0);
            GL.Vertex2(-1, 1);

            GL.End();
            openGlControl.SwapBuffers();
            GL.UseProgram(0);

        }

        private void openGlControl_Paint(object sender, PaintEventArgs e)
        {
            Draw();
        }

        private void openGlControl_Load(object sender, EventArgs e)
        {
            Init();
            Resize(openGlControl.Width, openGlControl.Height);
            InitShaders();
        }


        private void openGlControl_Resize(object sender, EventArgs e)
        {
            Resize(openGlControl.Width, openGlControl.Height);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            openGlControl.Width = this.ClientRectangle.Width - 24;
            openGlControl.Height = this.ClientRectangle.Height - 24;
        }

    } 

    public struct Material
    {
        public Vector3 Color;
        public Vector4 LightCoeffs;
        public float ReflectionCoef;
        public float RefractionCoef;
        public int MaterialType;
        public Material(Vector3 color, Vector4 lightCoefs, float reflectionCoef, float refractionCoef, int type)
        {
            Color = color;
            LightCoeffs = lightCoefs;
            ReflectionCoef = reflectionCoef;
            RefractionCoef = refractionCoef;
            MaterialType = type;
        }
    };

}

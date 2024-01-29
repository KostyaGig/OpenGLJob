using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Program;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Timers;
using static Program.MainRenderWindow;
using static System.Net.Mime.MediaTypeNames;
namespace Tests
{

    internal static class Program
    {
        private static void Main()
        {
            var game = new Game(1000, 1000, "Test", 60.0);
            game.Run();
        }
    }

    public class Game : MainRenderWindow
    {

        private readonly int width;
        private readonly int height;
        
        private const int ROTATION_RATE_IN_SECONDS = 2;
        private string timerText = "Seconds elapsed: 0";
        
        private int cubeIndex;
        private int cylinderIndex;
        private int spehereIndex;
        private int coneIndex;

        private int rotationY = 0;
        private int secondsElapsed;

        // map object index to its properties
        private readonly Dictionary<int, Vector3> positionMap = new Dictionary<int, Vector3>();
        private readonly Dictionary<int, float> scaleMap = new Dictionary<int, float>();
        private readonly List<int> objectIndexes = new List<int>();
        private readonly List<string> materials = new List<string>();

        public Game(int width, int height, string title, double FPS) : base(width, height, title, FPS)
        {
            this.width = width;
            this.height = height;
        }

        protected override void OnLoad()
        {
            Console.WriteLine("OnLoad thread " + Thread.CurrentThread.IsBackground);
            InitializeTimer();
            SetClearColor(new Color4(1.0f, 1.0f, 1.0f, 1.0f));
            UseDepthTest = true;
            UseAlpha = false;
            KeyboardAndMouseInput = true;
            base.OnLoad();
            CreateMainLight(new Vector3(0.0f, 0.0f, 0.5f), new Vector3(255.0f, 104.0f, 104.0f));

            materials.Add("C:\\Users\\kostiggig\\Desktop\\OpenTK-3D-Engine-master\\Tests\\metal.png");
            materials.Add("C:\\Users\\kostiggig\\Desktop\\OpenTK-3D-Engine-master\\Tests\\blue_glass.png");
            materials.Add("C:\\Users\\kostiggig\\Desktop\\OpenTK-3D-Engine-master\\Tests\\dirt.png");
            materials.Add("C:\\Users\\kostiggig\\Desktop\\OpenTK-3D-Engine-master\\Tests\\brick.png");

            Shuffle(materials);

            cubeIndex = OpenTexturedObj("C:\\Users\\kostiggig\\Desktop\\OpenTK-3D-Engine-master\\Tests\\cutted_cube.obj", materials[_mainTexturedObjects.Count]);
            cylinderIndex = OpenTexturedObj("C:\\Users\\kostiggig\\Desktop\\OpenTK-3D-Engine-master\\Tests\\cutted_cylinder.obj", materials[_mainTexturedObjects.Count]);
            spehereIndex = OpenTexturedObj("C:\\Users\\kostiggig\\Desktop\\OpenTK-3D-Engine-master\\Tests\\cutted_sphere2.obj", materials[_mainTexturedObjects.Count]);
            coneIndex = OpenTexturedObj("C:\\Users\\kostiggig\\Desktop\\OpenTK-3D-Engine-master\\Tests\\cutted_cone2.obj", materials[_mainTexturedObjects.Count]);

            objectIndexes.Add(cubeIndex);
            objectIndexes.Add(cylinderIndex);
            objectIndexes.Add(spehereIndex);
            objectIndexes.Add(coneIndex);

            positionMap.Add(cubeIndex, new Vector3(0.0f, 0.0f, 0.0f));
            scaleMap.Add(cubeIndex, 10.0f);

            positionMap.Add(cylinderIndex, new Vector3(40.0f, -10.0f, 0.0f));
            scaleMap.Add(cylinderIndex, 10.0f);

            positionMap.Add(spehereIndex, new Vector3(80.0f, -10.0f, 0.0f));
            scaleMap.Add(spehereIndex, 10.0f);

            positionMap.Add(coneIndex, new Vector3(130.0f, -10.0f, 10.0f));
            scaleMap.Add(coneIndex, 10.0f);

            foreach (int objectIndex in objectIndexes)
            {
                _mainTexturedObjects[objectIndex].SetPositionInSpace(positionMap[objectIndex]);
                _mainTexturedObjects[objectIndex].SetScale(scaleMap[objectIndex]);
            }
            /*_mainTexturedObjects[cubeIndex].SetPositionInSpace(new Vector3(0.0f, 0.0f, 0.0f));
            _mainTexturedObjects[cubeIndex].SetScale(10.0f);
            
            _mainTexturedObjects[textureObjIndex2].SetPositionInSpace(new Vector3(40.0f, -10.0f, 0.0f));
            _mainTexturedObjects[textureObjIndex2].SetScale(10.0f);

            _mainTexturedObjects[textureObjIndex3].SetPositionInSpace(new Vector3(80.0f, -10.0f, 0.0f));
            _mainTexturedObjects[textureObjIndex3].SetScale(10.0f);

            _mainTexturedObjects[textureObjIndex4].SetPositionInSpace(new Vector3(130.0f, -10.0f, 0.0f));
            _mainTexturedObjects[textureObjIndex4].SetScale(10.0f);*/

            _camera.Position = new Vector3((-5.0f, 16.0f, 39.0f));
        }

       


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Clear();
            var tbmt = new Bitmap(1, 1);
            var tgrx = Graphics.FromImage(tbmt);
            Font font = new Font("Microsoft Sans Serif", 20.0f, FontStyle.Bold);

            var fontSize = tgrx.MeasureString(timerText, font);
            DrawText(timerText, (float) width - fontSize.Width, (float) height - fontSize.Height, font, Color4.Yellow);

            if (++rotationY % (ROTATION_RATE_IN_SECONDS * 60) == 0) 
            {
                foreach (int objectIndex in objectIndexes)
                {
                    _mainTexturedObjects[objectIndex].SetRotationY((float) rotationY);
                }
            }
            
            Render3DObjects();
            base.OnRenderFrame(e);
           
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = $"Test app, FPS: {1/ e.Time}";
            base.OnUpdateFrame(e);
        }

       

        private void InitializeTimer()
        {
            Thread thread1 = new Thread(() =>
            {
                System.Timers.Timer timer = new System.Timers.Timer(1000);
                timer.Elapsed += OnTimedEvent;
                timer.Start();
            });
            thread1.Start();
           
        }

        void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            secondsElapsed++;
            timerText = "Seconds elapsed: " + secondsElapsed;
        }

        private readonly Random rng = new Random();

        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
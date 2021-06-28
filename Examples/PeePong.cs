using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using ConsoleEngine;
using ABSoftware;

namespace ConsoleEngine2D
{
    public class PeePong : Engine
    {
        static void Main(string[] args)
        {
            PeePong p = new PeePong();
            p.Construct(60, 30, 30);
            p.Start();
        }

        public const string CONFIG_PATH = "PongConfig.klin";
        Timer StartTimer = new Timer(1500);
        Timer controlsTimer100 = new Timer(100);
        Timer controlsTimer10 = new Timer(10);
        Timer controlsTimer1 = new Timer(1);
        int centerX { get { return ScreenWidth / 2; } }
        int centerY { get { return ScreenHeight / 2; } }

        public int P1Score = 0;
        public int P2Score = 0;

        public Point P1Platform;
        public Point P2Platform;

        public Point BallPosition;
        public int BallVelocityX;
        public int BallVelocityY;

        public int PlatformWidth;
        public int PlatformHeight;

        public int BallWidth = 2;
        public int BallHeight = 1;

        Pixel BallColor = Pixel.Gray;
        Pixel PlatformColor = Pixel.White;

        public bool Paused = true;

        public override void OnCreate()
        {
            InitPlatform();
            LoadConfig();
            P1Platform = new Point(2, centerY - PlatformHeight / 2);
            P2Platform = new Point(ScreenWidth - PlatformWidth - 2, centerY - PlatformHeight / 2);
            BallPosition = new Point(ScreenWidth / 2, ScreenHeight / 2);
            Resize(ScreenWidth + 2, ScreenHeight + 2);
        }

        public override void OnUpdate()
        {
            Clear(Pixel.Black);
            glyphRenderer.Clear();
            ControlsChecker();
            DrawRect(0, 0, ScreenWidth, ScreenHeight, getRainbow(1f, 1f, 1f));

            if (!Paused)
            {
                BallPosition.x += BallVelocityX;
                BallPosition.y += BallVelocityY;

                CheckP1Collision();
                CheckP2Collision();
                PerformBallActions();

                FillRect(P1Platform.x, P1Platform.y, PlatformWidth, PlatformHeight, PlatformColor);
                FillRect(P2Platform.x, P2Platform.y, PlatformWidth, PlatformHeight, PlatformColor);
                FillRect(BallPosition.x, BallPosition.y, BallWidth, BallHeight, BallColor);

                glyphRenderer.DrawText(centerX / 2 - P1Score.ToString().Length / 2, 3, $"{P1Score}", Pixel.Cyan); //Player 1 score display
                glyphRenderer.DrawText(centerX + centerX / 2 - P2Score.ToString().Length / 2, 3, $"{P2Score}", Pixel.Cyan); //Player 2 score display

                if(BallVelocityX == 0 && BallVelocityY == 0)
                {
                    if(StartTimer.Tick())
                    {
                        StartBall();
                    }
                }
            }
            else
            {
                string pausedText = "PAUSED! PRESS ENTER TO CONTINUE!";
                glyphRenderer.DrawText(centerX - pausedText.Length / 2, centerY, pausedText, Pixel.Red);
            }
        }

        public void StartBall()
        {
            int dirx = Randomizer.RandomInt(0, 1);
            int diry = Randomizer.RandomInt(0, 1);
            if (dirx == 0)
                BallVelocityX = -1;
            else if (dirx == 1)
                BallVelocityX = 1;

            if (diry == 0)
                BallVelocityY = -1;
            else if (diry == 1)
                BallVelocityY = 1;
        }

        public void OnPauseChange()
        {
            StartTimer.Init();
        }

        public void PerformBallActions()
        {
            bool right = false;
            bool xcol = false;
            bool ycol = false;
            bool platform = false;
            CheckBallCollision(ref xcol, ref ycol, ref right, ref platform);

            if(platform)
            {
                BallVelocityX = -BallVelocityX;
            }

            if(xcol)
            {
                if(right)
                {
                    P1Score++;
                    BallPosition.x = centerX;
                    BallPosition.y = centerY;
                    BallVelocityX = 0;
                    BallVelocityY = 0;
                    StartTimer.Init();
                }
                else
                {
                    P2Score++;
                    BallPosition.x = centerX;
                    BallPosition.y = centerY;
                    BallVelocityX = 0;
                    BallVelocityY = 0;
                    StartTimer.Init();
                }
            }

            if(ycol)
            {
                BallVelocityY = -BallVelocityY;
            }
        }

        public void CheckP1Collision()
        {
            if (P1Platform.y <= 0)
            {
                P1Platform.y = 1;
            }
            else if(P1Platform.y + PlatformHeight >= ScreenHeight - 1)
            {
                P1Platform.y = ScreenHeight - PlatformHeight - 1;
            }
        }

        public void CheckP2Collision()
        {
            if (P2Platform.y <= 0)
            {
                P2Platform.y = 1;
            }
            else if(P2Platform.y + PlatformHeight >= ScreenHeight - 1)
            {
                P2Platform.y = ScreenHeight - PlatformHeight - 1;
            }
        }

        public void CheckBallCollision(ref bool collisionX, ref bool collisionY, ref bool right, ref bool platform)
        {
            //BALL
            if(BallPosition.x >= P1Platform.x - 1 && BallPosition.x <= P1Platform.x + PlatformWidth && BallPosition.y >= P1Platform.y && BallPosition.y <= P1Platform.y + PlatformHeight)
            {
                platform = true;
            }
            if (BallPosition.x + 1 >= P2Platform.x - 1 && BallPosition.x + 1 <= P2Platform.x + PlatformWidth && BallPosition.y >= P2Platform.y && BallPosition.y <= P2Platform.y + PlatformHeight)
            {
                platform = true;
            }

            if (BallPosition.x <= 1 || BallPosition.x + BallWidth >= ScreenWidth - 1)
            {
                collisionX = true;
                if (BallPosition.x + BallWidth >= ScreenWidth - 1)
                    right = true;
            }

            if (BallPosition.y <= 1 || BallPosition.y + BallHeight >= ScreenHeight - 1)
            {
                collisionY = true;
            }
        }

        public void ControlsChecker()
        {
            if(controlsTimer100.Tick())
            {
                if(Keyboard.GetAsyncKeyState(Keyboard.Keys.VK_RETURN) != 0 && isFocused)
                {
                    Paused = !Paused;
                    OnPauseChange();
                }
            }
            if(controlsTimer10.Tick())
            {
                if(!Paused)
                {
                    if (Keyboard.GetAsyncKeyState(Keyboard.Keys.W) != 0 && isFocused)
                    {
                        P1Platform.y -= 1;
                    }
                    if (Keyboard.GetAsyncKeyState(Keyboard.Keys.S) != 0 && isFocused)
                    {
                        P1Platform.y += 1;
                    }

                    if (Keyboard.GetAsyncKeyState(Keyboard.Keys.VK_UP) != 0 && isFocused)
                    {
                        P2Platform.y -= 1;
                    }
                    if (Keyboard.GetAsyncKeyState(Keyboard.Keys.VK_DOWN) != 0 && isFocused)
                    {
                        P2Platform.y += 1;
                    }
                }
            }
            if(controlsTimer1.Tick())
            {

            }
        }



        public void InitPlatform()
        {
            PlatformHeight = 5;
            PlatformWidth = 3;
        }

        public void LoadConfig()
        {
            if (File.Exists(CONFIG_PATH))
            {
                KLIN k = new KLIN();
                k.Parse(File.ReadAllText(CONFIG_PATH));
                //PLATFORM COLOR
                string pcValue = k.Get("PlatformColor").ToString();
                byte r = byte.Parse(pcValue.Split(',')[0]);
                byte g = byte.Parse(pcValue.Split(',')[1]);
                byte b = byte.Parse(pcValue.Split(',')[2]);
                PlatformColor = Pixel.FromRGB(r, g, b);
                //!PLATFORM COLOR

                //BALL COLOR
                string bcValue = k.Get("BallColor").ToString();
                byte rb = byte.Parse(bcValue.Split(',')[0]);
                byte gb = byte.Parse(bcValue.Split(',')[1]);
                byte bb = byte.Parse(bcValue.Split(',')[2]);
                BallColor = Pixel.FromRGB(rb, gb, bb);
                //!BALL COLOR
            }
            else
            {
                KLIN k = new KLIN();
                File.Create(CONFIG_PATH).Dispose();
                k.Add("PlatformColor", $"{PlatformColor.R},{PlatformColor.G},{PlatformColor.B}");
                k.Add("BallColor", $"{BallColor.R},{BallColor.G},{BallColor.B}");
                File.WriteAllText(CONFIG_PATH, k.ToString());
            }
        }



        public Pixel getRainbow(float sec, float sat, float bri)
        {
            float hue = ((GetCurrentTimeMillis()) % (int)(sec * 1000f)) / (float)(sec * 1000f);
            return Pixel.FromHSV(hue, sat, bri);
        }

        public Pixel getRainbow(float sec, float sat, float bri, long index)
        {
            float hue = ((GetCurrentTimeMillis() + index) % (int)(sec * 1000f)) / (float)(sec * 1000f);
            return Pixel.FromHSV(hue, sat, bri);
        }
    }
}

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
    public class Game3D : Engine
    {

        static void Main(string[] args)
        {
            Game3D p = new Game3D();
            p.Construct(120, 40, -1);
            p.Start();
        }

        string map;
        //PLAYER
        float pX = 8.0f, pY = 8.0f, pA = 0.0f;
        float fov = 3.14159f / 4.0f;
        float speedMultiplier = 0.5f;
        //MAP
        float depth = 16;
        int mWidth = 16, mHeight = 16;
        

        public void LoadMap(string mapString)
        {
            string[] Lines = mapString.Split('\n');
            for(int i = 0; i < Lines.Length; i++)
            {
                map += Lines[i];
            }
            mWidth = Lines[0].Length;
            mHeight = Lines.Length;
            depth = (float)Math.Max(mWidth, mHeight);
        }

        public void LoadPlayer(string klin)
        {
            KLIN k = new KLIN();
            k.Parse(klin);
            pX = Maths.FloatFromString(k.Get("playerX").ToString());
            pY = Maths.FloatFromString(k.Get("playerY").ToString());
            pA = Maths.FloatFromString(k.Get("playerAngle").ToString());
            fov = (float)Maths.DegreesToRadians(Maths.FloatFromString(k.Get("fov").ToString()));
            speedMultiplier = Maths.FloatFromString(k.Get("speedMultiplier").ToString());
        }

        public override void OnCreate()
        {
            AppName = $"{AppName} - Made by ABlair 'https://vk.com/ablair'";

            if (!File.Exists("readme.txt"))
            {
                File.Create("readme.txt").Dispose();
                File.WriteAllText("readme.txt", " ==СОЗДАНИЕ КАРТ== \n*'#' - Стена\n*' ' (Пробел) - Пустое место\n*Для постройки карты вам нужно открыть файл 'map.m2d', далее, вы должны составить карту из данных вам символов выше.\n*Пример карты приложен.\n*Внимание, следите за количеством символов в строке, они должны совпадать!\n ==КОММЕНТАРИЙ ABlair`a== \nВ данной демонстрации возможностей моего консольного движка отсутствует проверка коллизии, так что персонаж будет проходить сквозь стены.\n ==ИЗМЕНЕНИЕ ПАРАМЕТРОВ ИГРОКА==\n*На данном этапе все будет слегка посложнее, вам придется работать с технологией 'KLIN', каждый параметр назван своим именем, поэтому вам не будет так трудно найти нужный параметр и изменить его!\n*Среди всех параметров есть стартовая позиция, стартовый угол поворота и угол обзора.\n*УГОЛ ОБЗОРА - Укажите угол обзора персонажа в градусах. Например: 45.\n*СТАРТОВАЯ ПОЗИЦИЯ - Укажите X и Y координату расположенную на полотне карты для удачного спавна ;)");
            }
            if (!File.Exists("author.html"))
            {
                File.Create("author.html").Dispose();
                File.WriteAllText("author.html", "<head>\n<meta http-equiv=\"refresh\" content=\"0; url = http://vk.com/ablair\" />\n</head>");
            }

            if (!File.Exists("map.m2d"))
            {
                DrawString(0, 0, "Файл содержащий карту отсутствует, возможно, он был переименован.", Pixel.Red, ColorType.Foreground);
                DrawString(0, 1, "Для продолжения нажмите любую клавишу...", Pixel.White, ColorType.Foreground);
                Console.ReadKey();
                enabled = false;
                return;
            }
            else if (!File.Exists("config.klin"))
            {
                DrawString(0, 0, "Конфигурационный файл отсутствует, возможно, он был переименован.", Pixel.Red, ColorType.Foreground);
                DrawString(0, 1, "Для продолжения нажмите любую клавишу...", Pixel.White, ColorType.Foreground);
                Console.ReadKey();
                enabled = false;
                return;
            }
            else
            {
                LoadMap(File.ReadAllText("map.m2d"));
                LoadPlayer(File.ReadAllText("config.klin"));
            }
        }

        public override void OnUpdate()
        {
            try
            {
                if (Keyboard.GetAsyncKeyState(Keyboard.Keys.A) != 0 && isFocused)
                {
                    pA -= 0.1f;
                }
                if (Keyboard.GetAsyncKeyState(Keyboard.Keys.D) != 0 && isFocused)
                {
                    pA += 0.1f;
                }
                if (Keyboard.GetAsyncKeyState(Keyboard.Keys.W) != 0 && isFocused)
                {
                    pX += (float)Math.Sin(pA) * speedMultiplier;
                    pY += (float)Math.Cos(pA) * speedMultiplier;

                    if(map[(int)pY * mWidth + (int)pX] == '#')
                    {
                        pX -= (float)Math.Sin(pA) * speedMultiplier;
                        pY -= (float)Math.Cos(pA) * speedMultiplier;
                    }
                }
                if (Keyboard.GetAsyncKeyState(Keyboard.Keys.S) != 0 && isFocused)
                {
                    pX -= (float)Math.Sin(pA) * speedMultiplier;
                    pY -= (float)Math.Cos(pA) * speedMultiplier;

                    if (map[(int)pY * mWidth + (int)pX] == '#')
                    {
                        pX += (float)Math.Sin(pA) * speedMultiplier;
                        pY += (float)Math.Cos(pA) * speedMultiplier;
                    }
                }


                for (int x = 0; x < ScreenWidth; x++)
                {
                    float rayA = (pA - fov / 2.0f) + ((float)x / (float)ScreenWidth) * fov;
                    float dtw = 0f;

                    float eX = (float)Math.Sin(rayA), eY = (float)Math.Cos(rayA);

                    bool hw = false;
                    while (!hw && dtw < depth)
                    {
                        dtw += 0.1f;

                        int tx = (int)(pX + eX * dtw);
                        int ty = (int)(pY + eY * dtw);

                        if (tx < 0 || tx >= mWidth || ty < 0 || ty >= mHeight)
                        {
                            hw = true;
                            dtw = depth;
                        }
                        else
                        {
                            if (map[ty * mWidth + tx] == '#')
                            {
                                hw = true;
                            }
                        }
                    }

                    int ceiling = (int)((float)(ScreenHeight / 2.0f) - ScreenHeight / (dtw));
                    int floor = ScreenHeight - ceiling;

                    Pixel wshade;
                    if (dtw <= depth / 8.0f) wshade = Pixel.FromRGB(255, 255, 255);
                    else if (dtw < depth / 7.0f) wshade = Pixel.FromRGB(200, 200, 200);
                    else if (dtw < depth / 6.0f) wshade = Pixel.FromRGB(180, 180, 180);
                    else if (dtw < depth / 5.0f) wshade = Pixel.FromRGB(150, 150, 150);
                    else if (dtw < depth / 4.0f) wshade = Pixel.FromRGB(120, 120, 120);
                    else if (dtw < depth / 3.0f) wshade = Pixel.FromRGB(100, 100, 100);
                    else if (dtw < depth / 2.0f) wshade = Pixel.FromRGB(80, 80, 80);
                    else if (dtw < depth) wshade = Pixel.FromRGB(60, 60, 60);
                    else wshade = Pixel.Sky;

                    for (int y = 0; y < ScreenHeight; y++)
                    {
                        if (y < ceiling + 1)
                            Draw(x, y, Pixel.Sky);
                        else if (y > ceiling && y < floor)
                            Draw(x, y, wshade);
                        else
                        {
                            float b = 1.0f - (((float)y - ScreenHeight / 2.0f) / ((float)ScreenHeight / 2.0f));
                            if (b < 0.25f) Draw(x, y, Pixel.Dirt);
                            else if (b < 0.5f) Draw(x, y, Pixel.FromRGB(67, 46, 18));
                            else if (b < 0.75f) Draw(x, y, Pixel.FromRGB(50, 35, 14));
                            else if (b < 0.9f) Draw(x, y, Pixel.FromRGB(38, 27, 11));
                            else Draw(x, y, Pixel.Sky);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public override void OnDestroy()
        {
            
        }
    }
}

using System;
using ABSoftware;
using ConsoleEngine;

namespace ConsoleEngine2D
{
    public class Test : EngineChar
    {
        static void Main(string[] args)
        {
            Test t = new Test();
            t.ChangeFont(I_PIXEL_WIDTH, I_PIXEL_HEIGHT);
            t.Construct(100, 100, -1);
            t.Start();
        }

        public const int I_PIXEL_WIDTH = 8;
        public const int I_PIXEL_HEIGHT = 8;

        bool paused = false;
        bool[] cells;

        float delay = 0.2f; //Seconds
        int tickNumber = 0;

        public override void OnCreate()
        {
            QuickEditMode(false);
            Keyboard.OnKeyDown += Keyboard_OnKeyDown;
            Keyboard.OnKeyHeld += Keyboard_OnKeyHeld;
            Move(GetWindowRectangle().left, GetWindowRectangle().top, GetConsoleWidth() + I_PIXEL_WIDTH, GetConsoleHeight() + I_PIXEL_HEIGHT);
            cells = new bool[ScreenWidth * ScreenHeight];

            SetCell(1, 05, "                        #            ");
            SetCell(1, 06, "                      # #            ");
            SetCell(1, 07, "            ##      ##            ## ");
            SetCell(1, 08, "           #   #    ##            ## ");
            SetCell(1, 09, "##        #     #   ##               ");
            SetCell(1, 10, "##        #   # ##    # #            ");
            SetCell(1, 11, "          #     #       #            ");
            SetCell(1, 12, "           #   #                     ");
            SetCell(1, 13, "            ##                       ");
        }

        private void Keyboard_OnKeyHeld(string VK_Name, int VK_KEY)
        {
            if (!isFocused)
                return;
            if (VK_KEY.Equals(Keyboard.Keys.VK_LBUTTON))
            {
                Point cPos = CursorPos();
                SetCell(cPos.x, cPos.y, true);
            }
            if (VK_KEY.Equals(Keyboard.Keys.VK_RBUTTON))
            {
                Point cPos = CursorPos();
                SetCell(cPos.x, cPos.y, false);
            }
            if(VK_KEY.Equals(Keyboard.Keys.VK_UP))
            {
                delay += 0.001f;
            }
            if (VK_KEY.Equals(Keyboard.Keys.VK_DOWN))
            {
                if(delay >= 0.01f)
                    delay -= 0.001f;
            }
        }

        private void Keyboard_OnKeyDown(string VK_Name, int VK_KEY)
        {
            if (!isFocused)
                return;
            if (VK_KEY.Equals(Keyboard.Keys.VK_SPACE))
            {
                paused = !paused;
            }
            if(VK_KEY.Equals(Keyboard.Keys.C))
            {
                for(int i = 0; i < cells.Length; i++)
                {
                    cells[i] = false;
                }
            }
            if(VK_KEY.Equals(Keyboard.Keys.R))
            {
                delay = 0.2f;
            }
        }

        float time = 0f;

        public override void OnUpdate(float elapsedTime)
        {
            AppName = $"Game Of Life | Tick: {tickNumber} | Delay:{delay} {(paused ? "- Paused" : "")}";
            Keyboard.UpdateKeys();
            Clear(' ');
            for(int x = 0; x < ScreenWidth; x++)
            {
                for (int y = 0; y < ScreenHeight; y++)
                {
                    switch(GetCell(x,y))
                    {
                        case true:
                            {
                                Draw(x, y, '█');
                            }
                            break;
                        case false:
                            {
                                Draw(x, y, ' ');
                            }
                            break;
                    }
                }
            }
            if(time >= delay && !paused)
            {
                Tick();
                time = 0f;
            }
            time += elapsedTime;
        }

        void Tick()
        {
            bool[] newCells = new bool[ScreenWidth * ScreenHeight];
            for (int x = 0; x < ScreenWidth; x++)
            {
                for (int y = 0; y < ScreenHeight; y++)
                {
                    bool cellState = GetCell(x, y);
                    int count = CountOfTrue(GetNeighbours(x, y));
                    if(cellState) //White
                    {
                        if(count == 2 || count == 3)
                        {
                            newCells[y * ScreenWidth + x] = true;
                        }
                        else
                        {
                            newCells[y * ScreenWidth + x] = false;
                        }
                    }
                    else
                    {
                        if(count == 3)
                        {
                            newCells[y * ScreenWidth + x] = true;
                        }
                        else
                        {
                            newCells[y * ScreenWidth + x] = false;
                        }
                    }
                }
            }
            cells = newCells;
            tickNumber++;
        }

        public Point CursorPos()
        {
            Windows.RECT rect = GetWindowRectangle();
            Point p = GetCursor();
            int x = (p.x - rect.left - 8) / I_PIXEL_WIDTH;
            int y = (p.y - rect.top - 30) / I_PIXEL_HEIGHT;
            return new Point(x, y);
        }

        void SetCell(int x, int y, bool state)
        {
            if (x < 0 || y < 0 || x >= ScreenWidth || y >= ScreenHeight)
                return;
            cells[y * ScreenWidth + x] = state;
        }

        void SetCell(int x, int y, string line)
        {
            for(int i = 0; i < line.Length; i++)
            {
                SetCell(x + i, y, line[i]!=' ');
            }
        }

        bool[] GetNeighbours(int x, int y)
        {
            bool[] neighbours = new bool[8];
            neighbours[0] = GetCell(x - 1, y - 1);      //- | -
            neighbours[1] = GetCell(x, y - 1);          //  | -
            neighbours[2] = GetCell(x + 1, y - 1);      //+ | -
            neighbours[3] = GetCell(x - 1, y);          //- | 
            neighbours[4] = GetCell(x + 1, y);          //+ | 
            neighbours[5] = GetCell(x - 1, y + 1);      //- | +
            neighbours[6] = GetCell(x, y + 1);          //  | +
            neighbours[7] = GetCell(x + 1, y + 1);      //+ | +
            return neighbours;
        }

        int CountOfTrue(bool[] array)
        {
            int count = 0;
            foreach(bool b in array)
            {
                if (b)
                    count++;
            }
            return count;
        }

        bool GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= ScreenWidth || y >= ScreenHeight)
                return false;
            return cells[y * ScreenWidth + x];
        }
    }
}

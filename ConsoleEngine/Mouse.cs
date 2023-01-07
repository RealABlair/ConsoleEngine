using System;

namespace ConsoleEngine
{
    public class Mouse
    {
        public Input[] mouseInput { get; private set; } = new Input[5];

        private bool[] oldStamp = new bool[5];
        public bool[] newMouseStamp = new bool[5];

        public void UpdateStates()
        {
            UpdateStates(this.newMouseStamp);
        }

        public void UpdateStates(bool[] newStamp)
        {
            for(int i = 0; i < oldStamp.Length; i++)
            {
                if (newStamp[i] && !oldStamp[i])
                {
                    mouseInput[i].isDown = true;
                    mouseInput[i].isUp = false;
                    mouseInput[i].isHeld = false;
                }
                else if (!newStamp[i] && oldStamp[i])
                {
                    mouseInput[i].isDown = false;
                    mouseInput[i].isUp = true;
                    mouseInput[i].isHeld = false;
                }
                else if (newStamp[i] && oldStamp[i])
                {
                    mouseInput[i].isDown = false;
                    mouseInput[i].isUp = false;
                    mouseInput[i].isHeld = true;
                }    
                else
                {
                    mouseInput[i].isDown = false;
                    mouseInput[i].isUp = false;
                    mouseInput[i].isHeld = false;
                }
                this.oldStamp[i] = newStamp[i];
            }
        }

        public Input GetInput(int index)
        {
            return mouseInput[index];
        }

        public Input this[int index]
        {
            get => GetInput(index);
        }

        public const int LEFT_MOUSE_BUTTON = 0;
        public const int RIGHT_MOUSE_BUTTON = 1;
        public const int MIDDLE_MOUSE_BUTTON = 2;
    }
}

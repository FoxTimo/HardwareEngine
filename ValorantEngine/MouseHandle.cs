using System;
using System.Runtime.InteropServices;

namespace ValorantEngine
{
    class MouseHandle
    {
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetAsyncKeyState(int vKey);

        private static MouseHandle instance;

        private MouseHandle() { }

        public static MouseHandle Initialize()
        {
            if (instance != null)
            {
                instance = new MouseHandle();
            }

            return instance;
        }

        public static bool IsMouseButtonDown(MOUSE_BUTTONS button)
        {
            return (GetAsyncKeyState((int)button) & 0x8000) > 1;
        }
    }

    enum MOUSE_BUTTONS : int //Source: https://msdn.microsoft.com/nl-nl/library/windows/desktop/dd375731(v=vs.85).aspx
    {
        VK_LBUTTON = 0x01,
        VK_RBUTTON = 0x02,
        VK_MBUTTON = 0x04,
        VK_XBUTTON1 = 0x05,
        VK_XBUTTON2 = 0x06,
        VK_ALT = 0x12,
    }
}

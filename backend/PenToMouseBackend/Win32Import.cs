using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PenToMouseBackend
{
    public static partial class Win32Import        //https://stackoverflow.com/questions/8050825/how-to-move-mouse-cursor-using-c
    {
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]         //https://www.pinvoke.net/default.aspx/user32.setcursorpos
        public static partial bool SetCursorPos(int x, int y);

        //need client to screen also
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        //we can then use this by calling the class and methods
        //need to add mouse_event for clicks https://pinvoke.net/default.aspx/user32.mouse_event
        [LibraryImport("user32.dll")]
        public static partial void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
        //need to define the dwFlags values
        public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const uint MOUSEEVENTF_LEFTUP = 0x0004;

    }
}

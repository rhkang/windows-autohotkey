using System;
using System.Runtime.InteropServices;

namespace Utils
{

    public static class WindowResizer
    {
        private const int MINLENGTH = 100;

        public static void ResizeActiveWindowHeight(int deltaHeight)
        {
            IntPtr hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero || hWnd == GetShellWindow())
                return;

            RECT rect;
            if (!GetWindowRect(hWnd, out rect))
                return;

            int currentHeight = rect.Bottom - rect.Top;
            int newHeight = currentHeight + deltaHeight;

            if (newHeight < MINLENGTH)
                newHeight = MINLENGTH;

            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);
            int posX = rect.Left;
            int posY = rect.Top;

            MoveWindow(hWnd, posX, posY, rect.Right - rect.Left, newHeight, true);
        }

        public static void ResizeActiveWindowWidth(int deltaWidth)
        {
            IntPtr hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero || hWnd == GetShellWindow())
                return;

            RECT rect;
            if (!GetWindowRect(hWnd, out rect))
                return;

            int currentWidth = rect.Right - rect.Left;
            int newWidth = currentWidth + deltaWidth;

            if (newWidth < MINLENGTH)
                newWidth = MINLENGTH;

            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);
            int posX = rect.Left;
            int posY = rect.Top;

            MoveWindow(hWnd, posX, posY, newWidth, rect.Bottom - rect.Top, true);
        }

        public static void UpsizeActiveWindow(float deltaRatio)
        {
            ResizeActiveWindowByDelta(deltaRatio);
        }

        public static void DownsizeActiveWindow(float deltaRatio)
        {
            ResizeActiveWindowByDelta(-deltaRatio);
        }

        private static void ResizeActiveWindowByDelta(float deltaRatio)
        {
            IntPtr hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero || hWnd == GetShellWindow())
                return;

            RECT rect;
            if (!GetWindowRect(hWnd, out rect))
                return;

            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);
            int currentWidth = rect.Right - rect.Left;
            int currentHeight = rect.Bottom - rect.Top;
            float currentRatioW = (float)currentWidth / screenWidth;
            float currentRatioH = (float)currentHeight / screenHeight;
            float newRatioW = Math.Max(0.1f, Math.Min(currentRatioW + deltaRatio, 1.0f));
            float newRatioH = Math.Max(0.1f, Math.Min(currentRatioH + deltaRatio, 1.0f));

            int newWidth = (int)(screenWidth * newRatioW);
            int newHeight = (int)(screenHeight * newRatioH);

            int posX = (screenWidth - newWidth) / 2;
            int posY = (screenHeight - newHeight) / 2;

            MoveWindow(hWnd, posX, posY, newWidth, newHeight, true);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private struct RECT { public int Left, Top, Right, Bottom; }
    }
}
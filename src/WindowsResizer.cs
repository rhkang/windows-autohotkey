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

            // Get monitor info for the active window
            IntPtr monitor = MonitorFromWindow(hWnd, 2); // MONITOR_DEFAULTTONEAREST
            MONITORINFO monitorInfo = new MONITORINFO();
            monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            if (!GetMonitorInfo(monitor, ref monitorInfo))
                return;

            int workLeft = monitorInfo.rcWork.Left;
            int workTop = monitorInfo.rcWork.Top;
            int workWidth = monitorInfo.rcWork.Right - monitorInfo.rcWork.Left;
            int workHeight = monitorInfo.rcWork.Bottom - monitorInfo.rcWork.Top;

            int currentWidth = rect.Right - rect.Left;
            int currentHeight = rect.Bottom - rect.Top;
            float currentRatioW = (float)currentWidth / workWidth;
            float currentRatioH = (float)currentHeight / workHeight;
            float newRatioW = Math.Max(0.1f, Math.Min(currentRatioW + deltaRatio, 1.0f));
            float newRatioH = Math.Max(0.1f, Math.Min(currentRatioH + deltaRatio, 1.0f));

            int newWidth = (int)(workWidth * newRatioW);
            int newHeight = (int)(workHeight * newRatioH);

            int posX = workLeft + (workWidth - newWidth) / 2;
            int posY = workTop + (workHeight - newHeight) / 2;

            MoveWindow(hWnd, posX, posY, newWidth, newHeight, true);
        }

        // Add these P/Invoke and struct definitions if not already present:
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
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
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Utils
{
    public static class WindowManager
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int GWL_STYLE = -16;
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_BORDER = 0x00800000;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_MAXIMIZE = 0x01000000;
        private const int SW_RESTORE = 9;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static void MoveForegroundWindowByOffset(int xOffset, int yOffset)
        {
            IntPtr hWnd = GetForegroundWindow();
            IntPtr shellWindow = GetShellWindow();
            if (hWnd == IntPtr.Zero || hWnd == shellWindow)
                return;

            if (ShouldIncludeWindow(hWnd, shellWindow))
            {
                if (GetWindowRect(hWnd, out RECT rect))
                {
                    int width = rect.Right - rect.Left;
                    int height = rect.Bottom - rect.Top;
                    int newX = rect.Left + xOffset;
                    int newY = rect.Top + yOffset;
                    try
                    {
                        MoveWindow(hWnd, newX, newY, width, height, true);
                    }
                    catch { }
                }
            }
        }

        private static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length == 0) return null;

            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static void CascadeWindows(float ratio)
        {
            IntPtr shellWindow = GetShellWindow();
            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);
            int newWidth = (int)(screenWidth * ratio);
            int newHeight = (int)(screenHeight * ratio);
            int offsetX = 50;
            int offsetY = 40;

            EnumWindows((hWnd, lParam) =>
                {
                    if (ShouldIncludeWindow(hWnd, shellWindow))
                    {
                        int style = GetWindowLong(hWnd, GWL_STYLE);
                        if ((style & WS_MAXIMIZE) != 0 || IsIconic(hWnd))
                        {
                            ShowWindow(hWnd, SW_RESTORE);
                        }
                    }
                    return true;
                }, IntPtr.Zero);

            int windowCount = 0;
            EnumWindows((hWnd, lParam) =>
                {
                    if (ShouldIncludeWindow(hWnd, shellWindow))
                        windowCount++;
                    return true;
                }, IntPtr.Zero);

            int screenCenterX = screenWidth / 2;
            int screenCenterY = screenHeight / 2;
            int startX = screenCenterX + (windowCount - 1) * offsetX / 2;
            int startY = screenCenterY + (windowCount - 1) * offsetY / 2;
            startX -= newWidth / 2;
            startY -= newHeight / 2;

            IntPtr[] windows = new IntPtr[windowCount];
            int index = 0;
            EnumWindows((hWnd, lParam) =>
                {
                    if (ShouldIncludeWindow(hWnd, shellWindow) && index < windowCount)
                    {
                        windows[index++] = hWnd;
                    }
                    return true;
                }, IntPtr.Zero);

            for (int i = 0; i < windowCount; i++)
            {
                IntPtr hWnd = windows[i];
                int posX = startX - (i * offsetX);
                int posY = startY - (i * offsetY);

                try
                {
                    MoveWindow(hWnd, posX, posY, newWidth, newHeight, true);
                }
                catch { }
            }
        }

        private static bool ShouldIncludeWindow(IntPtr hWnd, IntPtr shellWindow)
        {
            if (hWnd == shellWindow || !IsWindowVisible(hWnd))
                return false;

            int style = GetWindowLong(hWnd, GWL_STYLE);
            if ((style & WS_VISIBLE) == 0 || (style & WS_BORDER) == 0)
                return false;

            string title = GetWindowTitle(hWnd);
            if (string.IsNullOrWhiteSpace(title))
                return false;

            if (GetWindowRect(hWnd, out RECT rect))
            {
                int screenWidth = GetSystemMetrics(0);
                int screenHeight = GetSystemMetrics(1);
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                bool isFullscreen = width >= screenWidth &&
                                    height >= screenHeight &&
                                    (style & WS_CAPTION) == 0 &&
                                    (style & WS_BORDER) == 0;

                if (isFullscreen)
                    return false;
            }

            return true;
        }
    }

    public static class WindowResizer
    {
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

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: util.exe cascade [ratio] | up [delta] | down [delta] | move [x] [y]");
                return;
            }

            string action = args[0].ToLowerInvariant();
            float cascadedRatio = 0.66f;
            float resizeDelta = 0.05f;

            switch (action)
            {
                case "cascade":
                    if (args.Length > 1 && float.TryParse(args[1], out float ratio))
                        cascadedRatio = ratio;
                    WindowManager.CascadeWindows(cascadedRatio);
                    break;
                case "up":
                    if (args.Length > 1 && float.TryParse(args[1], out float upDelta))
                        resizeDelta = upDelta;
                    WindowResizer.UpsizeActiveWindow(resizeDelta);
                    break;
                case "down":
                    if (args.Length > 1 && float.TryParse(args[1], out float downDelta))
                        resizeDelta = downDelta;
                    WindowResizer.DownsizeActiveWindow(resizeDelta);
                    break;
                case "move":
                    int x = 0, y = 0;
                    if (args.Length > 2)
                    {
                        int.TryParse(args[1], out x);
                        int.TryParse(args[2], out y);
                    }
                    else if (args.Length > 1)
                    {
                        int.TryParse(args[1], out x);
                        y = x;
                    }
                    WindowManager.MoveForegroundWindowByOffset(x, y);
                    break;
                default:
                    Console.WriteLine("Unknown action: " + action);
                    break;
            }
        }
    }
}
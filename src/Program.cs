using System;

namespace Utils
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: util.exe cascade [ratio] | up [delta] | down [delta] | move [x] [y] | add [x] [y]");
                return;
            }

            string action = args[0].ToLowerInvariant();
            float cascadedRatio = 0.66f;
            float resizeRatioDelta = 0.05f;

            switch (action)
            {
                case "cascade":
                    if (args.Length > 1 && float.TryParse(args[1], out float ratio))
                        cascadedRatio = ratio;
                    WindowManager.CascadeWindows(cascadedRatio);
                    break;
                case "up":
                    if (args.Length > 1 && float.TryParse(args[1], out float upDelta))
                        resizeRatioDelta = upDelta;
                    WindowResizer.UpsizeActiveWindow(resizeRatioDelta);
                    break;
                case "down":
                    if (args.Length > 1 && float.TryParse(args[1], out float downDelta))
                        resizeRatioDelta = downDelta;
                    WindowResizer.DownsizeActiveWindow(resizeRatioDelta);
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
                case "add":
                    int deltaX = 0, deltaY = 0;
                    if (args.Length > 2)
                    {
                        int.TryParse(args[1], out deltaX);
                        int.TryParse(args[2], out deltaY);
                    }
                    else if (args.Length > 1)
                    {
                        int.TryParse(args[1], out deltaX);
                        deltaY = deltaX;
                    }
                    WindowResizer.ResizeActiveWindowWidth(deltaX);
                    WindowResizer.ResizeActiveWindowHeight(deltaY);
                    break;
                default:
                    Console.WriteLine("Unknown action: " + action);
                    break;
            }
        }
    }
}
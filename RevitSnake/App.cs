using Autodesk.Revit.UI;
using System;
using System.Reflection;
using Autodesk.Revit.UI.Events;

namespace RevitSnake
{
    internal class App : IExternalApplication
    {
        private static DateTime lastUpdate = DateTime.MinValue;
        private static TimeSpan updateInterval = TimeSpan.FromMilliseconds(200);

        public Result OnStartup(UIControlledApplication application)
        {
            // ribbon panel and button
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Snake");

            string path = Assembly.GetExecutingAssembly().Location;

            PushButtonData buttonData = new PushButtonData("StartSnakeBtn", "Start Game", path, "RevitSnake.SetupCommand");

            PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;
            pushButton.ToolTip = "Start the snake game";

            // register Idling event to call Game Loop
            application.Idling += IdlingHandler;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            application.Idling -= IdlingHandler;
            return Result.Succeeded;
        }

        static void IdlingHandler(object sender, IdlingEventArgs e)
        {
            if (Game.isRunning)
            {
                DateTime now = DateTime.Now;
                
                // Check if update interval has passed
                if (now - lastUpdate >= updateInterval)
                {
                    lastUpdate = now;
                    Game.MoveSnake();
                }
            }
        }
    }
}

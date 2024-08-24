using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RevitSnake
{
    internal class App : IExternalApplication
    {
        public static UIDocument uidoc;
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // Create a new ribbon panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Snake");

            string path = Assembly.GetExecutingAssembly().Location;

            PushButtonData buttonData = new PushButtonData("StartSnakeBtn", "Start Game", path, "RevitSnake.SampleCommand");

            PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;

            return Result.Succeeded;
        }
    }
}

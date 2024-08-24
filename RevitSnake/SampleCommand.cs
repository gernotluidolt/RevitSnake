using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitSnake
{
    [Transaction(TransactionMode.ReadOnly)]
    public class SampleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            OpenSnakeRvt(commandData.Application.ActiveUIDocument.Document.Title, commandData.Application);

            return Result.Succeeded;
        }
        private void OpenSnakeRvt(String docTitle, UIApplication uiApp)
        {
            // Create and open new document if needed
            if (docTitle == "snake")
            {
                TaskDialog.Show("Revit", "Snake rvt is already open!");
            }
            else
            {
                Document doc = uiApp.Application.NewProjectDocument(UnitSystem.Metric);

                string tempPath = Path.GetTempPath();
                tempPath = Path.Combine(tempPath, "snake.rvt");

                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
                doc.SaveAs(tempPath);


                App.uidoc = uiApp.OpenAndActivateDocument(doc.PathName);

                TaskDialog.Show("Revit", "Opened snake.rvt!");
            }
        }
    }
}

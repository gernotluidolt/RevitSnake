using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCommandTemplate
{
    [Transaction(TransactionMode.ReadOnly)] // Change to TransactionMode.Manual if you need to modify the model
    public class SampleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the UIDocument and Document (if needed)
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // your code here
            TaskDialog.Show("Revit", "Hello World!");

            return Result.Succeeded;
        }
    }
}

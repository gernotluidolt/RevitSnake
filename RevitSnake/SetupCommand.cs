using Autodesk.Revit.Attributes;
using Autodesk.Revit;
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
    /// <summary>
    /// External Command that gets called when the button in the UI is clicked. Prepares the Game.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class SetupCommand : IExternalCommand
    {
        static Level snakeLevel = null;
        public static int size = 24;
        
        UIDocument uidoc;
        public static Document doc;

        static Element[,] grid = new Element[size, size]; // Grid of floors

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            // Open temporary snake.rvt if it is not already open
            if (doc.Title != "snake")
            {
                OpenSnakeRvt(commandData.Application);
            }

            // Create the snake level and view, if they don't exist
            if (snakeLevel == null)
            {
                CreateSnakeLevelAndView(doc, uidoc);
            }

            // Create the environment, if it doesn't exist
            if (grid[0, 0] == null)
            {
                CreateEnvironment(doc, size);
            }
            else
            {
                using (Transaction t = new Transaction(doc, "Clear Grid"))
                {
                    t.Start();
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            grid[i, j].get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("field");
                        }
                    }
                    t.Commit();
                }
            }

            // Apply filters to the view if they don't exist
            if (doc.ActiveView.GetFilters().Count == 0)
            {
                ApplyFilters();
            }
                
            TaskDialog taskDialog = new TaskDialog("Snake");
            taskDialog.MainContent = "Snake Game is ready to play!";
            taskDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

            if (taskDialog.Show() == TaskDialogResult.Ok)
            {
                Game.InitializeGame(6, size);
            }
            else
            {
                Game.isRunning = false;
            }

            return Result.Succeeded;

        }

        /// <summary>
        /// Open temporary snake.rvt if it is not already open.
        /// </summary>
        /// <param name="docTitle">Title of the current Document</param>
        /// <param name="uiApp">UIApplication to handle creating and opening new file</param>
        private void OpenSnakeRvt(UIApplication uiApp)
        {
            string tempPath = Path.GetTempPath();
            tempPath = Path.Combine(tempPath, "snake.rvt");

            if (File.Exists(tempPath))
            {
                uiApp.OpenAndActivateDocument(tempPath);
                uidoc = uiApp.ActiveUIDocument;
                doc = uidoc.Document;
            } 
            else
            {
                Document tmpdoc = uiApp.Application.NewProjectDocument(UnitSystem.Metric);
                tmpdoc.SaveAs(tempPath);
                uidoc = uiApp.OpenAndActivateDocument(tempPath);
                doc = uidoc.Document;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="uiDoc"></param>
        private void CreateSnakeLevelAndView(Document doc, UIDocument uiDoc)
        {
            ElementParameterFilter filter = new ElementParameterFilter(ParameterFilterRuleFactory.CreateEqualsRule(new ElementId(BuiltInParameter.VIEW_NAME), "SnakeView"));
            ViewPlan viewPlan = new FilteredElementCollector(doc).WherePasses(filter).Cast<ViewPlan>().FirstOrDefault();

            if (viewPlan != null)
            {
                using (Transaction t = new Transaction(doc, "Create Snake Level and View"))
                {
                    uiDoc.ActiveView = viewPlan;
                }
            }
            else
            {
                using (Transaction t = new Transaction(doc, "Create Snake Level and View"))
                {
                    t.Start();
                    // Create a new level
                    Level level = Level.Create(doc, 0);
                    level.Name = "Snake";
                    snakeLevel = level;

                    // Create a new ViewPlan

                    // Get FamilySymbol for ViewPlan
                    ViewFamilyType viewFamilyType = new FilteredElementCollector(doc)
                        .OfClass(typeof(ViewFamilyType))
                        .Cast<ViewFamilyType>()
                        .FirstOrDefault(x => x.ViewFamily == ViewFamily.FloorPlan);

                    // Create ViewPlan
                    viewPlan = ViewPlan.Create(doc, viewFamilyType.Id, level.Id);
                    viewPlan.Name = "SnakeView";
                    t.Commit();

                    uiDoc.ActiveView = viewPlan;
                    
                }
            }
        }

        /// <summary>
        /// Creates the Grid and Walls for the Snake Game
        /// </summary>
        /// <param name="doc"></param>
        private void CreateEnvironment(Document doc, int size)
        {
            // Get default floor type
            ElementId floorTypeId = new FilteredElementCollector(doc)
                .OfClass(typeof(FloorType))
                .Cast<FloorType>()
                .First()
                .Id;

            // greate a grid of floors size x size
            using (Transaction t = new Transaction(doc, "Create Environment"))
            {
                t.Start();
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        XYZ p1 = new XYZ(i, j, 0);
                        XYZ p2 = new XYZ(i + 1, j, 0);
                        XYZ p3 = new XYZ(i + 1, j + 1, 0);
                        XYZ p4 = new XYZ(i, j + 1, 0);

                        IList<CurveLoop> loop = new List<CurveLoop>();
                        CurveLoop curveLoop = new CurveLoop();
                        curveLoop.Append(Line.CreateBound(p1, p2));
                        curveLoop.Append(Line.CreateBound(p2, p3));
                        curveLoop.Append(Line.CreateBound(p3, p4));
                        curveLoop.Append(Line.CreateBound(p4, p1));
                        loop.Add(curveLoop);

                        Floor newFloor = Floor.Create(doc, loop, floorTypeId, snakeLevel.Id);

                        grid[i, j] = newFloor;
                        grid[i, j].get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Field");
                    }
                }
                t.Commit();
            }
        }

        public static void SetCellValue(int x, int y, string value)
        {
            Transaction t = new Transaction(doc, "Set Cell Value");
            t.Start();
            grid[x, y].get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(value);
            t.Commit();
        }

        private void ApplyFilters()
        {
            using (Transaction t = new Transaction(doc, "Apply Filters"))
            {
                t.Start();
                ICollection<ElementId> categories = new List<ElementId>();
                categories.Add(new ElementId(BuiltInCategory.OST_Floors));

                // Create filters for field, snake and food
                ElementParameterFilter filterField = new ElementParameterFilter(ParameterFilterRuleFactory.CreateEqualsRule(new ElementId(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS), "Field"));
                ElementParameterFilter filterSnake = new ElementParameterFilter(ParameterFilterRuleFactory.CreateEqualsRule(new ElementId(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS), "Snake"));
                ElementParameterFilter filterFood = new ElementParameterFilter(ParameterFilterRuleFactory.CreateEqualsRule(new ElementId(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS), "Food"));

                // Graphic overrides to set color of field, snake and food
                OverrideGraphicSettings fieldGraphicSettings = new OverrideGraphicSettings();
                fieldGraphicSettings.SetProjectionLineColor(new Color(180, 255, 112));
                fieldGraphicSettings.SetSurfaceForegroundPatternColor(new Color(180, 255, 112));
                fieldGraphicSettings.SetSurfaceForegroundPatternId(GetFillPatternId());

                OverrideGraphicSettings snakeGraphicSettings = new OverrideGraphicSettings();
                snakeGraphicSettings.SetProjectionLineColor(new Color(94, 128, 235));
                snakeGraphicSettings.SetSurfaceForegroundPatternColor(new Color(94, 128, 235));
                snakeGraphicSettings.SetSurfaceForegroundPatternId(GetFillPatternId());

                OverrideGraphicSettings foodGraphicSettings = new OverrideGraphicSettings();
                foodGraphicSettings.SetProjectionLineColor(new Color(242, 125, 94));
                foodGraphicSettings.SetSurfaceForegroundPatternColor(new Color(242, 125, 94));
                foodGraphicSettings.SetSurfaceForegroundPatternId(GetFillPatternId());

                ParameterFilterElement fieldFilter = ParameterFilterElement.Create(doc, "Field", categories, filterField);
                ParameterFilterElement snakeFilter = ParameterFilterElement.Create(doc, "Snake", categories, filterSnake);
                ParameterFilterElement foodFilter = ParameterFilterElement.Create(doc, "Food", categories, filterFood);

                // Apply filters and graphic overrides
                doc.ActiveView.AddFilter(fieldFilter.Id);
                doc.ActiveView.SetFilterOverrides(fieldFilter.Id, fieldGraphicSettings);

                doc.ActiveView.AddFilter(snakeFilter.Id);
                doc.ActiveView.SetFilterOverrides(snakeFilter.Id, snakeGraphicSettings);

                doc.ActiveView.AddFilter(foodFilter.Id);
                doc.ActiveView.SetFilterOverrides(foodFilter.Id, foodGraphicSettings);

                t.Commit();
            }

            

        }
        ElementId GetFillPatternId()
        {
            FilteredElementCollector allFills = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement));
            FillPatternElement solidFill = allFills.Cast<FillPatternElement>().First(x => x.GetFillPattern().IsSolidFill);
            return solidFill.Id;
        }
    }
}

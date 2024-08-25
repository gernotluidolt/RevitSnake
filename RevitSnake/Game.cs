using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace RevitSnake
{
    internal class Game
    {
        public static List<(int x, int y)> snakePositions = new List<(int x, int y)>();
        public static (int x, int y) foodPosition;

        public static (int x, int y) direction = (1, 0);
        private static object directionLock = new object();

        public static bool gameOver = false;
        public static bool isRunning = false;

        private static Random random = new Random();
        private static ControlWindow controlWindow;

        // Update direction and preventing the snake from reversing on itself
        public static void UpdateDirection((int x, int y) newDirection)
        {
            lock (directionLock)
            {
                if (direction.x + newDirection.x != 0 || direction.y + newDirection.y != 0)
                {
                    direction = newDirection;
                }
            }
        }

        public static void InitializeGame(int initialLength, int gridSize)
        {
            if (controlWindow == null)
            {
                controlWindow = new ControlWindow();
                controlWindow.Show();
            }

            System.Threading.Thread.Sleep(2000);

            int startX = gridSize / 2;
            int startY = gridSize / 2;

            for (int i = initialLength; i > 0; i--)
            {
                snakePositions.Add((startX, startY - i));
            }

            SpawnFood();
            isRunning = true;
            UpdateGrid();
        }

        public static void MoveSnake()
        {
            (int x, int y) head = snakePositions.Last();
            (int x, int y) newHead = (head.x + direction.x, head.y + direction.y);

            // Check if the new head position has hit the wall
            if (newHead.x < 0 || newHead.x >= SetupCommand.size || newHead.y < 0 || newHead.y >= SetupCommand.size)
            {
                gameOver = true;
                isRunning = false;
                TaskDialog.Show("Game Over", "You hit the wall!");
                controlWindow.Close();
                controlWindow = null;
                return;
            }

            // Check if the new head position is colliding with the snake
            if (snakePositions.Contains(newHead))
            {
                gameOver = true;
                isRunning = false;
                TaskDialog.Show("Game Over", "You collided with yourself!");
                controlWindow.Close();
                controlWindow = null;
                return;
            }

            // Check if the new head position is colliding with the food
            if (newHead == foodPosition)
            {
                snakePositions.Add(newHead);
                SpawnFood();
            }
            else
            {
                snakePositions.Add(newHead);

                SetupCommand.SetCellValue(snakePositions[0].x, snakePositions[0].y, "Field");
                snakePositions.RemoveAt(0);
            }

            UpdateGrid();
        }

        private static void SpawnFood()
        {
            int gridSize = SetupCommand.size;

            (int x, int y) position;
            do
            {
                position = (random.Next(0, gridSize), random.Next(0, gridSize));
            }
            while (snakePositions.Contains(position)); // Ensure food doesn't spawn on the snake

            foodPosition = position;

            SetupCommand.SetCellValue(foodPosition.x, foodPosition.y, "Food");
        }

        private static void UpdateGrid()
        {
            foreach (var position in snakePositions)
            {
                SetupCommand.SetCellValue(position.x, position.y, "Snake");
            }

            SetupCommand.SetCellValue(foodPosition.x, foodPosition.y, "Food");
        }
    }
}

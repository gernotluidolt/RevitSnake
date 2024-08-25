using System.Windows;
using System.Windows.Input;

namespace RevitSnake
{
    public partial class ControlWindow : Window
    {
        public ControlWindow()
        {
            InitializeComponent();
            this.KeyDown += ControlWindow_KeyDown;
        }

        private void ControlWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    Game.UpdateDirection((0, 1)); // Up
                    break;
                case Key.A:
                    Game.UpdateDirection((-1, 0)); // Left
                    break;
                case Key.S:
                    Game.UpdateDirection((0, -1)); // Down
                    break;
                case Key.D:
                    Game.UpdateDirection((1, 0)); // Right
                    break;
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Game.isRunning)
            {
                Game.isRunning = false;
                PauseButton.Content = "Resume";
            }
            else
            {
                Game.isRunning = true;
                PauseButton.Content = "Pause";
            }
        }

        public void UpdateScore()
        {
            Dispatcher.Invoke(() =>
            {
                ScoreTextBlock.Text = $"Score: {Game.score}";
            });
        }
    }
}

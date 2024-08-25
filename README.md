# RevitSnake - A Snake Game for Autodesk Revit

## Overview

RevitSnake is a fun and interactive snake game built within Autodesk Revit. This project leverages the Revit API to create a playable version of the classic Snake game directly in the Revit environment. The game grid is built using Revit floor elements and filters with graphic overwrites, the snake is controlled via keyboard inputs.
The Project was born from a weekend experiment to explore whether Revit could be repurposed as a simple game engine—successfully proving it can.

![RevitSnake Picture](https://i.imgur.com/Tt9hiqG.png)


## Installation

1. **Clone the Repository**: Clone this repository to your local machine.

   ```bash
   git clone https://github.com/gernotluidolt/RevitSnake.git
   ```

2. **Open the Project**: Open the RevitSnake solution in Visual Studio.

3. **Build the Project**: Ensure all dependencies are installed, then build the solution.

4. **Load the Add-In**: 
   - Copy the resulting DLL file from the build output to your Revit add-ins directory.

5. **Run the Game**: Once loaded, a "Snake" ribbon panel with a "Start Game" button will appear in Revit. Click it to launch the game.

## How It Works

### Game Mechanics

It's snake, you know the game mechanics.

### Core Components

1. **App.cs**
   - Registers the game button in Revit's UI.
   - Handles the game loop via the `Idling` event.

2. **ControlWindow.cs**
   - WPF Window that captures keyboard inputs to control the snake.
   - Manages the Pause/Resume functionality and displays the score.

3. **Game.cs**
   - Contains the game logic, including snake movement, collision detection, and score updates.
   - Manages the game grid, which is created using Revit elements.

4. **SetupCommand.cs**
   - Prepares the Revit environment for the game.
   - Creates necessary elements.
   - Manages the game's visual representation within Revit using filters and graphic overrides.

## Customization

You can customize the game by modifying the following parameters in `SetupCommand.cs`:

- **Grid Size**: Adjust the size of the game grid by changing the `size` variable.
- **Initial Snake Length**: Set the initial length of the snake by modifying the `initialLength` parameter in `Game.InitializeGame`.
- **Update Interval**: Change the snake's movement speed by adjusting the `updateInterval` in `App.cs`.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more details.

## Contributing

Contributions are welcome! Feel free to fork this repository and submit pull requests with enhancements, bug fixes, or new features.

## Contact

For any questions or feedback, please contact me at [gernot@luido.lt].

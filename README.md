
# SDL2 C# Engine

## Overview
This simple engine for creating 2D games using SDL2 and C#. It allows for easy manipulation of game objects and components through a straightforward API. The engine supports basic vector operations, input handling, game object management, and rendering capabilities.

## Structure
The engine is organized into several key classes and components:

- `Vec2D` and `Vec3D`: Structures for 2-dimensional and 3-dimensional vector operations respectively.
- `GameObject`: The base class for all entities in the game world. It can have children and components.
- `Component`: The base class for all behaviors that can be attached to GameObjects.
- `Scene`: Represents a collection of GameObjects that make up a game scene.
- `Camera` and `Camera2D`: Abstract and concrete classes for handling the camera's view and projection in the game world.
- `Drawable`: An abstract class that drawable components inherit from to render graphics.
- `RotatingSquare`: An example drawable component that renders a rotating square.
- `Input`: A static class to handle user input such as keyboard and mouse events.
- `Engine`: The main class that initializes SDL2, handles game loops, and manages updates and rendering.

## How to Use

### Creating a Scene

To create a new scene with a rotating square and mouse interaction, you can use the following example:

```csharp
Scene myScene = CreateScene();

Engine engine = new Engine(myScene);
engine.Run();
```

### Example Function `CreateScene`

```csharp
public static Scene CreateScene()
{
    var scene = new Scene();
    var gameObject = new GameObject(scene);
    gameObject.AddComponent<RotatingSquare>();
    gameObject.AddComponent<MouseTracker>();
    scene.AddChild(gameObject);
    gameObject.SetPosition(new Vec2D(500, 500));
    return scene;
}
```

This function sets up a scene with a single GameObject that includes a `RotatingSquare` component for visual rendering and a `MouseTracker` component for mouse position tracking.

## Requirements
- SDL2
- SDL2_ttf (for text rendering)
- .NET compatible development environment

## Building and Running
1. Ensure SDL2 and SDL2_ttf libraries are installed and correctly linked in your project settings.
2. Compile the project with a C# compiler that supports C# 8.0 or higher.
3. Run the output executable to start the engine.


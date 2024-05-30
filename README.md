
# SDL2 C# Engine

## Overview
This simple engine for creating 2D games using SDL2 and C# with C# bindings from [SDL2-CS](https://github.com/flibitijibibo/SDL2-CS). It allows for easy manipulation of game objects and components through a straightforward API. The engine supports basic vector operations, input handling, game object management, and rendering capabilities.

```bash
git clone --recurse-submodules https://github.com/4-en/sdl2-engine.git
```

## Structure
The engine is organized into several key classes and components:

- `Vec2D` and `Vec3D`: Structures for 2-dimensional and 3-dimensional vector operations respectively.
- `GameObject`: The base class for all entities in the game world. It can have children and components.
- `Component`: The base class for all behaviors that can be attached to GameObjects.
- `Scene`: Represents a collection of GameObjects that make up a game scene.
- `Camera` and `Camera2D`: Abstract and concrete classes for handling the camera's view and projection in the game world.
- `Drawable`: An abstract class that drawable components inherit from to render graphics.
- `TextureRenderer`: A simple renderer that can be used to render image textures.
- `SpriteRenderer`: A renderer that supports sprite maps and different animations.
- `TextRenderer`: A simple renderer to draw text, background and a border.
- `Input`: A static class to handle user input such as keyboard and mouse events.
- `Time`: A static class that handles time related variables like time since start, deltaTime or tick count.
- `Engine`: The main class that initializes SDL2, handles game loops, and manages updates and rendering.
- `Physics`: Uses different Components like PhysicsBody and Colliders to move GameObjects and solve collisions.
- `Assets`: Loadable Assets like Textures or Sounds that can be used in Components.
- `Prototype`: Defines GameObjects with Components that can be instantiated.
- `Coroutine`: Functions to start coroutines from custom scripts.
- `SceneSerialization`: Functions to save/load Scenes to/from files, including state of GameObjects and Components.

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
Clone this repository including submodules:
```bash
git clone --recurse-submodules https://github.com/4-en/sdl2-engine.git
```

Update submodules afterwards:
```bash
git submodule update --init --recursive
```

Required Libraries (so far):
- SDL2
- SDL2_ttf
- SDL2_image
- SDL2_mixer

Windows DLLs are already provided.
For Linux, use install_requirements.sh to install libraries or build from source manually.

## Building and Running
1. Ensure SDL2 and SDL2_ttf libraries are installed and correctly linked in your project settings.
2. Compile the project with a C# compiler that supports C# 8.0 or higher.
3. Run the output executable to start the engine.


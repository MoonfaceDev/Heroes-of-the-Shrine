# Creating Scenes

Heroes of the Shrine is divided into scenes. The Unity manual states:

> Scenes are where you work with content in Unity. They are assets that contain all or part of a game or application. For example, you might build a simple game in a single scene, while for a more complex game, you might use one scene per level, each with its own environments, characters, obstacles, decorations, and UI
. You can create any number of scenes in a project.

Each level can rely on multiple scenes, each representing a different part in the level. Scene transitions are costy in terms of client resources, thus they shouldn't occur very often.

## Scene Structure

In our game, each scenes should have a `Scene Root` game object. `Scene Root` is a prefab, located in *Assets/Prefabs*, and it contains essential global objects and scripts (and even the player itself).

![Scene root hierarchy](resources/SceneRootHierarchy.png)

Scenes will also contain enemies, UI, and level definitions (encounters, transitions, cutscenes, etc.)

> [!NOTE]
> All UI elements must live under the `Canvas` object in the `Scene Root`

## Create a Scene

To add a new scene follow these steps:

1. Create a scene assets in *Assets/Scenes*. Give it an indicative name, such as *Level3Forest*.
2. Get inside your new scene, by double clicking the asset.
3. Add `Scene Root` to the scene, by dragging it from *Assets/Prefabs/Scene Root*.
4. Add scene background using normal 2D sprites.
5. Add the enemies, encounters, hazards and cutscenes - everything that makes this scene unique.
6. Save your changes!

> [!WARNING]
> *Ctrl+S* is usually sufficient for saving, but sometimes it might skip some parts. In order to make sure you saved everything, right click the scene object in the hierarchy, and click *Save Scene*
> ![Scene root hierarchy](resources/SaveScene.png)
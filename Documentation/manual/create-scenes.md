# Creating Scenes

Heroes of the Shrine is divided into scenes. The Unity manual states:

> Scenes are where you work with content in Unity. They are assets that contain all or part of a game or application. For example, you might build a simple game in a single scene, while for a more complex game, you might use one scene per level, each with its own environments, characters, obstacles, decorations, and UI
. You can create any number of scenes in a project.

Each level can rely on multiple scenes, each representing a different part in the level. Scene transitions are costy in terms of client resources, thus they shouldn't occur very often. However, and even more important, is to use scenes where it fits.

## Scene Structure

In our game, each scenes should have a **Scene Root** game object. *Scene Root* is a prefab, located in *Assets/Prefabs*, and it contains essential global objects and scripts (and even the player itself).

![Scene root hierarchy](../resources/SceneRootHierarchy.png)

Scenes will also contain enemies, UI, and level definitions (encounters, transitions, cutscenes, etc.)

> [!NOTE]
> All UI elements must live under the *Canvas* object in the *Scene Root*.

## Create a Scene

To add a new scene follow these steps:

1. Create a scene asset in *Assets/Scenes*. Give it an indicative name, such as *Level3Forest*.

2. Get inside your new scene, by double clicking the asset.

3. Delete the *Main Camera* object that is included in the scene template. The *Scene Root* you are about to add, has a camera that already conatins all needed scripts.

4. Add **Scene Root** to the scene, by dragging it from *Assets/Prefabs/Scene Root*.

5. Add scene background using normal 2D sprites.

6. Set the **walkable grid** *position* and *size*.

7. Set **world border** in the camera's [Camera Movement](../api/Global.CameraMovement.html) component.

8. Set the camera's initial position using its *transform*.

9. If you want to change the camera zoom, go to the component *Pixel Perfect Camera*. Click "Run In Edit Mode", and play with the value of *Assets Pixels Per Unit*.

10. Add the entrance cutscene inside **Player Entrance** object. Refer to the [cutscenes manual](../manual/create-cutscenes.md), and skip steps 1-4 because they have already been done in *Scene Root*. Also, in step 7, the *Playable Director* already exists. You only need to attach the timeline from step 6 to it.

    > [!WARNING]
    > The player position should always stay inside the *walkable grid*. To achieve that, set the player's position (in [Movable Object](../api/Global.MovableObject.html)) to be inside the *walkable grid*. Also, if you animate the position inside the timeline, make sure it is always inside the grid, even in the first frame.

11. In **Player Death**, set the *Objects To Destroy*. The list should have the HUD (by default), and the foreground sprite of the level (you have to select it manually).

12. Add the enemies, encounters, hazards and cutscenes - everything that makes this scene unique.

13. Save your changes!

    > [!WARNING]
    > *Ctrl+S* is usually sufficient for saving, but sometimes it might skip some parts. In order to make sure you saved everything, right click the scene object in the hierarchy, and click "Save Scene".
    > ![Scene root hierarchy](../resources/SaveScene.png)
# Changelog World Generation


## [1.9.1] - 2023-06-25
## Fixed
- Error in saving data from menu
- First Person character always falling through ground


## [1.9.0] - 2023-06-23
## Added
- FlyCam
- It is possible to switch between FlyCam and FirstPerson


## [1.8.0] - 2023-06-21
## Added
- First Person Camera
- First Person Movement


## [1.7.5] - 2023-06-20
# Changed
- Counter for rabbits and foxes
  - if one counter reaches 0 the level gets reloaded

 
## [1.7.4] - 2023-06-19
- Trying to solve problems with carrots


## [1.7.3] - 2023-06-17
## Added
- float variable for height of mountain area
- DeathFloor, if Agents fall through the terrain and hit the DeathFloor they die

## Changed 
- Agents are now instantiated only after generation is complete


## [1.7.2] - 2023-06-16
## Changed
- All scripts are now in one global folder/directory
- Reworked Reproduce()
- Fixed clock
- Fixed RespawnPlants()


## [1.7.1] - 2023-06-14
## Changed
- RespawnPlants now only respawns carrots
- Radius of burrow
- Spawn procedure for Grass and Reed


## [1.7.0] - 2023-06-09
## Added
- Main Menu
  - seed
  - maxTerrainHeight
  - resolution

## Changed
- AssetManager
  - inMountainRegion


## [1.6.0] -2023-06-07
## Added
- Date display


## [1.5.2] - 2023-06-06
## Added
- Missing code in Breed()


## [1.5.1] - 2023-06-03
## Added
- Unload Scene


## [1.5.0] - 2023-06-02
## Changed
- Moved Rabbit/Fox/Burrow lists to separate file and changed them to Dictionaries
  - agents are saved with their ID as key
- ID initialization for agents now depends on a counter and is no loner a random number
  - each agent has its own counter

## [1.4.0] - 2023-05-31
## Added
- numberOfPlacementAttempts added to asset settings
- Collider to water
  - changed ProjectSettings so Raycasts can hit but collider is not collidable


## [1.3.0] - 2023-05-30
## Added
- Separate prefabs for rabbit and ox burrow
- Break counter for asset spawning

## Changed
- deleted delay before carrot gets destroyed
- Burrow / Carrot:
  - Check if object needs to be destroyed now only when day is over
  
## Removed
- Trees, reed and grass are no longer able to die or reproduce


## [1.2.3] - 2023-05-27
## Changed
- Kill():
  - Check if Agent is in list -> then delete


## [1.2.2] - 2023-05-22
## Changed
- localScale: Fox, Rabbit, Burrow


## [1.2.1] - 2023-05-18
## Bugfixes
- Leave Burrow
  - refactored while loop


## [1.2.0] - 2023-05-17
## Added
- Ground Check for burrow


## [1.1.0] - 2023-05-16
## Added
- Spawn points for leaving the burrow

## Changed
- ReloadWorld()


## [1.0.0] - 2023-05-15
## Added
- Spawn Point for baby rabbit

## Changed
- Bugfix in TimeManager

## Function Test
- LeaveBurrow()
- EnterBurrow()


## [0.12.0] - 2023-05-08
## Added
- Reproduction for Carrots
- Position Check


## [0.11.0] - 2023-05-05
## Added
- GameManager
- Missing Collider to Carrot
- Carrot changes hunger of rabbit
- Breed() to Burrow
- BuildBurrow() for rabbits

## Changed
- Wall Prefab now belongs to GroundGenerator
- Minor changes in AssetManager
- ReloadWorld() now also kills all rabbits in the scene
- Burrow now has the right graphic, no longer placeholder
- Respawning of plants only checked once per month


## [0.10.0] - 2023-05-02
## Added
- Reed around water (https://assetstore.unity.com/packages/3d/vegetation/lowpoly-vegetation-season-pack-lite-96083)
- different types of trees (https://assetstore.unity.com/packages/3d/vegetation/lowpoly-vegetation-season-pack-lite-96083)
- RabbitBurrow Placeholder
- InitialSpawn for RabbitBurrow

## Changed
- Carrot
  - Interact() to Eat()
- RabbitBurrow
  - Interact() to Enter() and Leave()


## [0.9.0] - 2023-04-20
## Changed
- Assets are no longer underwater
- New Carrot Prefab


## [0.8.0] - 2023-04-14
## Added
- Grass (From Daniel Ilet - https://github.com/daniel-ilett/shaders-6grass)


## [0.8.0] - 2023-04-19
## Added
- Random Spawner for Grass

## Changed
- Wrong calculation for InGameDates
- Code Refactoring


## [0.7.0] - 2023-04-13
## Added
- Random Spawner for carrots

## Changed
- OpenSimplexNoise instead of Perlin Noise


## [0.6.0] - 2023-04-12
- Code Refactoring

## Added
- WorldManager
- TerrainManager
- AssetManager
- Spawner


## [0.5.2] - 2023-04-05
## Added
- Mesh Collider


## [0.5.1] - 2023-03-30
## Added
- Classes:
  - NoiseGenerator
- ScriptableObjects
  - GeneralSettings

## Changed
  - MapGenerator -> GroundGenerator

- CodeCleanup
- CodeRefactoring


## [0.4.0] - 2023-03-23
## Added
- Classes:
  - TimeManager
    - New data struct InGameDate
    - ToString method
    - Check for correct input
    - 3 different timeScales
    - Resume
    - Pause 
  
## Changed
  - MapGenerator -> GroundGenerator
  - WaterGenerator
  - MeshGenerator
  - GeneratorMain


## [0.3.0] - 2023-03-20
## Added
- Classes:
  - TimeManager
    - Handles inGame time


## [0.2.0] - 2023-03-19
## Added
- Classes:
  - ColorGenerator
  - WaterGenerator
    - generate mesh
    - add shader
  - uv-map for mesh generation


## [0.1.0] - 2023-03-15
## Added
- Classes:
  - MainGenerator
  - MapGenerator
  - MeshGenerator
- Color for different heights
# Changelog World Generation


## [0.9.0] - 2023-04-20
## Changed
- Assets are no longer underwater
- New Carrot Prefab


## [0.8.0] - 2023-04-14
## Added
- Grass Mesh (From Daniel Ilet - https://github.com/daniel-ilett/shaders-6grass)
- Random Spawner for Grass


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
    - New data struct IngameDate
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
    - Handles ingame time


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
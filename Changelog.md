# Changelog World Generation


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
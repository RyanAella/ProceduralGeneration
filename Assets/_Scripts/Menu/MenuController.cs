
using _Scripts.WorldGeneration.Helper;
using _Scripts.WorldGeneration.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.Menu
{
    public class MenuController : MonoBehaviour
    {
        #region Variables
        
        public string newGameLevel;
        public GeneralSettings generalSettings;

        [Header("Settings")] [SerializeField] private TMP_InputField seed; // 0
        [SerializeField] private Toggle randomToggle;
        
        [SerializeField] private TMP_InputField maxTerrainHeight; // 1
        [SerializeField] private TMP_InputField resolutionX; // 2   
        [SerializeField] private TMP_InputField resolutionY; // 3

        [SerializeField] private Toggle aboveToggle;
        [SerializeField] private Toggle withinToggle;

        private string _levelToLoad;
        private float _maxTerrainHeight;
        private Vector2Int _resolution;
        private int _resolutionX;
        private int _resolutionY;
        private string _seed;

        private int _selectedInput;
        
        #endregion

        #region Unity Methods
        
        private void Start()
        {
            _seed = generalSettings.seed;
            randomToggle.isOn = generalSettings.useRandomSeed;
            _maxTerrainHeight = generalSettings.maxTerrainHeight;
            _resolution = generalSettings.resolution;
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Tab)) return;
            
            _selectedInput++;
            if (_selectedInput > 3) _selectedInput = 0;
            SelectInputField();
        }

        private void SelectInputField()
        {
            switch (_selectedInput)
            {
                case 0:
                    seed.Select();
                    break;
                case 1:
                    maxTerrainHeight.Select();
                    break;
                case 2:
                    resolutionX.Select();
                    break;
                case 3:
                    resolutionY.Select();
                    break;
            }
        }

        public void SeedSelected()
        {
            _selectedInput = 0;
        }

        public void MaxTerrainHeightSelected()
        {
            _selectedInput = 1;
        }

        public void ResolutionXSelected()
        {
            _selectedInput = 2;
        }

        public void ResolutionYSelected()
        {
            _selectedInput = 3;
        }

        public void StartButton()
        {
            if (randomToggle.isOn)
            {
                generalSettings.useRandomSeed = true;
                generalSettings.seed = Time.realtimeSinceStartupAsDouble.ToString();
            }
            else
            {
                SetSeed();
            }
            
            if (aboveToggle.isOn) generalSettings.watchMode = WatchMode.FlyCam;
            else if (withinToggle.isOn) generalSettings.watchMode = WatchMode.FirstPerson;
            
            SetResolution();
            SetMaxTerrainHeight();

            Checker.IsStartingFromMenu = true;

            SceneManager.LoadSceneAsync(newGameLevel);
        }

        public void ExitButton()
        {
            Application.Quit();
        }

        public void SetSeed()
        {
            generalSettings.seed = seed.text == "" ? "Labor Games 2023" : seed.text;
        }

        public void SetResolutionX()
        {
            _resolutionX = resolutionX.text == "" ? 12 : int.Parse(resolutionX.text);
        }

        public void SetResolutionY()
        {
            _resolutionY = resolutionY.text == "" ? 12 : int.Parse(resolutionY.text);
        }

        private void SetResolution()
        {
            SetResolutionX();
            SetResolutionY();
            
            generalSettings.resolution = new Vector2Int(_resolutionX, _resolutionY);
        }

        public void SetMaxTerrainHeight()
        {
            generalSettings.maxTerrainHeight = maxTerrainHeight.text == "" ? 50f : float.Parse(maxTerrainHeight.text);
        }

        public void ResetButton()
        {
            // Reset values from generalSettings
            generalSettings.seed = _seed;
            generalSettings.maxTerrainHeight = _maxTerrainHeight;
            generalSettings.resolution = _resolution;
        }
        
        #endregion
    }
}
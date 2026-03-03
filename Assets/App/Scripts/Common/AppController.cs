using SGSTools.Components;
using SGSTools.Extensions;
using SGSTools.Util;
using UnityEngine;

namespace SpaceInvaders
{
    public class AppController : MonoBehaviour
    {
        [Header("Config")]
        public GameConfig GameConfig;
        
        [Header("Debug Options")]
        public bool UseTouchControls;
        
        [Header("References")]
        public ObjectShaker CameraShaker;
        public Material VignetteMaterial;
        
        [Space]
        public AudioController AudioController;
        public GameController GameController;
        public UIController UIController;

        public InputService InputService { get; private set; } = new InputService();
        public HighScoreService HighScoreService { get; private set; } = new HighScoreService();

        private float _cameraAspect;
        
        private void Awake()
        {
            ServiceLocator.Add(this);
            ServiceLocator.Add(AudioController);
            ServiceLocator.Add(InputService);
            ServiceLocator.Add(GameController);
            ServiceLocator.Add(UIController);
            ServiceLocator.Add(GameConfig);
            
            Application.targetFrameRate = 60;
            DebugDraw.IsEnabled = false;
            DebugDraw.Settings.DefaultColor = Color.gray;
            
            InputService.Init();
            UIController.Init();
            GameController.Init();
            ResetVignette();
            
            if (IsMobilePlatform())
            {
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                Screen.orientation = ScreenOrientation.LandscapeRight;
                Screen.fullScreen = true;
            }
            
            _cameraAspect = CameraUtils.MainCamera.aspect;
            AdaptToCameraAspect();
        }
        
        private void OnDestroy()
        {
            ResetVignette();
        }

        private void Update()
        {
            // debug
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                DebugDraw.IsEnabled = !DebugDraw.IsEnabled;
            }
            
            // camera
            var cameraAspect = CameraUtils.MainCamera.aspect;
            if (_cameraAspect != cameraAspect)
            {
                _cameraAspect = cameraAspect;
                AdaptToCameraAspect();
            }

            // gameplay
            GameController.OnUpdate();
        }

        private void FixedUpdate()
        {
            GameController.OnFixedUpdate();
        }

        public bool IsMobilePlatform()
        {
#if UNITY_EDITOR
            return UseTouchControls;
#endif
            return Application.isMobilePlatform;
        }
        
        public void UpdateVignette(float power, bool isAdditive)
        {
            VignetteMaterial.SetFloat(Constants.VIGNETTE_PROPERTY_POWER, power);
            VignetteMaterial.SetFloat(Constants.VIGNETTE_PROPERTY_IS_ADDITIVE, isAdditive ? 1f : 0f);
        }
        
        private void AdaptToCameraAspect()
        {
            var aspect = Mathf.Clamp(_cameraAspect, Constants.CAMERA_ASPECT_MIN, Constants.CAMERA_ASPECT_MAX);
            var aspectT = 1f - Mathf.InverseLerp(Constants.CAMERA_ASPECT_MIN, Constants.CAMERA_ASPECT_MAX, aspect);
            
            var camera = CameraUtils.MainCamera;
            camera.orthographicSize = GameConfig.CameraSizeRange.GetValueAt(aspectT);
            var canvasScale = GameConfig.CanvasScaleRange.GetValueAt(aspectT);
            UIController.SafeAreaRoot.transform.SetLocalScale(canvasScale);
            Debug.Log($"New camera aspect: {_cameraAspect}");
        }
        
        public void ResetVignette()
        {
            UpdateVignette(GameConfig.VignettePowerRange.Min, isAdditive: false);
        }
    }
}
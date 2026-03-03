using System;
using DG.Tweening;
using SGSTools.Extensions;
using SGSTools.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceInvaders
{
    public class UIController : MonoBehaviour
    {
        public const float TRANSITION_DURATION = 0.5f;
        
        public Canvas Canvas;
        public GameObject SafeAreaRoot;
        
        [Space]
        public MainMenuScreen MainMenuScreen;
        public GameplayScreen GameplayScreen;
        public GameOverScreen GameOverScreen;
        public HighScoresScreen HighScoresScreen;
        public ControlsScreen ControlsScreen;

        [Space]
        public Button MuteButton;
        public Sprite AudioOnSprite;
        public Sprite AudioOffSprite;
        
        [Space]
        public GameObject PauseOverlayView;
        public UIButton ResumeButton;
        public UIButton ExitButton;
        
        [Space]
        public Image TransitionImage;

        public BaseScreen ActiveScreen { get; private set; }
        public bool IsTransitioning { get; private set; }
        private bool IsAudioMuted => AudioListener.volume == 0f;
        
        private GameController GameController => ServiceLocator.Get<GameController>();

        public void Init()
        {
            MainMenuScreen.Init();
            GameplayScreen.Init();
            GameOverScreen.Init();
            HighScoresScreen.Init();
            ControlsScreen.Init();

            TransitionImage.SetAlpha(1f);
            TransitionImage.gameObject.SetActive(true);

            MuteButton.onClick.AddListener(OnMuteButtonClicked);
            MuteButton.image.sprite = AudioOnSprite;
            
            ResumeButton.Text.text = Strings.BUTTON_RESUME;
            ExitButton.Text.text = Strings.BUTTON_EXIT;
            
            ResumeButton.Button.onClick.AddListener(OnResumeButtonClicked);
            ExitButton.Button.onClick.AddListener(OnExitButtonClicked);
            SetPauseOverlayVisible(false);
        }

        public void SetActiveScreen(BaseScreen screen)
        {
            if (ActiveScreen != null)
            {
                ActiveScreen.Hide();
            }
            ActiveScreen = screen;
            if (ActiveScreen != null)
            {
                ActiveScreen.Show();
            }
        }

        public void StartScreenTransition(BaseScreen toScreen, Action onComplete = null)
        {
            IsTransitioning = true;
            // TransitionImage.SetAlpha(0f);
            TransitionImage.gameObject.SetActive(true);

            var duration = TRANSITION_DURATION / 2f;
            var seq = DOTween.Sequence();
            seq.Append(TransitionImage.DOFade(1f, duration));
            seq.AppendCallback(() =>
            {
                SetActiveScreen(toScreen);
                onComplete?.Invoke();
            });
            seq.Append(TransitionImage.DOFade(0f, duration));
            seq.AppendCallback(() =>
            {
                IsTransitioning = false;
                TransitionImage.gameObject.SetActive(false);
            });
        }
        
        private void OnResumeButtonClicked()
        {
            GameController.ResumeGame();
        }
        
        private void OnExitButtonClicked()
        {
            GameController.ExitGame();
        }

        private void OnMuteButtonClicked()
        {
            AudioListener.volume = IsAudioMuted ? 1f : 0f;
            MuteButton.image.sprite = IsAudioMuted ? AudioOffSprite : AudioOnSprite;
        }
        
        public void SetPauseOverlayVisible(bool visible)
        {
            PauseOverlayView.SetActive(visible);
        }
    }
}
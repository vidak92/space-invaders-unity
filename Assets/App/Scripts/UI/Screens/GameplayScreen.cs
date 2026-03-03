using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceInvaders
{
    public class GameplayScreen : BaseScreen
    {
        public TMP_Text TitleText;
        public Button PauseButton;
        
        [Space]
        public StatItem ScoreItem;
        public StatItem WaveItem;
        public StatItem LivesItem;

        [Space]
        public TouchButton ShootButton;
        public TouchJoystick MoveJoystick;

        protected override void OnInit()
        {
            TitleText.text = Strings.GAME_TITLE;
            ScoreItem.LabelText.text = Strings.STAT_SCORE;
            ScoreItem.ValueText.text = "0";
            WaveItem.LabelText.text = Strings.STAT_WAVE;
            WaveItem.ValueText.text = "0";
            LivesItem.LabelText.text = Strings.STAT_LIVES;
            LivesItem.ValueText.text = "0";
            
            PauseButton.onClick.AddListener(OnPauseButtonClicked);

            ShootButton.OnPressed += OnShootButtonPressed;
            ShootButton.OnReleased += OnShootButtonReleased;
            
            var isMobilePlatform = AppController.IsMobilePlatform();
            ShootButton.gameObject.SetActive(isMobilePlatform);
            MoveJoystick.gameObject.SetActive(isMobilePlatform);
        }

        protected override void OnShow()
        {
            UIController.SetPauseOverlayVisible(false);
        }

        protected override void OnHide()
        {
            UIController.SetPauseOverlayVisible(false);
        }

        private void OnPauseButtonClicked()
        {
            GameController.PauseGame();
            AudioController.PlaySound(AudioController.ButtonPressSound);
        }

        private void OnShootButtonPressed()
        {
            InputService.SetInputAction(InputAction.Shoot, true);
        }
        
        private void OnShootButtonReleased()
        {
            InputService.SetInputAction(InputAction.Shoot, false);
        }

        public void SetGameStats(int score, int wave, int lives)
        {
            ScoreItem.ValueText.text = $"{score}";
            WaveItem.ValueText.text = $"{wave}";
            LivesItem.ValueText.text = $"{lives}";
        }
    }
}
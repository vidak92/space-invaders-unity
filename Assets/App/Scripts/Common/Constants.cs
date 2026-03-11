namespace SpaceInvaders
{
    public static class Constants
    {
        public const string TIMESTAMP_FORMAT = "yyyyMMddHHmmss";
        
        public const float CAMERA_ASPECT_MIN = 3f / 2f;
        public const float CAMERA_ASPECT_MAX = 16f / 9f;
        
        public const string VIGNETTE_PROPERTY_POWER = "_VignettePower";
        public const string VIGNETTE_PROPERTY_IS_ADDITIVE = "_IsAdditive";

        public const int DESTRUCTIBLE_QUAD_GRID_COL_COUNT = 9;
        public const int DESTRUCTIBLE_QUAD_GRID_ROW_COUNT = 4;
        public const int DESTRUCTIBLE_QUAD_GRID_FLAG_COUNT = DESTRUCTIBLE_QUAD_GRID_COL_COUNT * DESTRUCTIBLE_QUAD_GRID_ROW_COUNT;
        
        public const string DESTRUCTIBLE_QUAD_SHADER_PROPERTY_ROWS = "_Rows";
        public const string DESTRUCTIBLE_QUAD_SHADER_PROPERTY_COLS = "_Cols";
        public const string DESTRUCTIBLE_QUAD_SHADER_PROPERTY_FLAGS = "_Flags";
        public const string DESTRUCTIBLE_QUAD_SHADER_PROPERTY_MAIN_TEX_ST = "_MainTex_ST";
    }

    public static class SortingLayers
    {
        public const string DEFAULT = "Default";
        public const string BACKGROUND = "Background";
    }

    public static class Strings
    {
        public const string SCORE_INFO_PREFIX = ": ";
        public const string SCORE_INFO_SUFFIX = " POINTS";
        public const string SCORE_INFO_MYSTERY = ": ???????";

        public const string GAME_TITLE = "SPACE INVADERS";
        public const string BUTTON_PLAY = "PLAY";
        public const string BUTTON_BACK = "BACK";
        public const string BUTTON_EXIT = "EXIT";
        public const string BUTTON_RESUME = "RESUME";
        public const string BUTTON_HIGH_SCORES = "HIGH SCORES";
        public const string BUTTON_CONTROLS = "CONTROLS";
        
        public const string CONTROLS_TITLE = "CONTROLS";
        public const string CONTROLS_HEADER_ACTION = "ACTION";
        public const string CONTROLS_HEADER_PRIMARY_KEY = "PRIMARY";
        public const string CONTROLS_HEADER_SECONDARY_KEY = "SECONDARY";
        public const string CONTROLS_ACTION_MOVE_LEFT = "MOVE LEFT";
        public const string CONTROLS_ACTION_MOVE_RIGHT = "MOVE RIGHT";
        public const string CONTROLS_ACTION_SHOOT = "SHOOT";

        public const string STAT_DIVIDER = ":";
        public const string STAT_SCORE = "SCORE";
        public const string STAT_WAVE = "WAVE";
        public const string STAT_LIVES = "LIVES";
        
        public const string GAME_OVER_TITLE = "GAME OVER";
        public const string GAME_OVER_SCORE_PREFIX = "SCORE: ";
        public const string GAME_OVER_WAVE_PREFIX = "WAVE: ";

        public const string HIGH_SCORES_TITLE = "HIGH SCORES";
        public const string HIGH_SCORES_HEADER_RANK = "RANK";
        public const string HIGH_SCORES_HEADER_SCORE = "SCORE";
        public const string HIGH_SCORES_HEADER_WAVE = "WAVE";
    }

    public static class PlayerPrefsKeys
    {
        // public const int VERSION = 1;
        public const string HIGH_SCORES = "HIGH_SCORES";
    }
}
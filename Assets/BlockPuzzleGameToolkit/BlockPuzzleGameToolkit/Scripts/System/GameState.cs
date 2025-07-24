using System;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.System
{
    [Serializable]
    public class GameState
    {
        public EGameState gameStatus;
        public int currentLevel;
        public EGameMode gameMode;
        public int score;
        public int remainingTime;
        public LevelRow[] levelRows;
        public DateTime quitTime;
        public int bestScore;

        public static void Save(GameState state, FieldManager field)
        {
            // Copy current field state to levelRows
            if (state.levelRows == null)
            {
                if (field != null)
                {
                    var cells = field.GetAllCells();
                    state.levelRows = new LevelRow[cells.GetLength(0)];
                    for (var i = 0; i < cells.GetLength(0); i++)
                    {
                        state.levelRows[i] = new LevelRow(cells.GetLength(1));
                        for (var j = 0; j < cells.GetLength(1); j++) 
                        {
                            if (cells[i, j].item != null && !cells[i, j].IsEmpty())
                            {
                                state.levelRows[i].cells[j] = cells[i, j].item?.itemTemplate;
                                state.levelRows[i].bonusItems[j] = cells[i, j].HasBonusItem();
                                state.levelRows[i].disabled[j] = cells[i, j].IsDisabled();
                            }
                        }
                    }
                }
            }
            
            var json = JsonUtility.ToJson(state);
            PlayerPrefs.SetString("GameState", json);
            PlayerPrefs.Save();
        }

        public static GameState Load()
        {
            if (PlayerPrefs.HasKey("GameState"))
            {
                var json = PlayerPrefs.GetString("GameState");
                return JsonUtility.FromJson<GameState>(json);
            }
            return null;
        }

        public static void Delete()
        {
            PlayerPrefs.DeleteKey("GameState");
            PlayerPrefs.Save();
        }
    }
}

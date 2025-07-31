// // ©2015 - 2025 Candy Smith
// // All rights reserved
// // Redistribution of this software is strictly not allowed.
// // Copy of this software can be obtained from unity asset store only.
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.Audio;
using BlockPuzzleGameToolkit.Scripts.Data;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay.FX;
using BlockPuzzleGameToolkit.Scripts.Gameplay.Managers;
using BlockPuzzleGameToolkit.Scripts.Gameplay.Pool;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public partial class LevelManager : MonoBehaviour
    {
        public int currentLevel;
        public LineExplosion lineExplosionPrefab;
        public ComboText comboTextPrefab;
        public Transform pool;
        public Transform fxPool;

        private int comboCounter;
        private int missCounter;

        [SerializeField] private RectTransform gameCanvas;

        [SerializeField] private RectTransform shakeCanvas;

        [SerializeField] private GameObject scorePrefab;

        [SerializeField] private GameObject[] words;

        [SerializeField] private TutorialManager tutorialManager;

        private EGameMode gameMode;
        public Level _levelData;

        private Cell[] emptyCells;

        public UnityEvent<Level> OnLevelLoaded;
        public Action<int> OnScored;
        public Action OnLose;
        private FieldManager field;
        private CellDeckManager cellDeck;
        private ItemFactory itemFactory;
        private TargetManager targetManager;

        private ObjectPool<ComboText> comboTextPool;
        private ObjectPool<LineExplosion> lineExplosionPool;
        private ObjectPool<ScoreText> scoreTextPool;
        private ObjectPool<GameObject> wordsPool;
        private ClassicModeHandler classicModeHandler;

        [SerializeField] public int maxMove = 20;
        [SerializeField] private TextMeshProUGUI txtMoves;

        private void OnEnable()
        {
            StateManager.instance.CurrentState = EScreenStates.Game;
            EventManager.GetEvent(EGameEvent.RestartLevel).Subscribe(RestartLevel);
            EventManager.GetEvent<Shape>(EGameEvent.ShapePlaced).Subscribe(CheckLines);
            EventManager.OnGameStateChanged += HandleGameStateChange;
            targetManager = FindObjectOfType<TargetManager>();
            itemFactory = FindObjectOfType<ItemFactory>();
            cellDeck = FindObjectOfType<CellDeckManager>();
            field = FindObjectOfType<FieldManager>();


            comboTextPool = new ObjectPool<ComboText>(
                () => Instantiate(comboTextPrefab, fxPool),
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                Destroy
            );

            lineExplosionPool = new ObjectPool<LineExplosion>(
                () => Instantiate(lineExplosionPrefab, pool),
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                Destroy
            );

            scoreTextPool = new ObjectPool<ScoreText>(
                () => Instantiate(scorePrefab, fxPool).GetComponent<ScoreText>(),
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                Destroy
            );

            wordsPool = new ObjectPool<GameObject>(
                () => Instantiate(words[Random.Range(0, words.Length)], fxPool),
                obj => obj.SetActive(true),
                obj => obj.SetActive(false),
                Destroy
            );
            Load();
            if (gameMode == EGameMode.Classic)
                RestoreGameState();
        }

        private void RestoreGameState()
        {
            var state = GameState.Load();
            if (state != null)
            {
                GameManager.instance.Score = state.score;

                if (state.levelRows != null)
                {
                    var fieldManager = FindObjectOfType<FieldManager>();
                    if (fieldManager != null)
                    {
                        fieldManager.RestoreFromState(state.levelRows);
                    }
                }
            }
        }

        private void RestartLevel()
        {
            comboCounter = 0;
            missCounter = 0;
            field.ShowOutline(false);
            Load();
        }

        private void SaveGameState()
        {
            classicModeHandler = FindObjectOfType<ClassicModeHandler>();
            var state = new GameState
            {
                score = classicModeHandler.score,
                bestScore = classicModeHandler.bestScore
            };
            GameState.Save(state, field);
        }

        private void OnDisable()
        {
            EventManager.GetEvent(EGameEvent.RestartLevel).Unsubscribe(RestartLevel);
            EventManager.GetEvent<Shape>(EGameEvent.ShapePlaced).Unsubscribe(CheckLines);
            EventManager.OnGameStateChanged -= HandleGameStateChange;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (gameMode == EGameMode.Classic && EventManager.GameStatus == EGameState.Playing)
                SaveGameState();
        }

        private void OnApplicationQuit()
        {
            if (gameMode == EGameMode.Classic && EventManager.GameStatus == EGameState.Playing)
                SaveGameState();
        }

        private void Load()
        {
            if (GameManager.instance.IsTutorialMode())
            {
                _levelData = tutorialManager.GetLevelForPhase();
            }
            else
            {
                currentLevel = GameDataManager.GetLevelNum();
                gameMode = GameDataManager.GetGameMode();
                _levelData = GameDataManager.GetLevel();
            }

            if (_levelData == null)
            {
                Debug.LogError("Level data is null");
                return;
            }

            FindObjectsOfType<MonoBehaviour>().OfType<IBeforeLevelLoadable>().ToList()
                .ForEach(x => x.OnLevelLoaded(_levelData));
            LoadLevel(_levelData);
            FindObjectsOfType<MonoBehaviour>().OfType<ILevelLoadable>().ToList()
                .ForEach(x => x.OnLevelLoaded(_levelData));
            Invoke(nameof(StartGame), 0.5f);
            if (GameManager.instance.IsTutorialMode())
            {
                tutorialManager.StartTutorial();
            }
        }

        private void StartGame()
        {
            EventManager.GameStatus = EGameState.PrepareGame;
        }

        private void LoadLevel(Level levelData)
        {
            maxMove = levelData.maxMove;
            txtMoves.text = maxMove + "";
            field.Generate(levelData);
            EventManager.GetEvent<Level>(EGameEvent.LevelLoaded).Invoke(levelData);
            OnLevelLoaded?.Invoke(levelData);
        }

        private void CheckLines(Shape obj)
        {
            maxMove--;
            txtMoves.text = maxMove + "";

            var lines = field.GetFilledLines(false, false);
            if (lines.Count > 0)
            {
                comboCounter++;
                shakeCanvas.DOShakePosition(0.2f, 35f, 50);
                StartCoroutine(AfterMoveProcessing(obj, lines));
                if (comboCounter > 1)
                {
                    field.ShowOutline(true);
                    ShowComboText(comboCounter);
                }
            }
            else
            {
                missCounter++;
                if (missCounter >= GameManager.instance.GameSettings.ResetComboAfterMoves)
                {
                    field.ShowOutline(false);
                    missCounter = 0;
                    comboCounter = 0;
                }

                StartCoroutine(CheckLose());
            }
        }

        private void ShowComboText(int comboCount)
        {
            var position = Vector3.right * 1 + Vector3.up * 1;
            var comboText = comboTextPool.Get();
            comboText.transform.position = position;
            comboText.Show(comboCount);
            DOVirtual.DelayedCall(1.5f, () => { comboTextPool.Release(comboText); });
        }

        private IEnumerator AfterMoveProcessing(Shape shape, List<List<Cell>> lines)
        {
            var transformPosition = shape.transform.position;
            yield return new WaitForSeconds(0.1f);
            if (gameMode == EGameMode.Adventure)
            {
                StartCoroutine(targetManager.AnimateTarget(lines));
            }

            yield return StartCoroutine(DestroyLines(lines, shape));

            var scoreTarget = GameManager.instance.GameSettings.ScorePerLine * lines.Count * comboCounter;
            OnScored?.Invoke(scoreTarget);
            if (gameMode == EGameMode.Adventure)
            {
                targetManager.UpdateScoreTarget(scoreTarget);
            }

            var scoreText = scoreTextPool.Get();
            scoreText.transform.position = transformPosition;
            scoreText.ShowScore(scoreTarget, transformPosition);
            DOVirtual.DelayedCall(1.5f, () => { scoreTextPool.Release(scoreText); });

            if (Random.Range(0, 3) == 0)
            {
                var txt = wordsPool.Get();
                txt.transform.position = transformPosition + new Vector3(0, -0.5f, 0);

                // Ensure txt is within the bounds of the gameCanvas
                var canvasCorners = new Vector3[4];
                gameCanvas.GetWorldCorners(canvasCorners);

                var txtPosition = txt.transform.position;
                txtPosition.x = Mathf.Clamp(txtPosition.x, canvasCorners[0].x, canvasCorners[2].x);
                txtPosition.y = Mathf.Clamp(txtPosition.y, canvasCorners[0].y, canvasCorners[2].y);
                txt.transform.position = txtPosition;

                DOVirtual.DelayedCall(1.5f, () => { wordsPool.Release(txt); });
            }

            yield return StartCoroutine(CheckLose());
        }

        private IEnumerator CheckLose()
        {
            if (targetManager.IsLevelComplete() && gameMode != EGameMode.Classic)
            {
                EventManager.GameStatus = EGameState.WinWaiting;
            }

            yield return new WaitForSeconds(0.5f);
            var lose = true;
            var availableShapes = cellDeck.GetShapes();
            foreach (var shape in availableShapes)
            {
                if (field.CanPlaceShape(shape))
                {
                    lose = false;
                    break;
                }
            }

            if (gameMode != EGameMode.Classic && targetManager.IsLevelComplete())
            {
                yield return new WaitForSeconds(0.5f);
                SetWin();
                lose = false;
            }

            if (lose)
            {
                SetLose();
            }

            yield return null;

            if (EventManager.GameStatus == EGameState.Playing)
            {
                if (maxMove <= 0)
                {
                    txtMoves.text = "0";
                    SetLose();
                }
            }
        }

        private void SetWin()
        {
            GameDataManager.UnlockLevel(currentLevel + 1);
            EventManager.GameStatus = EGameState.PreWin;
        }

        private void SetLose()
        {
            if (gameMode == EGameMode.Classic)
                GameState.Delete();
            OnLose?.Invoke();
            EventManager.GameStatus = EGameState.PreFailed;
        }

        private IEnumerator EndAnimations(Action action)
        {
            yield return StartCoroutine(FillEmptyCellsFailed());
            action?.Invoke();
        }

        private IEnumerator FillEmptyCellsFailed()
        {
            SoundBase.instance.PlaySound(SoundBase.instance.fillEmpty);
            var template = Resources.Load<ItemTemplate>("Items/ItemTemplate 0");
            emptyCells = field.GetEmptyCells();
            foreach (var cell in emptyCells)
            {
                cell.FillCellFailed(template);
                yield return new WaitForSeconds(0.01f);
            }
        }

        private void ClearEmptyCells()
        {
            foreach (var cell in emptyCells)
            {
                cell.ClearCell();
            }
        }

        private IEnumerator DestroyLines(List<List<Cell>> lines, Shape shape)
        {
            SoundBase.instance.PlayLimitSound(
                SoundBase.instance.combo[Mathf.Min(comboCounter, SoundBase.instance.combo.Length - 1)]);
            EventManager.GetEvent<Shape>(EGameEvent.LineDestroyed).Invoke(shape);

            // Mark cells as destroying immediately at the start
            foreach (var line in lines)
            {
                foreach (var cell in line)
                {
                    cell.SetDestroying(true);
                }
            }

            foreach (var line in lines)
            {
                if (line.Count == 0) continue;

                var lineExplosion = lineExplosionPool.Get();
                lineExplosion.Play(line, shape,
                    RectTransformUtils.GetMinMaxAndSizeForCanvas(line, gameCanvas.GetComponent<Canvas>()),
                    GetExplosionColor(shape));
                DOVirtual.DelayedCall(1.5f, () => { lineExplosionPool.Release(lineExplosion); });
                foreach (var cell in line)
                {
                    cell.DestroyCell();
                }
            }

            yield return null;
        }

        private Color GetExplosionColor(Shape shape)
        {
            var itemTemplateTopColor = shape.GetActiveItems()[0].itemTemplate.topColor;
            if (_levelData.levelType.singleColorMode)
            {
                itemTemplateTopColor = itemFactory.GetOneColor().topColor;
            }

            return itemTemplateTopColor;
        }

        private void Update()
        {
            if (Keyboard.current != null)
            {
                // Debug keys for win/lose
                if (Keyboard.current[GameManager.instance.debugSettings.Win].wasPressedThisFrame)
                {
                    SetWin();
                }

                if (Keyboard.current[GameManager.instance.debugSettings.Lose].wasPressedThisFrame)
                {
                    SetLose();
                }

                // Other debug keys
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    // Destroy 0 row
                    var instanceCell = field.cells[0, 0];
                    instanceCell.FillCell(Resources.Load<ItemTemplate>("Items/ItemTemplate 0"));
                    instanceCell.SetBonus(Resources.Load<BonusItemTemplate>("BonusItems/BonusItemTemplate 0"));
                    var randomShape =
                        itemFactory.CreateRandomShape(null, PoolObject.GetObject(cellDeck.shapePrefab.gameObject));
                    randomShape.SetBonus(Resources.Load<BonusItemTemplate>("BonusItems/BonusItemTemplate 0"), 1);
                    shakeCanvas.DOShakePosition(0.2f, 35f, 50);
                    StartCoroutine(AfterMoveProcessing(randomShape, field.GetRow(0)));
                    Destroy(randomShape.gameObject);
                }

                // Use the configurable UpdateDeck key from debug settings instead of hardcoded dKey
                if (Keyboard.current[GameManager.instance.debugSettings.UpdateDeck].wasPressedThisFrame)
                {
                    cellDeck.ClearCellDecks();
                    cellDeck.FillCellDecks();
                }

                if (Keyboard.current.aKey.wasPressedThisFrame)
                {
                    StartCoroutine(CheckLose());
                }

                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    GameManager.instance.RestartLevel();
                }
            }
        }

        public Level GetCurrentLevel()
        {
            return _levelData;
        }

        public EGameMode GetGameMode()
        {
            return gameMode;
        }
    }
}
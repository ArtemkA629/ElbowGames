using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Framework.Variables;
using Framework.Utils;
using System;
using Framework.Events;

namespace ClashTheCube
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private DailyRewardPopupUIController _dailyRewardPopupUIController;
        [SerializeField] private BoolReference gamePaused;
        [SerializeField] private BoolReference isVibrationOn;

        [SerializeField] private FPSCounter fpsCounter;

        private const string lastExecutionKey = "LastExecutionDate";
        private DateTime lastExecutionDate;
        public GameEvent DailyRewardEvent;

        private void Start()
        {
            ResumeGame();
            CheckDailyReward();
            try
            {
                Vibration.Init();
            }
            catch { }
        }

        public void Vibrate()
        {
            if (!isVibrationOn)
            {
                return;
            }
            
            try
            {
                Vibration.VibratePop();
            }
            catch { }
        }

        public void PauseGame()
        {
            gamePaused.Variable.SetValue(true);
            UpdateGamePause();
        }

        public void ResumeGame()
        {
            gamePaused.Variable.SetValue(false);
            UpdateGamePause();
        }

        private void UpdateGamePause()
        {
            Time.timeScale = gamePaused ? 0 : 1;
        }

        public void ReloadScene()
        {
            ResumeGame();
            
            string scene = SceneManager.GetActiveScene().name;
            Initiate.Fade(scene, Color.black, 2f);
        }

        public void ToggleFps()
        {
            fpsCounter.enabled = !fpsCounter.enabled;
        }

       

        private void CheckDailyReward()
        {
            // Загрузка даты последнего выполнения из PlayerPrefs
            string savedDate = PlayerPrefs.GetString(lastExecutionKey);
            if (!string.IsNullOrEmpty(savedDate))
            {
                lastExecutionDate = DateTime.Parse(savedDate);
            }

            // Проверка, был ли скрипт выполнен сегодня
            if (!IsExecutedToday())
            {
                // Если нет, выполняем скрипт
                if (!DailyRewardEvent)
                {
                    return;
                }

                DailyRewardEvent.Raise();

                // Сохранение даты последнего выполнения в PlayerPrefs
                PlayerPrefs.SetString(lastExecutionKey, DateTime.Now.ToString());
                PlayerPrefs.Save();
            }
        }

        private bool IsExecutedToday()
        {
            // Проверяем, был ли скрипт выполнен сегодня
            return lastExecutionDate.Date == DateTime.Today;
        }
    }
}

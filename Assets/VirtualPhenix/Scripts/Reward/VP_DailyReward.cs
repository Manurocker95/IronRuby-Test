using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using VirtualPhenix.Localization;
using VirtualPhenix.Formatter;
using VirtualPhenix.Vibration;

#if DOTWEEN
using DG.Tweening;
#endif

namespace VirtualPhenix.Reward
{
    [DefaultExecutionOrder(VP_ExecutingOrderSetup.DAILY_REWARD), AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX + "/Reward/Daily Reward")]
    public class VP_DailyReward : VP_Vibrator, VP_ITranslate
    {
        public enum RewardShowUp
        {
            None,
            Alert,
            CoinsMoving
        }

        public enum SaveMethod
        {
            PlayerPrefs,
            External
        }
        [Tooltip("Formatter"), Space]
        [SerializeField] protected FORMATTER_METHOD m_formatter = FORMATTER_METHOD.JSONUTILITY_RESOLVER;

        [Header("How reward is displayed"), Space]
        [SerializeField] protected RewardShowUp m_rewardShowUp;

        [Header("How reward is saved"), Space]
        [SerializeField] protected SaveMethod m_saveMethod = SaveMethod.External;

        [Header("Check to disable Daily Reward Feature"), Space]
        [SerializeField] protected bool m_disableDailyReward = false;

        [Header("Daily Reward Config"), Space]
        [Tooltip("Number of hours between 2 rewards")]
        [SerializeField] protected int m_rewardIntervalHours = 6;

        [Tooltip("Number of minues between 2 rewards")]
        [SerializeField] protected int m_rewardIntervalMinutes = 0;

        [Tooltip("Number of seconds between 2 rewards")]
        [SerializeField] protected int m_rewardIntervalSeconds = 0;
        [SerializeField] protected int m_minRewardValue = 3;
        [SerializeField] protected int m_maxRewardValue = 5;

        [Tooltip("Reward Text"), Space]
        [SerializeField] protected Button m_dailyRewardBtn;
        [SerializeField] protected TMP_Text m_dailyRewardBtnText;
        [SerializeField] protected string m_playerPrefKey = "CoinPPK";

        [Tooltip("Audios"), Space]
        [SerializeField] protected AudioClip m_dailyRewardAudio;
        [SerializeField] private AudioClip m_coinSound;

        [Tooltip("Coins"), Space]
        [SerializeField] private Transform[] m_rewardCoins;
        [SerializeField] private List<Vector3> m_rewardCoinsOriginalPos;
        [SerializeField] private Transform m_rewardPointEndRef;

        [Header("Button Audio"), Space]
        [SerializeField] protected AudioClip m_buttonClip;

        [Header("Debug"), Space]
        [SerializeField] protected string m_grabDaily = "";
        [SerializeField] protected string m_rewardIn = "";

        [SerializeField] protected bool m_rewardActive = true;
        [SerializeField] protected string m_lastSavedRewardTime;

        [Header("Event"), Space]
        public UnityEvent<int> OnShowRewardUI;

        public virtual System.TimeSpan TimeUntilReward
        {
            get
            {
                return GetNextRewardTime().Subtract(System.DateTime.Now);
            }
        }

        public virtual void TranslateTexts()
        {
            var lm = VP_LocalizationManager.Instance;
            m_grabDaily = lm.GetTextTranslated(VP_TextSetup.Menu.GRAB_DAILY);
            m_rewardIn = lm.GetTextTranslated(VP_TextSetup.Menu.REWARD_IN);
        }


        protected override void Initialize()
        {
            base.Initialize();

            VP_Debug.Log("Init Daily Reward: ");

            TranslateTexts();

            if (m_saveMethod == SaveMethod.PlayerPrefs)
            {
	            LoadRewardString(VP_Formatter.LoadFromPlayerPrefs(m_playerPrefKey, out string data, (bool _loaded) =>
                {
                    VP_Debug.Log("Loaded reward: " + _loaded);

                }));
            }
        }

 
        protected virtual void Update()
        {
            CheckReward();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftShift))
            {
                m_disableDailyReward = !m_disableDailyReward;
            }

            if (Input.GetKeyDown(KeyCode.N) && Input.GetKey(KeyCode.LeftShift))
            {
                ResetReward();
            }
#endif
        }

        public virtual void LoadRewardString(string _stored)
        {
            m_lastSavedRewardTime = _stored;

            CheckReward();

            m_dailyRewardBtn.interactable = m_rewardActive;
        }

        public virtual void CheckReward()
        {
            if (!m_disableDailyReward)
            {
                bool canReward = CanRewardNow();
                if (m_rewardActive && !canReward)
                {
                    m_rewardActive = false;
                    m_dailyRewardBtn.interactable = false;
                }
                else if (!m_rewardActive && canReward)
                {
                    m_rewardActive = true;
                    m_dailyRewardBtnText.text = m_grabDaily;
                    m_dailyRewardBtn.interactable = true;
                }
                else if (!m_rewardActive && !canReward)
                {
                    System.TimeSpan timeToReward = TimeUntilReward;
                    m_dailyRewardBtnText.text = m_rewardIn + string.Format("{0:00}:{1:00}:{2:00}", timeToReward.Hours, timeToReward.Minutes, timeToReward.Seconds);
                    //dailyRewardAnimator.SetTrigger("deactivate");
                }

            }
            else
            {
                if (m_rewardActive)
                {
                    m_rewardActive = false;
                    m_dailyRewardBtn.interactable = false;
                }
            }
        }


        public virtual void PlaySoundButton()
        {
            // Play button sound
            if (m_buttonClip)
                VP_AudioManager.PlayOneShot(m_buttonClip);
        }


        public virtual void PlayCoinSound()
        {
            if (m_coinSound)
                VP_AudioManager.PlayOneShot(m_coinSound);
        }

        public virtual void GrabDailyReward()
        {
            if (CanRewardNow())
            {
                PlaySoundButton();

                VP_AudioManager.PlayOneShot(m_dailyRewardAudio);

                int c = m_rewardCoins.Length;
                if (c > 0)
                {
                    int reward = Mathf.RoundToInt(GetRandomReward() / c);
                    if (m_rewardShowUp == RewardShowUp.CoinsMoving)
                    {
                        int idx = 0;
                        foreach (Transform t in m_rewardCoins)
                        {
                            t.position = m_rewardCoinsOriginalPos[idx];
                            t.gameObject.SetActive(true);
                            idx++;
                        }


                        WaitTime(.2f, () =>
                        {
                            foreach (Transform t in m_rewardCoins)
                            {
#if DOTWEEN
                                t.DOMove(m_rewardPointEndRef.position, Randf(.6f, .8f)).OnComplete(() =>
                                {
                                    ShowRewardUI(reward);
                                    t.gameObject.SetActive(false);
                                });
#else
                                ShowRewardUI(reward);
                                t.gameObject.SetActive(false);
#endif
                            }
                        });
                    }
                    else
                    {
                        ShowRewardUI(GetRandomReward());
                    }
                }
                else
                {
                    // Show the reward UI
                    ShowRewardUI(GetRandomReward());
                }

                // Update next time for the reward
                ResetNextRewardTime();


            }
        }


        public virtual void ResetReward()
        {
            StoreNextRewardTime(System.DateTime.Now);
        }

        public virtual void ShowRewardUI(int reward)
        {
            //PlayCoinSound();
            if (m_rewardShowUp == RewardShowUp.Alert)
            {
                VP_AlertManager.Alert(this.INTL("You gained <color=red>{0}</color> coins!", reward.ToString()), "Ok", () =>
                {
                    // Add coins-> Do Tween?
                    OnShowRewardUI.Invoke(reward);
                }, true, -1, false, true);
            }
            else
            {
                OnShowRewardUI.Invoke(reward);
            }
        }

        public virtual void UpdateCoinText(int _coins)
        {

        }

        /// <summary>
        /// Determines whether the waiting time has passed and can reward now.
        /// </summary>
        /// <returns><c>true</c> if this instance can reward now; otherwise, <c>false</c>.</returns>
        public virtual bool CanRewardNow()
        {
            return TimeUntilReward <= System.TimeSpan.Zero;
        }

        /// <summary>
        /// Gets the random reward.
        /// </summary>
        /// <returns>The random reward.</returns>
        public virtual int GetRandomReward()
        {
            return UnityEngine.Random.Range(m_minRewardValue, m_maxRewardValue + 1);
        }

        /// <summary>
        /// Set the next reward time to some time in future determined by the predefined number of hours, minutes and seconds.
        /// </summary>
        public virtual void ResetNextRewardTime()
        {
            System.DateTime next = System.DateTime.Now.Add(new System.TimeSpan(m_rewardIntervalHours, m_rewardIntervalMinutes, m_rewardIntervalSeconds));
            StoreNextRewardTime(next);
        }

        protected virtual void StoreNextRewardTime(System.DateTime _time)
        {
            m_lastSavedRewardTime = _time.ToBinary().ToString();

            VP_Debug.Log("Last Saved " + m_lastSavedRewardTime);

            if (m_saveMethod == SaveMethod.PlayerPrefs)
                VP_Formatter.SaveToPlayerPrefs(m_lastSavedRewardTime, m_playerPrefKey, null);
            else
                VP_EventManager.TriggerEvent(VP_EventSetup.Reward.STORE_NEXT_REWARD_TIME, m_lastSavedRewardTime);
        }

        protected virtual System.DateTime GetNextRewardTime()
        {
            if (!string.IsNullOrEmpty(m_lastSavedRewardTime))
                return System.DateTime.FromBinary(System.Convert.ToInt64(m_lastSavedRewardTime));
            else
                return System.DateTime.Now;
        }
    }
}


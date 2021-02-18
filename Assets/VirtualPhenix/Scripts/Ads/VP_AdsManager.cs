using UnityEngine;
using System.Collections;

#if USE_MORE_EFFECTIVE_COROUTINES
using MEC;
#endif

using UnityEngine.Events;
using System.Collections.Generic;


#if EASY_MOBILE
using EasyMobile;
using UnityEngine.Advertisements;
#endif

namespace VirtualPhenix
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.ADS_MANAGER),AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Ads/Ads Manager")]
    public class VP_AdsManager : VP_SingletonMonobehaviour<VP_AdsManager>
    {
        public enum BannerAdPos
        {
            Top,
            Bottom,
            Complete
        }

        [Header("Banner Ad Display Config"), Space]
        [Tooltip("Whether or not to show banner ad")]
        [SerializeField] protected string m_gameID = "3717527";
        [SerializeField] protected string m_bannerID = "BottomBanner";
        [SerializeField] protected bool m_manualTestMode = false;
        [SerializeField] protected bool m_showBannerAd = true;
        [SerializeField] protected bool m_showBannerManually = false;
        [SerializeField] protected BannerAdPos m_bannerAdPosition = BannerAdPos.Bottom;
#if USE_MONETIZATION
        [SerializeField] protected UnityEngine.Advertisements.BannerPosition m_bannerUnityAdPosition = BannerPosition.BOTTOM_CENTER;
#endif
        [Header("Interstitial Ad Display Config"), Space]
        [Tooltip("Whether or not to show interstitial ad")]
        [SerializeField] protected bool m_showInterstitialAd = false;

        [Tooltip("Show interstitial ad every [how many] games")]
        [SerializeField] protected int m_gamesPerInterstitial = 3;
        [Tooltip("How many seconds after game over that interstitial ad is shown")]
        [SerializeField] protected float m_showInterstitialDelay = 2f;

        [Header("Rewarded Ad Display Config")]
        [Tooltip("Check to allow watching ad to earn coins")]
        [SerializeField] protected bool m_watchAdToEarnCoins = true;
        [Tooltip("How many coins the user earns after watching a rewarded ad")]
        [SerializeField] protected int m_rewardedCoins = 5;

        [Header("Events"),Space]
        public UnityEvent CompleteRewardedAdToRecoverLostGame;
        public UnityEvent CompleteRewardedAdToEarnCoins;

        protected int m_gameCount = 0;
        protected bool m_shownBannerAd = false;
        protected bool m_needToShoBannerAd = false;

        public virtual int RewardedCoinsPerAdd { get { return m_rewardedCoins; } }
        public virtual bool WatchAdToEarnCoins { get { return m_watchAdToEarnCoins; } }
        public virtual bool AreAdsRemoved
        {
            get
            {
#if EASY_MOBILE
                return Advertising.IsAdRemoved();
#else
                return false;
#endif
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
        
        }

        protected override void OnDisable()
        {
            base.OnDisable();

        }

        protected override void Reset()
        {
            base.Reset();

            m_gameCount = 0;
            m_gamesPerInterstitial = 3;
            m_showInterstitialDelay = 2f;
            m_showInterstitialAd = false;
        }

        protected override void Initialize()
        {
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR
            this.gameObject.SetActive(false);
#else
	        base.Initialize();

#if EASY_MOBILE
	        if (!RuntimeManager.IsInitialized())
	        RuntimeManager.Init();
#endif      
#endif
        }

        protected override void Start()
        {
            m_shownBannerAd = false;
            //ShowBannerAdDown();
        }

        protected virtual void Update()
        {

        }

        public virtual void ShowBannerAd()
        {
#if EASY_MOBILE
            // Show banner ad
            if (!Advertising.IsAdRemoved() && m_showBannerAd && m_bannerAdPosition != BannerAdPos.Complete)
            {
                m_shownBannerAd = true;
                if (!m_showBannerManually)
                {

                    Advertising.ShowBannerAd(m_bannerAdPosition == BannerAdPos.Top ? BannerAdPosition.Top : BannerAdPosition.Bottom);
                }
                else
                {
#if USE_MONETIZATION
                    Advertisement.Initialize(m_gameID, m_manualTestMode);
                    StartCoroutine(ShowBannerWhenInitialized());
#endif
                }

            }
#endif
        }

#if EASY_MOBILE
        public virtual void ShowBannerAd(BannerAdPosition _position)
        {

            // Show banner ad
            if (!Advertising.IsAdRemoved() && m_showBannerAd)
            {
                m_shownBannerAd = true;
                if (!m_showBannerManually)
                {

                    Advertising.ShowBannerAd(_position);
                }
                else
                {
#if USE_MONETIZATION
                    Advertisement.Initialize(m_gameID, m_manualTestMode);
                    StartCoroutine(ShowBannerWhenInitialized());
#endif
                }

            }

        }
#endif
        public virtual void ShowBannerAdDown()
        {
#if EASY_MOBILE
            if (!Advertising.IsAdRemoved() && m_showBannerAd)
            {
                m_shownBannerAd = true;
                if (m_showBannerManually)
                {
#if USE_MONETIZATION
                    Advertisement.Initialize(m_gameID, m_manualTestMode);
                    StartCoroutine(ShowBannerWhenInitialized());
#else

                    Advertising.ShowBannerAd(BannerAdPosition.Bottom);
#endif
                }
                else
                {

                    Advertising.ShowBannerAd(BannerAdPosition.Bottom);
                }

            }
#endif
        }

#if USE_MONETIZATION
        protected virtual IEnumerator ShowBannerWhenInitialized()
        {
            while (!Advertisement.isInitialized)
            {
                yield return new WaitForSeconds(0.5f);
            }
            Advertisement.Banner.SetPosition(m_bannerUnityAdPosition);
            Advertisement.Banner.Show(m_bannerID);
        }
#endif
#if EASY_MOBILE
        public virtual void AddToCounter(System.Action<InterstitialAdNetwork, AdPlacement> _callback = null)
        {

            m_gameCount++;

            if (m_gameCount >= m_gamesPerInterstitial)
            {
                ShowInterstitialAd(_callback);
            }

    }

        public virtual void ShowInterstitialAd(System.Action<InterstitialAdNetwork, AdPlacement> _callback = null, System.Action _fallback = null)
        {
            if (VP_Utils.HasInternetConnection())
            {
                if (!Advertising.IsAdRemoved() && Advertising.IsInterstitialAdReady())
                {
#if USE_MORE_EFFECTIVE_COROUTINES
                    // Show default ad after some optional delay
                    Timing.RunCoroutine(ShowInterstitial(m_showInterstitialDelay, _callback));
#else
                    StartCoroutine(ShowInterstitial(m_showInterstitialDelay, _callback));
#endif
                    // Reset game count
                    m_gameCount = 0;
                }
                else
                {
                    if (_callback != null)
                    {
                        _callback.Invoke(InterstitialAdNetwork.UnityAds, AdPlacement.GameOver);
                    }
                }
            }
            else
            {
                if (_fallback != null)
                    _fallback.Invoke();
            }
        }
#endif
    /// <summary>
    ///  Purchase of Remove Ads. Called from VP_InAppManager
    /// </summary>
    public virtual void RemoveAds()
        {
#if EASY_MOBILE

            if (m_showBannerManually)
            {
#if USE_MONETIZATION
                Advertisement.Banner.Hide();
#else
                Advertising.HideBannerAd();
#endif
            }
            else
            {
                Advertising.HideBannerAd();
            }

            Advertising.RemoveAds();
#endif
        }

#if EASY_MOBILE
        public virtual void ShowInterstitialAdWhenReady(System.Action<InterstitialAdNetwork, AdPlacement> _callback = null)
        {
            if (Advertising.IsAdRemoved() || !VP_Utils.HasInternetConnection())
            {
                if (_callback != null)
                {
                    _callback.Invoke(InterstitialAdNetwork.UnityAds, AdPlacement.GameOver);
                }

                return;
            }

#if USE_MORE_EFFECTIVE_COROUTINES
            Timing.RunCoroutine(WaitForInterstitial(_callback));
#else
            StartCoroutine(WaitForInterstitial(_callback));
#endif
        }

        public virtual void StopListeningToInterstitialAdCompleted(System.Action<InterstitialAdNetwork, AdPlacement> _callback = null)
        {

            Advertising.InterstitialAdCompleted -= _callback;
        }
#endif

#if EASY_MOBILE && USE_MORE_EFFECTIVE_COROUTINES
        protected virtual IEnumerator<float> WaitForInterstitial(System.Action<InterstitialAdNetwork, AdPlacement> _callback = null)
        {
      
            float fallback = 0;
            float maxWait = VP_AppInfo.MAX_WAIT_FOR_AD;
            while (!Advertising.IsInterstitialAdReady() && fallback < maxWait)
            {
                fallback += Time.deltaTime;
                yield return Timing.WaitForOneFrame;
            }

            if (fallback < maxWait)
            {
                VP_Debug.Log("Show add", DEBUG_COLOR.RED);
                Advertising.ShowInterstitialAd();

                if (_callback != null)
                    Advertising.InterstitialAdCompleted += _callback;
            }
        }
#elif EASY_MOBILE && !USE_MORE_EFFECTIVE_COROUTINES
        protected virtual IEnumerator WaitForInterstitial(System.Action<InterstitialAdNetwork, AdPlacement> _callback = null)
        {
            float fallback = 0;
            float maxWait = VP_AppInfo.MAX_WAIT_FOR_AD;
            while (!Advertising.IsInterstitialAdReady() && fallback < maxWait)
            {
                fallback += Time.deltaTime;

                yield return null;
            }

            if (fallback < maxWait)
            {
                VP_Debug.Log("Show add", DEBUG_COLOR.RED);
                Advertising.ShowInterstitialAd();

                if (_callback != null)
                    Advertising.InterstitialAdCompleted += _callback;

            }
        }
#endif
#if EASY_MOBILE
#if USE_MORE_EFFECTIVE_COROUTINES
        protected virtual IEnumerator<float> ShowInterstitial(float delay = 0f, System.Action<InterstitialAdNetwork, AdPlacement> _callback = null)
        {
            if (delay > 0)
                yield return Timing.WaitForSeconds(delay);


            Advertising.ShowInterstitialAd();
            if (_callback != null)
                Advertising.InterstitialAdCompleted += _callback;
        }
#else
#if EASY_MOBILE
        protected virtual IEnumerator ShowInterstitial(float delay = 0f,System.Action<InterstitialAdNetwork, AdPlacement> _callback = null)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);


            Advertising.ShowInterstitialAd();
            if (_callback != null)
                Advertising.InterstitialAdCompleted += _callback;

         }
#endif
#endif
#endif
        public virtual bool CanShowRewardedAd()
        {
#if EASY_MOBILE
            return m_watchAdToEarnCoins && Advertising.IsRewardedAdReady();
#else
            return false;
#endif
        }

#if EASY_MOBILE
        public virtual void ShowRewardedAdToRecoverLostGame(System.Action<RewardedAdNetwork, AdPlacement> _callback = null)
        {
            if (CanShowRewardedAd())
            {

                Advertising.RewardedAdCompleted += OnCompleteRewardedAdToRecoverLostGame;

                if (_callback != null)
                {
                    Advertising.RewardedAdCompleted += _callback;
                }

                Advertising.ShowRewardedAd();

            }
            else
            {
                if (_callback != null)
                {
                    _callback.Invoke(RewardedAdNetwork.None, AdPlacement.GameOver);
                }
            }
        }

        protected virtual void OnCompleteRewardedAdToRecoverLostGame(RewardedAdNetwork adNetwork, AdPlacement location)
        {

            // Unsubscribe
            Advertising.RewardedAdCompleted -= OnCompleteRewardedAdToRecoverLostGame;

            // Fire event
            if (CompleteRewardedAdToRecoverLostGame != null)
            {
                this.CompleteRewardedAdToRecoverLostGame.Invoke();
            }
        }

        public virtual void ShowRewardedAdToEarnCoins(UnityAction _regularCallback, UnityAction _fallback, System.Action<RewardedAdNetwork, AdPlacement> _callback = null)
        {
            if (CanShowRewardedAd())
            {
                CompleteRewardedAdToEarnCoins.AddListener(_regularCallback);

#if EASY_MOBILE
                Advertising.RewardedAdCompleted += OnCompleteRewardedAdToEarnCoins;
                if (_callback != null)
                {
                    _callback.Invoke(RewardedAdNetwork.None, AdPlacement.Achievements);
                }
                Advertising.ShowRewardedAd();
#endif
            }
            else
            {
                if (_fallback != null)
                    _fallback.Invoke();
            }
        }

        public virtual void ShowRewardedAdToEarnCoins(System.Action<RewardedAdNetwork, AdPlacement> _callback = null)
        {
            if (CanShowRewardedAd())
            {
                Advertising.RewardedAdCompleted += OnCompleteRewardedAdToEarnCoins;
                if (_callback != null)
                {
                    _callback.Invoke(RewardedAdNetwork.None, AdPlacement.Achievements);
                }
                Advertising.ShowRewardedAd();
            }
        }

        protected virtual void OnCompleteRewardedAdToEarnCoins(RewardedAdNetwork adNetwork, AdPlacement location)
        {

            // Unsubscribe
            Advertising.RewardedAdCompleted -= OnCompleteRewardedAdToEarnCoins;

            // Fire event
            if (CompleteRewardedAdToEarnCoins != null)
            {
                this.CompleteRewardedAdToEarnCoins.Invoke();
            }
        }
#endif
    }
}

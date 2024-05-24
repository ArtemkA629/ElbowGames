using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;
using Framework.Events;
using Framework.SystemInfo;
using System;

namespace ClashTheCube
{
    public class AdvertisingController : MonoBehaviour
    {
        private static string AddAbilityPlacement = "Add_Ability";

        [SerializeField] private GameEvent adCompletedEvent;
        [SerializeField] private GameEvent abilityAdCompletedEvent;
        [SerializeField] private GameEvent adSkippedEvent;
        [SerializeField] private GameEvent adNotReadyEvent;

        [SerializeField] private GameObject removeAdsButton;
        [SerializeField] private GameObject continueGameRewardedButton;
        [SerializeField] private GameObject continueGameInfoText;
        [SerializeField] private GameObject continueGameText;

        //private void Start()
        //{
        //    //RefreshADs();
        //}

        private void OnEnable()
        {
            //Advertising.InterstitialAdCompleted += InterstitialAdCompletedHandler;
            //Advertising.RewardedAdCompletedAndRewardReceived += RewardedAdCompletedHandler;
            //Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;
        }

        private void OnDisable()
        {
            //Advertising.InterstitialAdCompleted -= InterstitialAdCompletedHandler;
            // Advertising.RewardedAdCompletedAndRewardReceived -= RewardedAdCompletedHandler;
            //Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;
        }



        private bool IsADsSupported()
        {
            return Platform.IsMobilePlatform();
        }

        public void RefreshADs()
        {
            if (!IsADsSupported())
            {
                removeAdsButton.SetActive(false);

                continueGameRewardedButton.SetActive(false);
                continueGameText.SetActive(false);
                continueGameInfoText.SetActive(false);
                return;
            }

            bool isADsVisible = true;

            if (Advertising.IsAdRemoved())
            {
                isADsVisible = false;
            }
            else if (InAppPurchaseController.Instance.IsRemoveADsProductOwned())
            {
                Advertising.RemoveAds();
                isADsVisible = false;
            }

            removeAdsButton.SetActive(isADsVisible);
        }

        public void ResetRemoveADs()
        {
            Advertising.ResetRemoveAds();
            removeAdsButton.SetActive(true);
        }

        public void ShowAchievementAD()
        {

            if (!IsADsSupported())
            {
                if (adCompletedEvent)
                {
                    adCompletedEvent.Raise();
                }
                return;
            }
            //if (!Advertising.IsInterstitialAdReady())
            //{
            //    if (adNotReadyEvent)
            //    {
            //        adNotReadyEvent.Raise();
            //    }
            //    return;
            //}

            ShowInter();
        }

        public void ShowContinueGameAD()
        {

            if (!IsADsSupported())
            {
                if (adCompletedEvent)
                {
                    adCompletedEvent.Raise();
                }
                return;
            }
            if (!Advertising.IsRewardedAdReady())
            {
                if (adNotReadyEvent)
                {
                    adNotReadyEvent.Raise();
                }
                return;
            }

            Advertising.ShowRewardedAd();
        }

        public void ShowAddAbilityGameAD()
        {

            if (!IsADsSupported())
            {
                if (abilityAdCompletedEvent)
                {
                    abilityAdCompletedEvent.Raise();
                }
                return;
            }
            //if (!Advertising.IsRewardedAdReady())
            //{
            //    if (adNotReadyEvent)
            //    {
            //        adNotReadyEvent.Raise();
            //    }
            //    return;
            //}

            //Advertising.ShowRewardedAd(AdPlacement.PlacementWithName(AddAbilityPlacement));
            ShowRewarded((res) =>
            {
                if (res)
                {
                    AddAbility();
                }
            });
        }

        private void InterstitialAdCompletedHandler(InterstitialAdNetwork network, AdPlacement placement)
        {
            Debug.Log("Interstitial ad has been closed.");

            AdCompletedHandler(placement);
        }

        private void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement placement)
        {
            Debug.Log("Rewarded ad has completed. The user should be rewarded now.");

            AdCompletedHandler(placement);
        }

        private void AdCompletedHandler(AdPlacement placement)
        {
            GameEvent gameEvent = adCompletedEvent;

            if (placement == AdPlacement.PlacementWithName(AddAbilityPlacement))
            {
                gameEvent = abilityAdCompletedEvent;
            }

            if (gameEvent)
            {
                gameEvent.Raise();
            }
        }

        private void AddAbility()
        {

            var gameEvent = abilityAdCompletedEvent;

            if (gameEvent)
            {
                gameEvent.Raise();
            }
        }

        private void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement placement)
        {
            Debug.Log("Rewarded ad was skipped. The user should NOT be rewarded.");

            if (adSkippedEvent)
            {
                adSkippedEvent.Raise();
            }
        }




        #region IronSourse



#if UNITY_ANDROID
        string appKey = "1e89e325d";
#elif UNITY_IPHONE
        string appKey = "unexpected_platform";
#else
    string appKey = "unexpected_platform";
#endif
        [SerializeField] private float InterShowInterval = 180f; // Интервал между показами межстраничного объявления в секундах

        [SerializeField] GameObject Question;
        [SerializeField] GameObject Congretulate;
        private bool _ready;

        private Action<bool> _rewardProgress;

        public bool RewardedIsAvailable
        {
            get
            {
                if (!_ready) return false;
                bool available = IronSource.Agent.isRewardedVideoAvailable();
                if (!available) IronSource.Agent.loadRewardedVideo();
                return available;
            }
        }
        public bool InterIsAvailable
        {
            get
            {
                if (!_ready) return false;
                bool available = IronSource.Agent.isInterstitialReady();
                if (!available) IronSource.Agent.loadInterstitial();
                return available;
            }
        }

        void Start()
        {
            IronSource.Agent.validateIntegration();
            _ready = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
#else
            SdkInitializationCompletedEvent();
#endif
            IronSource.Agent.init(appKey);

            
        }


        public void ShowInter()
        {
#if UNITY_EDITOR
            Debug.Log("Showed inter. EDITOR");
#endif
            Debug.Log("Showed inter. EDITOR");
            if (!_ready) return;
            if (InterIsAvailable)
            {
                IronSource.Agent.showInterstitial();
            }
            StartCoroutine(ShowInterCoroutine());
        }

        private IEnumerator ShowInterCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(InterShowInterval);

                Debug.Log("ShowInterCoroutine");

                ShowInter();
                Debug.Log("Show Inter addddddddd");
            }
        }

        public bool ShowRewarded(Action<bool> onComplete)
        {
#if UNITY_EDITOR
            onComplete.Invoke(true);
            return true;
#elif UNITY_ANDROID

        _rewardProgress = null;

        if (!_ready) return false;
        if (RewardedIsAvailable)
        {
            IronSource.Agent.showRewardedVideo();
            _rewardProgress = onComplete;
            return true;
        }
        return false;
#endif
        }

        // Indicates that there’s an available ad.
        // The adInfo object includes information about the ad that was loaded successfully
        // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
        private void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
        {

        }
        // Indicates that no ads are available to be displayed
        // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
        private void RewardedVideoOnAdUnavailable()
        {

        }
        // The Rewarded Video ad view has opened. Your activity will loose focus.
        private void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {

        }
        // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
        private void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            IronSource.Agent.loadRewardedVideo();
        }
        // The user completed to watch the video, and should be rewarded.
        // The placement parameter will include the reward data.
        // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
        private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            _rewardProgress.Invoke(true);
            _rewardProgress = null;
            IronSource.Agent.loadRewardedVideo();
        }
        // The rewarded video ad was failed to show.
        private void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
        {
            _rewardProgress.Invoke(false);
            _rewardProgress = null;
            IronSource.Agent.loadRewardedVideo();
        }
        // Invoked when the video ad was clicked.
        // This callback is not supported by all networks, and we recommend using it only if
        // it’s supported by all networks you included in your build.
        private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {

        }

        private void InterstitialVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            IronSource.Agent.loadInterstitial();
            Debug.Log("InterstitialVideoOnAd     ClosedEvent");
        }




        private void SdkInitializationCompletedEvent()
        {
            IronSource.Agent.loadInterstitial();
            IronSource.Agent.loadRewardedVideo();
            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialVideoOnAdClosedEvent;
            _ready = true;
            Debug.Log("sdk initilized");
        }

        private void OnApplicationPause(bool pause)
        {
            IronSource.Agent.onApplicationPause(pause);
        }
        #endregion

    }
}

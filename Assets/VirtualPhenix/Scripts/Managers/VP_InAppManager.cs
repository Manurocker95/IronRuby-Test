using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Localization;
using VirtualPhenix.Vibration;
using TMPro;
using UnityEngine.Events;
#if DOTWEEN
using DG.Tweening;
#endif
using VirtualPhenix.Fade;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace VirtualPhenix.InApp
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.INAPP_MANAGER)]
    /// <summary>
    /// In App Manager
    /// </summary>
    /// <typeparam name="T">Pack</typeparam>
    /// <typeparam name="T0">pack<T></typeparam>
    /// <typeparam name="T1">Singleton instance (child)</typeparam>
    public class VP_InAppManager<T, T0, T1> : VP_VibratorSingleton<T1>, VP_ITranslate where T : VP_InAppPack<T0> where T1 : VP_Monobehaviour
    {
        public enum ShopAppearType
        {
            None,
            DoTween,
            Fade,
            Custom
        }

        [Header("Properties"), Space]
        /// <summary>
        /// Packs 
        /// </summary>
        [SerializeField] protected bool m_canRestorePurchases;
        public bool m_canInteract = true;

        [Header("Shop"), Space]
        [SerializeField] protected bool m_translateCoinTexts = true;
        /// <summary>
        /// Packs 
        /// </summary>
        [SerializeField] protected TMP_Text[] m_coinTexts;
        [SerializeField] protected TMP_Text[] m_valueTexts;
        [SerializeField] protected TMP_Text m_shopTitleText;
        [SerializeField] protected GameObject m_group;
        [SerializeField] protected Transform m_panel;
        [SerializeField] protected VP_FadePostProcess m_fadePostProcess;
        [SerializeField] protected float m_fadeDuration = .5f;
        [SerializeField] protected ShopAppearType m_appearType;

        [Header("Packs"), Space]
        /// <summary>
        /// Packs 
        /// </summary>
        [SerializeField] protected List<T> m_packs = new List<T>();
        /// <summary>
        /// Properties
        /// </summary>
        public virtual List<T> Packs { get { return m_packs; } }

        /// <summary>
        /// Callback when purchase is completed
        /// </summary>
        public UnityEvent<T> m_OnPurchaseComplete;
        /// <summary>
        /// Callback when purchase is failed
        /// </summary>
        public UnityEvent<T> m_OnPurchaseFailed;
        public UnityEvent<bool> m_OnShowPanel;


        public virtual void TriggerFailEvent(T _pack)
        {
            m_OnPurchaseFailed.Invoke(_pack);
        }

        public virtual void TriggerCompleteEvent(T _pack)
        {
            m_OnPurchaseComplete.Invoke(_pack);
        }

        protected override void Initialize()
        {
            base.Initialize();
            TranslateTexts();
            InitPurchaseIfNotInit();
        }

        public virtual void TranslateTexts()
        {
            if (m_translateCoinTexts)
            {
                var lm = VP_LocalizationManager.nullableInstance;

                if (m_shopTitleText)
                    m_shopTitleText.text = lm.GetTextTranslated(VP_TextSetup.InApp.SHOP_TITLE);

                for (int i = 0; i < m_packs.Count; i++)
                {
                    if (i >= m_coinTexts.Length)
                    {
                        break;
                    }
                    else
                    {
                        if (m_coinTexts[i])
                            m_coinTexts[i].text = lm.GetTextTranslated(m_packs[i].m_id);

                        if (m_valueTexts[i])
                            m_valueTexts[i].text = lm.GetTextTranslated(m_packs[i].m_priceString);
                    }
                }
            }
        }

        public virtual void PurchaseAdsRemoval()
        {
            VP_AdsManager.nullableInstance.RemoveAds();
        }

        protected override void StartAllListeners()
        {
            base.StartAllListeners();
#if EASY_MOBILE
            InAppPurchasing.PurchaseCompleted += OnPurchaseCompleted;
            InAppPurchasing.RestoreCompleted += OnRestoreCompleted;
            InAppPurchasing.PurchaseFailed += OnPurchaseFailed;
#endif
        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();

#if EASY_MOBILE
            InAppPurchasing.PurchaseCompleted -= OnPurchaseCompleted;
            InAppPurchasing.RestoreCompleted -= OnRestoreCompleted;
            InAppPurchasing.PurchaseFailed -= OnPurchaseFailed;
#endif
        }

        public virtual void ShowShopPanel()
        {
            if (!m_canInteract)
                return;

            m_canInteract = false;

            if (m_appearType == ShopAppearType.DoTween)
            {
                m_group.gameObject.SetActive(true);
#if DOTWEEN
                m_panel.DOScale(1f, .3f).From(.1f).OnComplete(() =>
                {
                    m_canInteract = true;
                    m_OnShowPanel.Invoke(true);
                });
#else
                m_canInteract = true;
                m_OnShowPanel.Invoke(true);
#endif       
            }
            else if (m_appearType == ShopAppearType.Fade)
            {
                m_fadePostProcess.m_effectDuration = m_fadeDuration;
                m_fadePostProcess.FadeUp(true, () =>
                {
                    m_fadePostProcess.FadeDown(false, () =>
                    {
                        m_group.gameObject.SetActive(true);
                        m_OnShowPanel.Invoke(true);
                        WaitTime(.1f, () =>
                        {
                            m_fadePostProcess.FadeUp(false, () =>
                            {
                                m_canInteract = true;
                            });
                        });
                    });
                });
            }
            else if (m_appearType == ShopAppearType.None)
            {
                m_group.gameObject.SetActive(true);
                m_canInteract = true;
                m_OnShowPanel.Invoke(true);
            }

        }

        public virtual void HideShopPanel()
        {
            if (!m_canInteract)
                return;

            m_canInteract = false;


            if (m_appearType == ShopAppearType.DoTween)
            {
#if DOTWEEN
                m_panel.DOScale(1f, .3f).From(.1f).OnComplete(() =>
                {
                    m_canInteract = true;
                    m_group.gameObject.SetActive(false);
                    m_OnShowPanel.Invoke(false);
                });
#else
                m_canInteract = true;
                m_group.gameObject.SetActive(false);
                m_OnShowPanel.Invoke(false);
#endif     
            }
            else if (m_appearType == ShopAppearType.Fade)
            {
                m_fadePostProcess.m_effectDuration = m_fadeDuration;
                m_fadePostProcess.FadeUp(true, () =>
                {
                    m_fadePostProcess.FadeDown(false, () =>
                    {
                        m_group.gameObject.SetActive(false);
                        m_OnShowPanel.Invoke(false);
                        WaitTime(.1f, () =>
                        {
                            m_fadePostProcess.FadeUp(false, () =>
                            {
                                m_canInteract = true;
                            });
                        });
                    });
                });
            }
            else if (m_appearType == ShopAppearType.None)
            {
                m_canInteract = true;
                m_group.gameObject.SetActive(false);
                m_OnShowPanel.Invoke(false);
            }
        }
        public virtual void InitPurchaseIfNotInit()
        {
#if EASY_MOBILE
            if (!InAppPurchasing.IsInitialized())
                InAppPurchasing.InitializePurchasing();
#endif
        }

        // Buy an IAP product using its name
        public virtual void Purchase(string productName)
        {
            VP_Debug.Log("Trying to purchase " + productName);
#if EASY_MOBILE
            if (InAppPurchasing.IsInitialized())
            {
                InAppPurchasing.PurchaseWithId(productName);
            }
            else
            {
                var lm = VP_LocalizationManager.Instance;

                NativeUI.Alert(lm.GetTextTranslated(VP_TextSetup.InApp.SERVICE_UNAVAILABLE), lm.GetTextTranslated(VP_TextSetup.InApp.CHECK_INTERNET_CONNECTION));
            }
#endif
        }

        /// <summary>
        /// Restore purchase
        /// </summary>
        public virtual void RestorePurchase()
        {
            var lm = VP_LocalizationManager.Instance;

            if (m_canRestorePurchases)
            {
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
#if EASY_MOBILE
                if (InAppPurchasing.IsInitialized())
                {
                    InAppPurchasing.RestorePurchases();
                }
                else
                {
                    NativeUI.Alert(lm.GetTextTranslated(VP_TextSetup.InApp.SERVICE_UNAVAILABLE), lm.GetTextTranslated(VP_TextSetup.InApp.CHECK_INTERNET_CONNECTION));
                }
#endif
#endif
            }
            else
            {
#if EASY_MOBILE
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
                NativeUI.Alert(lm.GetTextTranslated(VP_TextSetup.InApp.SERVICE_UNAVAILABLE), lm.GetTextTranslated(VP_TextSetup.InApp.CHECK_INTERNET_CONNECTION));
#endif
#endif
            }
        }


#if EASY_MOBILE

        /// <summary>
        ///  Failure purchase handler
        /// </summary>
        /// <param name="product"></param>
        protected virtual void OnPurchaseFailed(IAPProduct product, string _name)
        {
            string name = product.Id;

            // Purchase of coin packs
            foreach (T pack in m_packs)
            {
                if (pack.m_id.Equals(name))
                {
                    TriggerFailEvent(pack);
                    break;
                }
            }

            VP_Debug.Log("Failed to buy ID: " + product.Name + " and name: "+_name);

        }

        /// <summary>
        /// Successful purchase handler
        /// </summary>
        /// <param name="product"></param>
        protected virtual void OnPurchaseCompleted(IAPProduct product)
        {
            string name = product.Id;

            // Purchase of coin/Whatever packs
            foreach (T pack in m_packs)
            {
                if (pack.m_id.Equals(name))
                {
                    TriggerCompleteEvent(pack);
                    break;
                }
            }
        }

        /// <summary>
        /// Successful purchase restoration handler
        /// </summary>
        protected virtual void OnRestoreCompleted()
        {
            var lm = VP_LocalizationManager.Instance;
            NativeUI.Alert(lm.GetTextTranslated(VP_TextSetup.InApp.PURCHASE_RESTORE_COMPLETED), lm.GetTextTranslated(VP_TextSetup.InApp.PURCHASE_RESTORE_COMPLETED_MSG));
        }
#endif
    }
}

using UnityEngine;
using System;

namespace Game.Interfaces
{
    /// <summary>
    /// Reklam servisi için interface. AdMob veya başka reklam sağlayıcıları için kullanılır.
    /// </summary>
    public interface IAdService
    {
        /// <summary>
        /// Reklam servisini başlatır
        /// </summary>
        void InitializeAdService();
        
        /// <summary>
        /// Banner reklamı gösterir
        /// </summary>
        void ShowBannerAd();
        
        /// <summary>
        /// Interstitial (tam ekran) reklamı gösterir
        /// </summary>
        void ShowInterstitialAd();
        
        /// <summary>
        /// Ödüllü reklamı gösterir. Reklam izlendiğinde onComplete callback'i çağrılır.
        /// </summary>
        /// <param name="onComplete">Reklam izlendiğinde çağrılacak callback</param>
        void ShowRewardedAd(Action onComplete);
        
        /// <summary>
        /// Ödüllü reklam hazır mı kontrol eder
        /// </summary>
        /// <returns>Ödüllü reklam hazır mı?</returns>
        bool IsRewardedAdReady();
    }
}


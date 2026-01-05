using UnityEngine;
using System;

namespace Game.Interfaces
{
    /// <summary>
    /// Satın alma servisi için interface. Unity IAP veya başka satın alma sistemleri için kullanılır.
    /// </summary>
    public interface IPurchaseService
    {
        /// <summary>
        /// Satın alma servisini başlatır
        /// </summary>
        void InitializePurchaseService();
        
        /// <summary>
        /// Ürün satın alır
        /// </summary>
        /// <param name="productId">Satın alınacak ürün ID'si</param>
        /// <param name="onComplete">Satın alma tamamlandığında çağrılacak callback (başarılı: true, başarısız: false)</param>
        void PurchaseProduct(string productId, Action<bool> onComplete);
        
        /// <summary>
        /// Ürün sahip olunan ürünler arasında mı kontrol eder
        /// </summary>
        /// <param name="productId">Kontrol edilecek ürün ID'si</param>
        /// <returns>Ürün sahip olunan ürünler arasında mı?</returns>
        bool IsProductOwned(string productId);
    }
}


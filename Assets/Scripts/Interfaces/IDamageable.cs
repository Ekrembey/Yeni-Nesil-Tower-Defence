using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Interfaces
{
    /// <summary>
    /// Hasar alabilen nesneler için temel arayüz.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Nesnenin hasar almasını sağlar.
        /// </summary>
        /// <param name="damage">Verilecek hasar miktarı.</param>
        void TakeDamage(float damage);
    }
}



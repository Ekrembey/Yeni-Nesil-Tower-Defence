using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Enemies
{
    /// <summary>
    /// Ekran tıklamalarını raycast ile düşmanlara yönlendiren input yöneticisi.
    /// Main Camera üzerine eklenmelidir.
    /// </summary>
    public class EnemyClickInput : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_camera == null)
            {
                Debug.LogError("EnemyClickInput: Kamera bulunamadı! Lütfen _camera alanını atayın.");
            }
        }

        private void Update()
        {
            if (_camera == null)
            {
                return;
            }

            // Sol tık
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                {
                    Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.HandleClick();
                    }
                }
            }
        }
    }
}



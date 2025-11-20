using UnityEngine;

namespace HongNV.Game.Level3
{
public class DontDestroyOnLoad : MonoBehaviour
{
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}

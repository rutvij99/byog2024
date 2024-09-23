using System;
using UnityEngine;

namespace Bosses
{
    public class Checkpoint : MonoBehaviour
    {
        private void Start()
        {
            PlayerPrefs.SetString("LastSavedScene", this.gameObject.scene.name);
            PlayerPrefs.Save();
        }
    }
}
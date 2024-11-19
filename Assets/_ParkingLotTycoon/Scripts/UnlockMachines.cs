using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ArcadeBridge
{
    public class UnlockMachines : MonoBehaviour
    {
        [SerializeField] private List<GameObject> Machine = null;

        public void UnlockObject()
        {
            if (Machine != null)
            {
                for (int i = 0; i < Machine.Count; i++)
                {
                    Machine[i].SetActive(true);
                }
            }
            else
            {
                Debug.LogError("Nothing to unlock for unlocker " + name);
            }
        }
    }
}

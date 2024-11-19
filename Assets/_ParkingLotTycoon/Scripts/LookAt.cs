using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge
{
    public class LookAt : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ArcadeBridge
{
    public class TestDestinationSetter : MonoBehaviour
    {
        [SerializeField] private Transform dest;

        [SerializeField]private NavMeshAgent agent;
        // Start is called before the first frame update
        void Start()
        {
            agent.SetDestination(dest.position);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

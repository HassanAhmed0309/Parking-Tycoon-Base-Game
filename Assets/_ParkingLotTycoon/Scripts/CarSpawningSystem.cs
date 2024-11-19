using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace ArcadeBridge
{
    public class CarSpawningSystem : MonoBehaviour
    {
        [SerializeField] private List<GameObject> carPrefabs;
        [SerializeField] private int carsAllowedInQueue = 5;
        [SerializeField] public Transform spawnPoint;
        [SerializeField] private List<Transform> stopPoint;
        [SerializeField] private Transform despawnPoint;
        [SerializeField] private int stoppingDistanceFromNextCar = 0;
        [SerializeField] private Transform parkingLotExitPoint = null;
        
        private int spawnWaitTime = 0;
        private int totalPooledCars = 0;
        
        [Header("Queue")] [SerializeField]private List<GameObject> currentCars;
        
        
        [SerializeField] private bool hasCarBeenSpawned = false;

        public static Action<GameObject> dequeueFromList;
        public static Action<Transform> addToStopPointsList;
        public static Func<GameObject> instantiateACar;
        
        private void Awake()
        {
            currentCars = new List<GameObject>();
        }

        private void OnEnable()
        {
            dequeueFromList += DequeueTopCarFromList;
            addToStopPointsList += AddToStopPointList;
            instantiateACar += InstantiatedCar;
            StartCoroutine(HandleCarSpawning());
        }
        
        IEnumerator HandleCarSpawning()
        {
            yield return new WaitUntil(() => stopPoint.Count != 0 && !hasCarBeenSpawned);
            //hasCarBeenSpawned = true;
            if (currentCars!= null && currentCars.Count < carsAllowedInQueue)
            {
                SpawnTimeForCars();
            }
        }

        void AddToStopPointList(Transform point)
        {
            stopPoint.Add(point);
        }
        
        IEnumerator SpawnCars(float waitTime)
        {
            yield return new WaitUntil(() => !hasCarBeenSpawned);
            hasCarBeenSpawned = true;
            
            yield return new WaitForSecondsRealtime(waitTime);
            //Debug.Log("Waited for 6 sec");
            //Debug.Log("Instantiate Cars");
            
            var instantiatedCar = InstantiatedCar();

            currentCars.Add(instantiatedCar);
            
            hasCarBeenSpawned = false;
            StartCoroutine(HandleCarSpawning());
        }

        public GameObject InstantiatedCar()
        {
            GameObject instantiatedCar = Instantiate(carPrefabs[0],spawnPoint.position,spawnPoint.rotation,spawnPoint);
            instantiatedCar.gameObject.name += totalPooledCars;
            totalPooledCars++;
            
            CarMovement instantiatedCarMovementScript = instantiatedCar.GetComponent<CarMovement>();
            if (currentCars.Count <= 0)
            {
                instantiatedCarMovementScript.nextCarTransform = null;
            }
            else
                instantiatedCarMovementScript.nextCarTransform = currentCars.ElementAt(currentCars.Count - 1).transform;

            //Choose One Random Stop point
            int stopIndex = 0;
            if (stopPoint.Count > 1)
            {
                stopIndex = UnityEngine.Random.Range(0, stopPoint.Count);
            }
            else
                stopIndex = 0;

            instantiatedCarMovementScript.ParkingSystem = ParkingLotsHandler.parkingSystem?.Invoke(stopIndex);
            instantiatedCarMovementScript.stopPointInd = stopIndex;
            instantiatedCarMovementScript.finalStopPoint = stopPoint[stopIndex];
            instantiatedCarMovementScript.despawnPoint = despawnPoint;
            instantiatedCarMovementScript.stoppingDistanceFromNextCar = stoppingDistanceFromNextCar;
            instantiatedCarMovementScript._carSpawningSystem = this;
            
            instantiatedCar.SetActive(true);
            return instantiatedCar;
        }

        void DequeueTopCarFromList(GameObject Car)
        {
            if (currentCars.Count != 0)
            {
                //Dequeue from Queue
                currentCars.Remove(Car);
                
                if(!hasCarBeenSpawned)
                    StartCoroutine(HandleCarSpawning());
                
                if (currentCars.Count != 0)
                {
                    CarMovement tempRef = null;
                    GameObject carWithRef = null; /*currentCars.Find(x => x.GetComponent<CarMovement>().nextCarTransform.gameObject == Car);*/
                    for (int i = 0; i < currentCars.Count; i++)
                    {
                        tempRef = currentCars[i].GetComponent<CarMovement>(); 
                        if (tempRef != null && tempRef.nextCarTransform != null && tempRef.nextCarTransform.gameObject == Car)
                        {
                            carWithRef = currentCars[i];
                            break;
                        }
                    }
                    if (carWithRef != null)
                    {
                        //Debug.Log("Found the Car with Forward reference as " + Car);
                        //Setup next car to move to the end point
                        CarMovement topCar = carWithRef.GetComponent<CarMovement>();
                        topCar.nextCarTransform = null;
                        topCar.ResetSpeed();
                    }
                    else
                    {
                        Debug.Log("Didn't find a car with forward reference " + Car);
                    }
                }
            }
        }

        private void SpawnTimeForCars()
        {
            if (currentCars.Count == 0)
            {
                spawnWaitTime = 1;
            }
            else
            {
                spawnWaitTime = 6;
            }

            //Debug.Log("Spawn Cars Function Start!");
            StartCoroutine(SpawnCars(spawnWaitTime));
        }

        private void OnDisable()
        {
            dequeueFromList -= DequeueTopCarFromList;
            addToStopPointsList -= AddToStopPointList;
            instantiateACar -= InstantiatedCar;
        }
    }
}


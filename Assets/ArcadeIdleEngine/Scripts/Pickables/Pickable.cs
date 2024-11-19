using ArcadeBridge.ArcadeIdleEngine.Helpers;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Pickables
{
    public class Pickable : MonoBehaviour
    {
        [SerializeField] PickableDefinition _definition;
        
        Vector3 _defaultLocalScale;
        
        public PickableDefinition Definition => _definition;
        public int SellValue => _definition.SellValue;

        void Awake()
        {
            _defaultLocalScale = transform.localScale;
        }

        public void ReleaseToPool()
        {
            Transform trans = transform;
            trans.localScale = _defaultLocalScale;
            TweenHelper.KillAllTweens(trans);
            _definition.Pool.PutBackToPool(this);
        }

        // TODO: might remove here
        // public void Jump(Vector3 targetPoint, float jumpPower, int numJumps, float duration)
        // {
        //     TweenHelper.Jump(transform, targetPoint, jumpPower, numJumps, duration, DisappearSlowlyToPool);
        // }
        //
        // void DisappearSlowlyToPool()
        // {
        //     TweenHelper.DisappearSlowlyToPool(this);
        // }
    }
}
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Data
{
    [CreateAssetMenu(menuName = nameof(ArcadeIdleEngine) + "/" + nameof(Data) + "/" + nameof(IntVariable))]
    public class IntVariable : Saveable<int>
    {
        public override void RestoreState(object obj)
        {
            if (obj == null)
            {
                obj = GetDefaultValue;
            }
            RuntimeValue = (int)obj;
        }
    }
}
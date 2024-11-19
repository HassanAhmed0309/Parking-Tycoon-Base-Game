using System;
using ArcadeIdleEngine.ExternalAssets.NaughtyAttributes_2._1._4.Core.DrawerAttributes;
using ArcadeIdleEngine.ExternalAssets.NaughtyAttributes_2._1._4.Core.DrawerAttributes_SpecialCase;
using UnityEditor;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Data
{
    public abstract class Saveable<T> : Saveable
    {
        [SerializeField] T _initialValue;
        [NonSerialized, ShowNonSerializedField] T _runtimeValue;

#if UNITY_EDITOR
        [SerializeField, InfoBox("Specify a value and override Runtime Value. Useful for debugging.")] T _overrideValue;
#endif
        public event Action<T> ValueChanged;
        public override object GetDefaultValue => _initialValue;
        
        public T RuntimeValue
        {
            get => _runtimeValue;
            set
            {
                _runtimeValue = value;
                ValueChanged?.Invoke(_runtimeValue);
            }
        }

        public override object CaptureState()
        {
            return RuntimeValue;
        }

        protected void OnValueChanged(T t)
        {
            ValueChanged?.Invoke(t);
        }

#if UNITY_EDITOR
        [Button]
        void OverrideRuntimeValue()
        {
            RuntimeValue = _overrideValue;
        }
#endif
    }

    public abstract class Saveable : ScriptableObject
    {
        [SerializeField, HideInInspector]  string _guid;

        public string GetGuid => _guid;

        public abstract object GetDefaultValue { get; }
        
#if UNITY_EDITOR
        void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_guid))
            {
                string path = AssetDatabase.GetAssetPath(this);
                _guid = AssetDatabase.AssetPathToGUID(path);    
            }
        }
#endif

        public abstract void RestoreState(object obj);

        public abstract object CaptureState();

    }
}
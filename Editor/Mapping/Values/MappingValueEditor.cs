using System;
using System.Collections.Generic;
using System.Linq;
using Translations.Mapping;
using Translations.Mapping.Values;
using Translations.Utility;
using UnityEditor;

namespace Translations.Editor.Mapping.Values
{
    public abstract class MappingValueEditor
    {
        [InitializeOnLoadMethod]
        static void Init()
        {
            _ = Editors;
        }

        private static Dictionary<Type, Type> _editors = null;
        public static Dictionary<Type, Type> Editors
        {
            get
            {
                if (_editors == null)
                    _editors = TypeFinder.FindAllTypes<MappingValueEditor>()
                        .Where(x => !x.IsAbstract)
                        .Select(x => (MappingValueEditor)TypeFinder.CreateConstructorFromType(x))
                        .ToDictionary(x => x.TargetType, x => x.GetType());

                return _editors;
            }
        }

        public static MappingValueEditor GetEditor(MappingValue value)
        {
            var type = value?.GetType();
            var editor = type != null && Editors.TryGetValue(type, out var val) ?
                (MappingValueEditor)TypeFinder.CreateConstructorFromType(val) :
                null;

            editor?.InitializeValue(value);
            return editor;
        }

        public abstract Type TargetType { get; }

        public virtual void InitializeValue(MappingValue value) { }
        public abstract void OnGUIValue(MappingValue value);
    }

    public abstract class MappingValueEditor<T> : MappingValueEditor where T : MappingValue
    {
        public override Type TargetType => typeof(T);

        public sealed override void InitializeValue(MappingValue value) =>
            Initialize(value as T);
        public sealed override void OnGUIValue(MappingValue value) =>
            OnGUI(value as T);

        public virtual void Initialize(T value) { }
        public abstract void OnGUI(T value);
    }
}
using System.Collections.Generic;

namespace Translations.Editor.Mapping.Inspectors
{
    internal abstract class MappingWindowObjectInspector
    {
        public MappingWindowObjectInspector(MappingWindowInspector inspector)
        {
            Inspector = inspector;
        }

        public MappingWindowInspector Inspector { get; set; }

        public abstract bool ShouldOpen(IEnumerable<object> selected);

        public virtual void Initialize()
        {

        }
        
        public virtual void Uninitialize()
        {

        }

        public abstract void OnGUI();
    }
}
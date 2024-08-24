﻿using System;
using System.Collections.Generic;
using Translations.Mapping.Values;
using UnityEngine;

namespace Translations.Mapping
{
    [Serializable]
    public class MappingItem
    {
        public MappingItem() { }
        public MappingItem(string tag)
        {
            this.tag = tag;
        }

        public string tag;
        [SerializeReference] public MappingValue defaultValue = new MappingValueText();

        public List<MappingDynamicValue> dynamicValues = new List<MappingDynamicValue>();
    }
}
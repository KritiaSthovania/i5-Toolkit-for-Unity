﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.Toolkit.Core.Utilities.UnityAdapters
{
    public class TextMeshTextAdapter : ITextAdapter
    {
        public TextMesh TextMesh { get; private set; }

        public TextMeshTextAdapter(TextMesh textMesh)
        {
            TextMesh = textMesh;
        }

        public string Text
        {
            get => TextMesh.text;
            set => TextMesh.text = value;
        }
    }
}
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class CommonDelegate
    {
        public delegate void FloatChangedDelegate(float oldValue, float newValue);
        public delegate void IntChangedDelegate(int oldValue, int newValue);
        public delegate void StringChangedDelegate(string oldValue, string newValue);
    }
}
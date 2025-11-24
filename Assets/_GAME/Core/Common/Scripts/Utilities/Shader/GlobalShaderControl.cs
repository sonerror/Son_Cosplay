using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
  public class GlobalShaderControl : MonoBehaviour
  {
    [System.Serializable]
    public abstract class ShaderProperty<T>
    {
      public string name;
      public T value;
      private int _id = 0;
      public void Apply()
      {
        if (_id == 0)
        {
          _id = Shader.PropertyToID(name);
        }
        Apply(_id, value);
      }
      protected abstract void Apply(int id, T value);
    }

    [System.Serializable]
    public class ShaderPropertyFloat : ShaderProperty<float>
    {
      protected override void Apply(int id, float value) => Shader.SetGlobalFloat(id, value);
    }

    [System.Serializable]
    public class ShaderPropertyInt : ShaderProperty<int>
    {
      protected override void Apply(int id, int value) => Shader.SetGlobalInt(id, value);
    }

    [System.Serializable]
    public class ShaderPropertyColor : ShaderProperty<Color>
    {
      protected override void Apply(int id, Color value) => Shader.SetGlobalColor(id, value);
    }

    [System.Serializable]
    public class ShaderPropertyVector4 : ShaderProperty<Vector4>
    {
      protected override void Apply(int id, Vector4 value) => Shader.SetGlobalVector(id, value);
    }

    [System.Serializable]
    public class ShaderPropertyTexture : ShaderProperty<Texture>
    {
      protected override void Apply(int id, Texture value) => Shader.SetGlobalTexture(id, value);
    }

    public List<ShaderPropertyInt> propertyInts = new List<ShaderPropertyInt>();
    public List<ShaderPropertyFloat> propertyFloats = new List<ShaderPropertyFloat>();
    public List<ShaderPropertyColor> propertyColors = new List<ShaderPropertyColor>();
    public List<ShaderPropertyVector4> propertyVector4s = new List<ShaderPropertyVector4>();
    public List<ShaderPropertyTexture> propertyTextures = new List<ShaderPropertyTexture>();


    public void Apply()
    {
      foreach (var property in propertyInts)
      {
        property.Apply();
      }
      foreach (var property in propertyFloats)
      {
        property.Apply();
      }
      foreach (var property in propertyColors)
      {
        property.Apply();
      }
      foreach (var property in propertyVector4s)
      {
        property.Apply();
      }
      foreach (var property in propertyTextures)
      {
        property.Apply();
      }
    }

#if UNITY_EDITOR
    [SerializeField] private bool _isAutoApplyOnValidate = false;
    private void OnValidate()
    {
      if (_isAutoApplyOnValidate)
      {
        Apply();
      }
    }
#endif
  }
}
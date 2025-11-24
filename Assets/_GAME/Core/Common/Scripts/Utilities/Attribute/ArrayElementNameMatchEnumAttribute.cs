using UnityEngine;

public class ArrayElementNameMatchEnumAttribute : PropertyAttribute
{
    public System.Type enumType;
    public int startIndex = 0;
    public int lastIndex = -1;
    public float height = 20f;

    public ArrayElementNameMatchEnumAttribute(System.Type enumType, int startIndex = 0, int lastIndex = -1, float height = 20f)
    {
        this.enumType = enumType;
        this.startIndex = startIndex;
        this.lastIndex = lastIndex;
        this.height = height;
    }
}
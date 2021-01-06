[System.Serializable]
public struct SerializableColor
{
   public float r, g, b, a;

   public SerializableColor(UnityEngine.Color color)
   {
      r = color.r;
      g = color.g;
      b = color.b;
      a = color.a;
   }

   public UnityEngine.Color GetUnityColor()
   {
      return new UnityEngine.Color(r, g, b, a);
   }
}
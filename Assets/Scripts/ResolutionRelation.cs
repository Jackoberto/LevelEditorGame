using System;
using UnityEngine;

public class ResolutionRelation : MonoBehaviour
{
    //private static Resolution DefaultResolution => new Resolution{width = 1920, height = 1080, refreshRate = 60};
    //private static Rect DefaultResolution => new Rect(0, 0 , 1920, 1080); 
    private static Rect _previousResolution;
    /*public static float DefaultAspectRatio => DefaultResolution.width / DefaultResolution.height;
    public static float AspectRatio => (float) _camera.pixelWidth / _camera.pixelHeight;
    public static float AspectRatioRelation => AspectRatio / DefaultAspectRatio;
    private static int DefaultResPixelCount => DefaultResolution.width * DefaultResolution.height;
    public static float ScreenRelation => (float) Screen.width * Screen.height / DefaultResPixelCount;
    public static float WidthRelation => (float) Screen.width / DefaultResolution.width;
    public static float HeightRelation => (float) Screen.height / DefaultResolution.height;*/
    public static event Action ONResolutionChanged;
    private static Camera _camera;

    private void Start()
    {
        _previousResolution = GetComponent<Camera>().pixelRect;
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (_camera.pixelRect != _previousResolution)
        {
            ONResolutionChanged?.Invoke();
            _previousResolution = _camera.pixelRect;
        }
    }
}
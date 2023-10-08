using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValues : MonoBehaviour
{
    public static Camera mainCamera;
    public static Transform mainCameraTransform;
    public static int ShaderId_OutlineThickness;
    public static int ShaderId_OutlineAnimationSpeed;
    public static int ShaderId_NumberOfDots;
    public static int ShaderId_DotRadius;

    public static int currentScore;

    private void Awake() {
        mainCamera = Camera.main;
        mainCameraTransform = mainCamera.transform;
        ShaderId_OutlineThickness = Shader.PropertyToID("_OutlineThickness");
        ShaderId_OutlineAnimationSpeed = Shader.PropertyToID("_OutlineAnimationSpeed");
        ShaderId_NumberOfDots = Shader.PropertyToID("_NumberOfDots");
        ShaderId_DotRadius = Shader.PropertyToID("_DotRadius");
        currentScore = 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class BucketManager : MonoBehaviour
{
    public bool isUpdateBucketEveryFrame;
    public Camera mainCamera;
    public Material bucketOutlineMaterial;
    public float colliderThickness = 1.0f;
    public float topCollderHeight = 200.0f;

    [Range(0.0f, 5.0f)]
    public float bucketOutlineAnimationSpeed = 1.0f;
    [Range(0.0f, 0.5f)]
    public float bucketDotRadius = 0.2f;

    public Sprite squareSprite;
    [Header("---------------------------------------------")]
    public SpriteRenderer bucketSpriteRenderer;
    public BoxCollider2D topCollider;
    public BoxCollider2D buttomCollider;
    public BoxCollider2D leftCollider;
    public BoxCollider2D rightCollider;
    public BoxCollider2D floorCollider;


    private void Awake() {
        InitColliders();
    }

    private void Start() {
        UpdateBucketViaCamera();
    }


    [ContextMenu("Init")]
    private void InitColliders() {
        if(!mainCamera)
            mainCamera = Camera.main;

        if(!topCollider)
            topCollider = InitCollider("TopCollider", true);

        if(!buttomCollider)
            buttomCollider = InitCollider("ButtomCollider");

        if(!leftCollider)
            leftCollider = InitCollider("LeftCollider");

        if(!rightCollider)
            rightCollider = InitCollider("RightCollider");

        if(!floorCollider)
            floorCollider = InitCollider("FloorCollider");

        if(!bucketSpriteRenderer)
            bucketSpriteRenderer = InitBucketSpriteRenderer("BucketSpriteRenderer");
    }

    private BoxCollider2D InitCollider(string objectName, bool isTrigger = false) {
        Transform transBuff = new GameObject(objectName).transform;
        transBuff.SetParent(transform);
        transBuff.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        transBuff.localScale = Vector3.one;

        BoxCollider2D output = transBuff.AddComponent<BoxCollider2D>();
        output.isTrigger = isTrigger;

        if(!isTrigger) {
            Rigidbody2D rigid = transBuff.AddComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;
            rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        return output;
    }


    private SpriteRenderer InitBucketSpriteRenderer(string objectName) {
        Transform transBuff = new GameObject(objectName).transform;
        transBuff.SetParent(transform);

        SpriteRenderer spriteRenderer = transBuff.AddComponent<SpriteRenderer>();
        spriteRenderer.sharedMaterial = bucketOutlineMaterial;
        spriteRenderer.sprite = squareSprite;

        BoxCollider2D coll = transBuff.AddComponent<BoxCollider2D>();
        coll.size = Vector2.one;
        coll.isTrigger = true;


        return spriteRenderer;
    }


    private void Update() {
        if(isUpdateBucketEveryFrame)
            UpdateBucketViaCamera();
    }


    public void UpdateBucketViaCamera() {
        Vector3 camPos = mainCamera.transform.position;
        camPos.x = 0.0f;
        camPos.y = mainCamera.orthographicSize;
        mainCamera.transform.position = camPos;

        float screenWidth = mainCamera.orthographicSize * 2.0f * mainCamera.aspect;
        float screenHeight = mainCamera.orthographicSize * 2.0f;
        float sideColliderHeight = mainCamera.orthographicSize * 2.0f - PixelToMeter(topCollderHeight);
        float topColliderOffset = screenHeight*0.5f - PixelToMeter(topCollderHeight);
        float sideColliderOffset = -PixelToMeter(topCollderHeight * 0.5f);
        Vector3 cameraToBucket = mainCamera.transform.position - transform.position;

        topCollider.size = new Vector2(screenWidth, PixelToMeter(1));
        topCollider.transform.localPosition = new Vector3(0.0f, topColliderOffset) + cameraToBucket;

        buttomCollider.size = new Vector2(screenWidth, colliderThickness);
        buttomCollider.transform.localPosition = new Vector3(0.0f, -screenHeight*0.5f + colliderThickness*0.5f) + cameraToBucket;

        leftCollider.size = new Vector2(colliderThickness, sideColliderHeight);
        leftCollider.transform.localPosition = new Vector3(-screenWidth*0.5f + colliderThickness*0.5f, sideColliderOffset) + cameraToBucket;

        rightCollider.size = new Vector2(colliderThickness, sideColliderHeight);
        rightCollider.transform.localPosition = new Vector3(screenWidth*0.5f - colliderThickness*0.5f, sideColliderOffset) + cameraToBucket;

        floorCollider.size = new Vector2(screenWidth * 3.0f, colliderThickness);
        floorCollider.transform.localPosition = new Vector3(0, -screenHeight*0.5f - colliderThickness*0.5f) + cameraToBucket;



        // Update bucketSpriteRenderer
        Transform spriteTran = bucketSpriteRenderer.transform;
        spriteTran.SetLocalPositionAndRotation(new Vector3(0.0f, sideColliderHeight*0.5f, 0.0f), Quaternion.identity);
        spriteTran.localScale = new Vector3(screenWidth, sideColliderHeight, 1.0f);

        bucketSpriteRenderer.sharedMaterial.SetFloat(GlobalValues.ShaderId_OutlineThickness, colliderThickness);
        bucketSpriteRenderer.sharedMaterial.SetFloat(GlobalValues.ShaderId_OutlineAnimationSpeed, bucketOutlineAnimationSpeed);
        bucketSpriteRenderer.sharedMaterial.SetInt(GlobalValues.ShaderId_NumberOfDots, (int)(screenWidth));
        bucketSpriteRenderer.sharedMaterial.SetFloat(GlobalValues.ShaderId_DotRadius, bucketDotRadius);
    }



    public float PixelToMeter(float pixel) => PixelToMeter((int)pixel);
    public float PixelToMeter(int pixel) {
        return pixel / Screen.dpi;
    }
}

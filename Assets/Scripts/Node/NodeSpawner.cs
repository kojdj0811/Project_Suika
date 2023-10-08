using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Redcode.Pools;
using UnityEngine.Timeline;

public class NodeSpawner : MonoBehaviour
{
    public static NodeSpawner S;

    public PoolManager poolManager;
    public BucketManager bucketManager;

    [Range(0.0f, 1.0f)]
    public float outlineThickness = 0.2f;

    [Range(0.0f, 1.0f)]
    public float textRotationLerpAspect = 0.1f;
    [Range(0.001f, 0.01f)]
    public float spawnAreaSideOffset = 0.001f;
    public int nextNodeLevel;


    private Node spawnedNode;


    private void Awake() {
        if(S) {
            DestroyImmediate(this);
            return;
        }

        S = this;
    }


    private void Update() {
        if(Input.GetMouseButtonDown(0) && !spawnedNode) {
            spawnedNode = poolManager.GetFromPool<Node>("Node");
        }


        Vector3 mousePos = GlobalValues.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float nodePosX = 0.0f;
        float nodePosY = 0.0f;

        if(spawnedNode) {
            float halfOfNodeScale = spawnedNode.transform.localScale.x * 0.5f;
            float halfOfSide = GlobalValues.mainCamera.orthographicSize * GlobalValues.mainCamera.aspect - halfOfNodeScale - bucketManager.colliderThickness - spawnAreaSideOffset;
            nodePosX = Mathf.Clamp(mousePos.x, -halfOfSide, halfOfSide);
            nodePosY = GlobalValues.mainCamera.orthographicSize * 2.0f - halfOfNodeScale;
        }

        if(Input.GetMouseButton(0) && spawnedNode) {
            spawnedNode.transform.localPosition = new Vector3(
                nodePosX,
                nodePosY,
                0.0f
            );
        }

        if(Input.GetMouseButtonUp(0) && spawnedNode) {
            if(spawnedNode.isOverlaped) {
                poolManager.TakeToPool<Node>("Node", spawnedNode);
            } else {
                spawnedNode.transform.localPosition = new Vector3(
                    nodePosX,
                    nodePosY, 
                    0.0f
                );
                spawnedNode.rigid.bodyType = RigidbodyType2D.Dynamic;
                spawnedNode.coll.isTrigger = false;

                nextNodeLevel = GetRandomNodeLevel();
            }

            spawnedNode = null;
        }
    }


    public float GetNodeScale(int nodeLevel) {
        return Mathf.Pow(1.2f, nodeLevel);
        // return Mathf.Log(2.0f + nodeLevel, 2.0f);
    }

    public int GetNextNodeLevel() {
        return nextNodeLevel;
    }

    private int GetRandomNodeLevel() {
        return Random.Range(0, 5);
    }
}

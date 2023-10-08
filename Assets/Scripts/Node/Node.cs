using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

public class Node : MonoBehaviour, IPoolObject
{
    public int nodeLevel;
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D coll;
    public Rigidbody2D rigid;
    public TextMeshPro levelText;
    public RectTransform textTrans;

    [HideInInspector]
    public bool isOverlaped = false;
    private int overlpaedCount = 0;

    // private void FixedUpdate() {
        // if(!rigid.IsSleeping()) {
            // textTrans.rotation = Quaternion.Lerp(textTrans.rotation, Quaternion.identity, NodeSpawner.S.textRotationLerpAspect);
        // }
    // }



    private void OnTriggerEnter2D(Collider2D other) {
        overlpaedCount++;
        isOverlaped = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        overlpaedCount--;

        if(overlpaedCount <= 0) {
            overlpaedCount = 0;
            isOverlaped = false;
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        Transform otherTrans = other.transform;

        if(otherTrans.CompareTag("Node")) {
            Node otherNode = otherTrans.GetComponent<Node>();

            if(otherNode.nodeLevel == nodeLevel) {
                NodeCollisionManager.collisonNodePairs.Enqueue((this, otherNode));
            }
        }
    }



    public void OnGettingFromPool()
    {
        overlpaedCount = 0;
        isOverlaped = false;

        SetupNodeLevel(NodeSpawner.S.GetNextNodeLevel());

        coll.isTrigger = true;
        rigid.bodyType = RigidbodyType2D.Kinematic;
    }

    public void SetupNodeLevel(in int nodeLevel) {
        this.nodeLevel = nodeLevel;
        levelText.text = this.nodeLevel.ToString();
        transform.localScale = Vector3.one * NodeSpawner.S.GetNodeScale(nodeLevel);
        spriteRenderer.sharedMaterial.SetFloat(GlobalValues.ShaderId_OutlineThickness, (1.0f - NodeSpawner.S.outlineThickness) * 0.5f);
    }

    public void NodeLevelUp(Node otherNode) {
        nodeLevel++;
        transform.localPosition = (transform.localPosition + otherNode.transform.localPosition) * 0.5f;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0.0f;
        SetupNodeLevel(nodeLevel);
    }

    public void OnCreatedInPool()
    {
    }
}
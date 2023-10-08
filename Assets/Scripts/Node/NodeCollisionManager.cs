using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCollisionManager : MonoBehaviour
{
    public static NodeCollisionManager S;

    public static Queue<(Node, Node)> collisonNodePairs = new Queue<(Node, Node)>();


    private void Awake() {
        if(S) {
            DestroyImmediate(this);
            return;
        }

        S = this;
    }


    private void FixedUpdate() {
        while(collisonNodePairs.Count > 0) {
            (Node, Node) pair = collisonNodePairs.Dequeue();

            if(pair.Item1.gameObject.activeSelf && pair.Item2.gameObject.activeSelf) {
                int score = (int)Mathf.Pow(2.0f, pair.Item1.nodeLevel + 1);
                GlobalValues.currentScore += score;
                Debug.Log($"currentScore : {GlobalValues.currentScore} (+{score})");

                pair.Item1.NodeLevelUp(pair.Item2);
                NodeSpawner.S.poolManager.TakeToPool<Node>("Node", pair.Item2);
            }

            continue;                
        }
    }
}

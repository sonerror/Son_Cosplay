using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// creates a water flow that can be poured from a source container to a target container
[RequireComponent(typeof(LineRenderer))]
//[RequireComponent(typeof(Rigidbody2D))]
public class MGWaterFlow2D : MonoBehaviour
{
    public TextMeshPro txtWaterAmount;

    public LineRenderer waterFlowLine; // the line renderer to draw the water flow using the flow points

    public MGWaterContainer2D targetContainer; // the target container to transfer to
    public MGWaterContainer2D sourceContainer; // the source container to drain from

    public Transform flowTail; // falls last
    public Transform flowHead; // falls first

    public float transferSpeed = 1f; // the speed of the water flow transfer per second

    private float waterAmount; // the amount of water in the flow

    //private Vector2 spawnPos; // the spawn position of the water flow

    private bool isMerging; // is the water flow merging with the water level of the target container? (connected to the target)
    private bool isPouring = true; // is the water flow pouring into the target container? (still connected to the source)

    private float fallSpeed = 9.81f; // the speed of the water flow falling, similar to gravity

    public float WaterAmount
    {
        get => waterAmount; set
        {
            waterAmount = value;
            if (txtWaterAmount != null) txtWaterAmount.text = waterAmount.ToString("F2") + " L";
        }
    }

    private void Awake()
    {
        waterFlowLine.positionCount = 2;
        //spawnPos = transform.position;
        isPouring = true;
        WaterAmount = 0;
        DrawFlow();
    }

    void Update()
    {
        if (isMerging)
        {
            // keep the flow head at the water level of the target container
            flowHead.position = new Vector2(flowHead.position.x, targetContainer.water.position.y);
            // transfer the water from the flow to the target container
            if (WaterAmount > 0)
            {
                float amount = Mathf.Clamp(transferSpeed * Time.deltaTime, 0, WaterAmount);
                targetContainer.WaterAmount += amount;
                WaterAmount -= amount;
            }
        }
        else
        {
            // head falls
            flowHead.position += Vector3.down * fallSpeed * Time.deltaTime;

            // if the flow head touches the water level of the target container, start merging
            if (flowHead.position.y <= targetContainer.water.position.y)
            {
                OnContact();
            }
        }

        if (isPouring)
        {
            // keep the flow tail at source container water level Y and open edge X
            flowTail.position = new Vector2(sourceContainer.openEdge.position.x, sourceContainer.water.position.y - 0.16f);
            // transfer the water from the source container to the flow
            float amount = Mathf.Clamp(transferSpeed * Time.deltaTime, 0, sourceContainer.WaterAmount);
            WaterAmount += amount;
            sourceContainer.WaterAmount -= amount;
        }
        else
        {
            // tail falls
            flowTail.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        if (flowTail.position.y <= targetContainer.water.position.y)
        {
            // if the flow tail touches the water level of the target container, transfer the rest and destroy the flow
            targetContainer.WaterAmount += WaterAmount;
            WaterAmount = 0;
            targetContainer.OnWaterFlowEnd();
            Destroy(gameObject);
        }

        DrawFlow();
    }

    public void DrawFlow()
    {
        // sync the head to tail X
        flowHead.position = new Vector2(flowTail.position.x, Mathf.Clamp(flowHead.position.y, flowHead.position.y, flowTail.position.y));

        // draw the flow line from the flow tail to the flow head, with the X always from the tail
        waterFlowLine.SetPosition(0, flowTail.position);
        waterFlowLine.SetPosition(1, new Vector2(flowTail.position.x, flowHead.position.y));
    }

    // when the water flow starts merging with the water level of the target container
    private void OnContact()
    {
        isMerging = true;
        targetContainer.OnWaterFlowContact(flowHead.position);
    }

    public void OnStopPouring()
    {
        isPouring = false;
    }
}
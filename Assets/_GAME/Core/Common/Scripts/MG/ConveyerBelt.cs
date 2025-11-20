using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves items from one side to the other on a conveyer belt. Each item will be teleported to the other side when it reaches the end.
/// </summary>

public class ConveyerBelt : MonoBehaviour
{
    public float speed = 1.0f; // if speed is negative, items will move in the opposite direction
    public float length = 10.0f;
    public List<GameObject> items = new List<GameObject>();
    public Animator conveyerAnim;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool leftToRight = speed > 0;
        // move items
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items.RemoveAt(i);
                i--;
                continue;
            }

            Vector3 pos = items[i].transform.position;
            pos.x += speed * Time.deltaTime;
            if (leftToRight)
            {
                if (pos.x > transform.position.x + length / 2)
                {
                    pos.x = transform.position.x - length / 2;
                }
            }
            else
            {
                if (pos.x < transform.position.x - length / 2)
                {
                    pos.x = transform.position.x + length / 2;
                }
            }
            items[i].transform.position = pos;
        }
    }

    // visualize the length of the conveyer belt
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(-length / 2, 0, 0), transform.position + new Vector3(length / 2, 0, 0));
    }
}
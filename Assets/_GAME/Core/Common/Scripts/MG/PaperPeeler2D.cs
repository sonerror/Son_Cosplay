using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simulates the peeling of a paper-like object, such as a bandage or a face mask. Player can start peeling from any edge of the object.
/// </summary>
public class PaperPeeler2D : MonoBehaviour
{
    public SpriteRenderer frontRenderer;
    public SpriteRenderer backRenderer;
    public SpriteMask frontMask; // a rectangle spriteIcon that masks the front of the object
    public SpriteMask backMask; // a rectangle spriteIcon that masks the back of the object, and move with the front mask
    public Transform center;

    public bool isPeeling;

    private Vector2 peelDirection;
    private Vector2 peelStartPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            StopPeeling();
        }

        if (isPeeling)
        {
            if (peelStartPos != (Vector2)Input.mousePosition)
            {
                // get peeling direction
                peelDirection = (Vector2)Input.mousePosition - peelStartPos;
                peelDirection.Normalize();

                // rotate the front mask container to the peeling direction
                float angle = Mathf.Atan2(peelDirection.y, peelDirection.x) * Mathf.Rad2Deg;
                frontMask.transform.parent.rotation = Quaternion.Euler(0, 0, angle + 90);

                // rotate the back renderer container to the peeling direction multiplied by 2
                backRenderer.transform.parent.rotation = Quaternion.Euler(0, 0, (angle + 90) * 2);


                //// get the distance between the mirroring edge and the center of the front renderer
                //float distance = Vector2.Distance(center.position, frontMask.transform.position);

                //// move the back renderer's container to the peeling direction
                //transform.parent.position += (Vector3)peelDirection * Time.deltaTime * 2;
                //// move the front mask to the peeling direction half as fast
                //frontMask.transform.parent.position += (Vector3)peelDirection * Time.deltaTime * 1;

                // TODO
            }
        }
    }

    public void StartPeeling()
    {
        peelStartPos = Input.mousePosition;
        peelDirection = Vector2.zero;
        isPeeling = true;
    }

    private void OnMouseDown()
    {
        StartPeeling();
    }


    public void StopPeeling()
    {
        isPeeling = false;
        peelDirection = Vector2.zero;
    }
}

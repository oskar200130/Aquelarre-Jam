using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    Transform menuPos, initialPos;
    [SerializeField]
    Transform[] farStepsPositions;
    private int index;

    private (Vector3, Quaternion) destiny, startTransition;
    private bool tutorial, transitioning;
    private float timeTransition, actTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        index = -1;
        tutorial = false;
        transform.SetPositionAndRotation(menuPos.position, menuPos.rotation);
        destiny = (transform.position, transform.rotation);
        actTime = 0;
        Invoke(nameof(StartConcert), 2.5f);
    }

    public void StartConcert()
    {
        tutorial = false;
        StartMoving(2);
    }

    public void StartMoving(float speed)
    {
        timeTransition = speed;

        if (index == -1)
            destiny = (initialPos.position, initialPos.rotation);
        else if (index < farStepsPositions.Length)
            destiny = (farStepsPositions[index].position, farStepsPositions[index].rotation) ;

        startTransition = (transform.position, transform.rotation);
        index++;
        transitioning = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Movimiento
        if (transitioning)
        {
            transform.SetPositionAndRotation(Vector3.Lerp(startTransition.Item1, destiny.Item1, actTime / timeTransition), Quaternion.Lerp(startTransition.Item2, destiny.Item2, actTime / timeTransition));            
            if (actTime/timeTransition > 1)
            {
                actTime = 0;
                transitioning = false;
                BeatManager._instance.playSong();
            }
            else 
                actTime += Time.deltaTime;

            //Vector3 dir = destiny - transform.position;
            //if (dir.magnitude < 0.2f)
            //{
            //    transform.position = destiny;
            //    transitioning = false;
            //}
            //else
            //    transform.position += timeTransition * Time.deltaTime * dir.normalized;
        }
    }
}

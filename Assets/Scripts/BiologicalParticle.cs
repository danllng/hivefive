using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.IO;

/*
 * This class represents a particle with biological movement and contains the position data to which the particle is set over a period of time.
 */
public class BiologicalParticle : MonoBehaviour
{
    public Vector3[] positions;
    public bool active = false;

    float t = 0;
    float rt = 0;
    float scale = 0.5f;
    float startTime = 1000000000;
    float timeToReachTarget;

    int index = 0;

    bool restarting = false;

    Vector3 restartPosition;
    Vector3 targetPosition;
    Vector3 startPosition;
    Vector3 target;

    BiologicalSwarmManager loader;

    void Start()
    {
        GetComponent<Renderer>().enabled = true;
        setupSwarm();
    }

    void setupSwarm()
    {
        index = 0;
        startPosition = positions[index] * scale;
        transform.localPosition = startPosition;
        index++;
        timeToReachTarget = 0.001f;
        if (positions.Length > 1)
        {
            target = positions[index] * scale;
        }
        index++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(active)
        {
            GetComponent<Renderer>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(active)
        {
            GetComponent<Renderer>().enabled = false;
        }
    }

    void Update()
    {
        this.scale = loader.getScale();

        if(active)
        {
            t += Time.deltaTime * 1.8f;
            int index = (int)(t * 100) % positions.Length;
            transform.localPosition = positions[index] * scale;
        } else
        {
            if(Time.time >= startTime)
            {
                active = true;
            }
        }
    }

    void resetPositions()
    {
        if(Vector3.Distance(transform.localPosition, targetPosition) != 0)
        {
            rt += Time.deltaTime / 1;
            transform.localPosition = Vector3.Lerp(restartPosition, targetPosition, rt);
        } else
        {
            restarting = false;
            rt = 0;
            setupSwarm();
        }
    }

    public void initParticle(BiologicalSwarmManager loader, Vector3[] pos, float start, int name)
    {
        this.loader = loader;
        this.name = name.ToString();
        this.positions = pos;
        this.startTime = start;
    }
}

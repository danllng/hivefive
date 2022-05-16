using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is responsible for the control of the non-biological swarm
 */
public class NonBiologicalSwarmManager : MonoBehaviour
{
    public GameObject prefab;

    public Vector3 bestSwarmPosition = Vector3.zero;
    public Vector3 targetPosition;

    private List<GameObject> targets;
    private GameObject target;

    [HideInInspector]
    public GameObject swarmHolder;
    [HideInInspector]
    public int oldSize;
    [HideInInspector]
    public List<GameObject> swarm;
    [HideInInspector]
    public Vector3 flightLimits = new Vector3(0.1f, 0.1f, 0.1f);
    [HideInInspector]
    public bool flyingToTarget = false;

    [Header("Swarm Settings")]
    [Range(0, 150)]
    public int size = 10;

    public bool targetToManager = true;

    [Range(0f, 359.9f)]
    public float degrees;
    [Range(1f, 100f)]
    public float distance;
    [Range(0f, 5.0f)]
    public float speed = 0.35f;
    [Range(0f, 5.0f)]
    public float spread = 0.1f;
    [Range(1.0f, 10.0f)]
    public float neighbourDistance = 10f;
    [Range(0.0f, 5.0f)]
    public float rotationSpeed = 5f;

    void Start()
    {
        swarmHolder = new GameObject("NonBiologicalSwarm");
        swarmHolder.transform.position = this.transform.position;

        oldSize = size;
        swarm = new List<GameObject>();

        targetPosition = this.transform.position;
        
        for (int i = 0; i < size; i++)
        {
            Vector3 position = randomizeVector(flightLimits.x, targetPosition);
            createParticle(position);
        }
    }

    void Update()
    {
        if(targetToManager)
        {
            targetPosition = this.transform.position;
        }

        if (flyingToTarget)
        {
            if (Vector3.Distance(targetPosition, getSwarmCenterPosition()) < 0.3f)
            { }
        }

        updateSwarmSize();
    }

    void createParticle(Vector3 position)
    {
        GameObject newParticle = (GameObject)Instantiate(prefab, position, Quaternion.identity);
        newParticle.GetComponent<NonBiologicalParticle>()._swarmManager = this;
        newParticle.transform.parent = swarmHolder.transform;
        swarm.Add(newParticle);
    }

    void updateSwarmSize()
    {
        if (oldSize > size)
        {
            if (size >= 0)
            {
                int diff = oldSize - size;

                for (int i = 0; i < diff; i++)
                {
                    GameObject particle = swarm[swarm.Count - 1];
                    swarm.Remove(particle);
                    GameObject.Destroy(particle);
                }

                oldSize = size;
            }
        }
        else if (oldSize < size)
        {
            int diff = size - oldSize;

            for (int i = 0; i < diff; i++)
            {
                createParticle(randomizeVector(Random.Range(0.2f, 0.4f), getSwarmCenterPosition())); // particle is added to swarm while creation
            }

            oldSize = size;
        }
    }

    Vector3 getSwarmCenterPosition()
    {
        Vector3 center = Vector3.zero;
        foreach (GameObject particle in swarm)
        {
            center += particle.transform.position;
        }
        return center / swarm.Count;
    }

    Vector3 randomizeVector(float intensity, Vector3 position)
    {
        return position + new Vector3(
            Random.Range(-intensity, intensity),
            Random.Range(-intensity, intensity),
            Random.Range(-intensity, intensity));
    }

    Vector3 calculateTargetPosition(float deg)
    {
        float r = distance; // radius
        float rad = deg * Mathf.Deg2Rad;

        Vector2 pos2d = new Vector2(Mathf.Sin(rad) * r, Mathf.Cos(rad) * r);
        return new Vector3(pos2d.x, 1.0f, pos2d.y);

    }

    public void flyToTarget(Vector3 target)
    {
        targetToManager = false;
        targetPosition = new Vector3(target.x, 3f, target.z);
        flyingToTarget = true;
        speed = 8;
    }

}

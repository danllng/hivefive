using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class represents a particle with non-biological motion and contains the implementation of the ruleset according to Reynolds.
 */
public class NonBiologicalParticle : MonoBehaviour
{
    public NonBiologicalSwarmManager _swarmManager;

    float speed;
    bool randomizing = false;

    Vector3 tmpTarget;
    MeshRenderer meshRenderer;

    void Start()
    {
        speed = _swarmManager.speed;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        updateBoids();
        speed = _swarmManager.speed;
        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void updateBoids()
    {
        List<GameObject> swarm = _swarmManager.swarm;
        Vector3 center = Vector3.zero;
        Vector3 avoidance = Vector3.zero;
        float globalSpeed = 0.00f;
        float particleDistance;
        int swarmSize = 0;

        foreach (GameObject particle in swarm)
        {
            if(particle != this.gameObject)
            {
                particleDistance = Vector3.Distance(particle.transform.position, this.transform.position);
                if(particleDistance <= _swarmManager.neighbourDistance)
                {
                    center += particle.transform.position;
                    swarmSize++;

                    if(particleDistance < _swarmManager.spread)
                    {
                        avoidance = avoidance + (this.transform.position - particle.transform.position);
                    }

                    NonBiologicalParticle anotherParticle = particle.GetComponent<NonBiologicalParticle>();
                    globalSpeed = globalSpeed + anotherParticle.speed;
                }
            }
        }

        if(swarmSize > 0 && globalSpeed > 0)
        {
            center = center / swarmSize + (_swarmManager.targetPosition - this.transform.position);
            speed = globalSpeed / swarmSize;

            Vector3 direction = (center + avoidance) - transform.position;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    _swarmManager.rotationSpeed * Time.deltaTime);
            }
        }
    }
}

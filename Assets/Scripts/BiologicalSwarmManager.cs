using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

/*
 * Imports the data of the recorded swarm and if necessary adjusts the size and scaling of the swarm.
 */
public class BiologicalSwarmManager : MonoBehaviour
{
    public string data = "BiologicalSwarmTimeSeries.txt";
    // Ob4.txt from Sinhuber, M. et al. The Subnational Human Development Database. Sci. Data. 6:190036 https://doi.org/10.1038/sdata.2019.36 (2019).

    public GameObject prefab;
    public GameObject swarmHolder;

    BiologicalParticle ts;
    List<GameObject> particles;

    [Range(0, 10)]
    public int size = 10;
    [Range(0.001f, 0.0001f)]
    public float _scale = 0.0008f;

    int oldSize = 0;
    int maxsize = 10;

    void Start()
    {
        swarmHolder = new GameObject("BiologicalSwarm");
        swarmHolder.transform.position = this.transform.position;

        particles = new List<GameObject>();
        importData();
    }

    public float getScale()
    {
        return this._scale;
    }

    void Update()
    {
        updateSwarmSize();

    }

    void updateSwarmSize()
    {
        if (oldSize != size)
        {
            oldSize = size;

            int inactive = maxsize - size;
            int active = maxsize - inactive;

            if(active > 0)
            {
                for (int i = 0; i < active; i++)
                {
                    particles[i].GetComponent<Renderer>().enabled = true;
                }
            }

            for (int i = maxsize; i > maxsize - inactive; i--)
            {
                particles[i - 1].GetComponent<Renderer>().enabled = false;
            }
        }
    }

    /*
     * Import the data of the recorded swarm
     */
    void importData()
    {
        string fileData = System.IO.File.ReadAllText(data);
        string[] lines = fileData.Split("\n"[0]);
        List<Vector3> _positions = new List<Vector3>();
        int pos = 0;
        float startTime = 0f;


        for (int i = 0; i < lines.Length; i++)
        {
            string[] lineData = (lines[i].Trim()).Split(","[0]);
            if(pos == 0)
            {
                pos = int.Parse(lineData[0]);
                startTime = float.Parse(lineData[4], CultureInfo.GetCultureInfo("en-US"));
            }

            if (pos == int.Parse(lineData[0]))
            {
                _positions.Add(new Vector3(
                float.Parse(lineData[1], CultureInfo.GetCultureInfo("en-US")),
                float.Parse(lineData[3], CultureInfo.GetCultureInfo("en-US")),
                float.Parse(lineData[2], CultureInfo.GetCultureInfo("en-US"))));

                if(i + 1 == lines.Length)
                {
                    GameObject particle = GameObject.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
                    particle.GetComponent<BiologicalParticle>().initParticle(this, _positions.ToArray(), startTime, pos);
                    particle.transform.parent = swarmHolder.transform;
                    particles.Add(particle);
                }

            } else
            {
                GameObject particle = GameObject.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
                particle.GetComponent<BiologicalParticle>().initParticle(this, _positions.ToArray(), startTime, pos);
                particle.transform.parent = swarmHolder.transform;
                particles.Add(particle);

                _positions = new List<Vector3>();
                pos = int.Parse(lineData[0]);
                startTime = float.Parse(lineData[4], CultureInfo.GetCultureInfo("en-US"));
                i--;
            }
        }
    }
}

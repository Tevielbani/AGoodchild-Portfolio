using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject Sparks;
    public GameObject Ring;
    public ParticleSystem psR;
    public ParticleSystem psS;
    public float knowledge;
    // Start is called before the first frame update
    void Start()
    {
        knowledge = GameManager.knowledge;
        ParticleSystem psS = Sparks.GetComponent<ParticleSystem>();
        ParticleSystem psR = Ring.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        var vol = psR.velocityOverLifetime;
        var emS = psS.emission;
        var emR = psR.emission;

        emS.rateOverTime = (GameManager.knowledge / 2000) - 100;
        emR.rateOverTime = GameManager.knowledge / 200;

        if (knowledge <= 10000)
        {
            vol.orbitalZ = GameManager.knowledge / 200;
        }
        if (GameManager.knowledge >= 10000)
        {
            vol.orbitalZ = 50;
        }
    }
}

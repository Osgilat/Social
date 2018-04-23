using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Particle : MonoBehaviour {

    GameObject particleSystem;
    public GameObject targetGameobject;
    float healTime = 6.0f;
    GameObject targetTemp;

    ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;

    private void Start()
    {
        particleSystem = this.gameObject;
        particleSystem.GetComponent<ParticleSystem>().Pause();

        targetTemp = this.gameObject;
    }

  
    void LateUpdate()
    {
        InitializeIfNeeded();

        targetTemp = GetComponentInParent<HealAbility>().targetToHeal;
        if (targetTemp != this.gameObject)
        {
            if (healTime < 0)
            {
                GetComponentInParent<HealAbility>().targetToHeal = gameObject;
                // particleSystem.GetComponent<ParticleSystem>().Stop();
                targetGameobject = GetComponentInParent<HealAbility>().targetToHeal;
                //GetComponentInParent<HealAbility>().healTrigger = false;
                HealAbility.healTrigger = false;
                healTime = 6.0f;
            }
            else if (healTime >= 0)
            {
                targetGameobject = GetComponentInParent<HealAbility>().targetToHeal;
                // particleSystem.GetComponent<ParticleSystem>().Play();
                healTime -= Time.deltaTime;
            }
        }
        


        particleSystem.GetComponent<ParticleSystem>().Play();

        int length = m_System.GetParticles(m_Particles);
        int i = 0;
        if (targetGameobject != null)
        {
            transform.LookAt(targetGameobject.transform);



            while (i < length)
            {

                //Target is a Transform object
                Vector3 direction = targetGameobject.transform.position - m_Particles[i].position;
                direction.Normalize();

                float variableSpeed = (m_System.startSpeed / (m_Particles[i].remainingLifetime + 0.1f)) + m_Particles[i].startLifetime;
                m_Particles[i].position += direction * variableSpeed * Time.deltaTime;

                if (Vector3.Distance(targetGameobject.transform.position, m_Particles[i].position) < 1.0f)
                {
                    m_Particles[i].remainingLifetime = -0.1f; //Kill the particle
                }

                i++;

            }

            m_System.SetParticles(m_Particles, length);
        }
    }

    void InitializeIfNeeded()
    {
        if (m_System == null)
            m_System = particleSystem.GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < m_System.maxParticles)
            m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
    }
}

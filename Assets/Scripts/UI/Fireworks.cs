using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fireworks : MonoBehaviour
{
    ParticleSystem[] Fireballs; 
    bool ContinueFireworking = false;

    void Awake()
    {
        Fireballs = GetComponentsInChildren<ParticleSystem>();
        foreach (var _fireball in Fireballs)
            _fireball.gameObject.SetActive(false);

    }

    public void StartFireworks(){
        ContinueFireworking = true;
        StartCoroutine(Firewoking());
    }

    public void EndFireworks(){
        ContinueFireworking = false;
    }

    IEnumerator Firewoking(){
        // Start with a burst of fireball
        for (int i=0; i<5; ++i)
            PickRandom();

        // Then continue fireballing
        while (ContinueFireworking){
            yield return new WaitForSeconds(Random.Range(0f, 1f));
            PickRandom();
        }

    }

    void PickRandom(){
        // Select the fireball color
        var _fireIndex = Random.Range(0, Fireballs.Length);
        var _newBall = Instantiate(Fireballs[_fireIndex], transform);
        _newBall.transform.SetParent(transform);
        var _newBallParticle = _newBall.GetComponent<ParticleSystem>();
    
        // Select characterics of the fireball
        _newBall.transform.localPosition = new Vector3(Random.Range(-3f, 3f), Random.Range(-1.5f, 1.5f), 0);
        var _main = _newBallParticle.main;
        _main.startDelay = Random.Range(0f, 2f);
        _main.startDelay = Random.Range(0f, 2f);
        _main.startLifetime = Random.Range(1.75f, 3f);
        _main.startSpeed = Random.Range(75f, 300f);
        
        // Show the ball and destroy it afterward
        _newBall.gameObject.SetActive(true);
        Destroy(_newBall.gameObject, 5);
    }
}

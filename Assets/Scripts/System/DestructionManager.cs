using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class DestructionManager : MonoBehaviour
{
    [SerializeField] float minCoolDown;
    [SerializeField] float maxCoolDown;
    [SerializeField] float duration;
    public int heightLimit;
    [SerializeField] int remainedHeight;
    [SerializeField] int platformSearchDistance;
    [SerializeField] DestructionMode destructionMode;
    [SerializeField] int minLocalHeightLimit;
    [SerializeField] int maxLocalHeightLimit;
    [SerializeField] int maxWindParticles;

    [SerializeField] AudioClip wind;
    float musicWindTimer = 30;
    AudioSource audioSource;

    public GameObject windPrefab;
    public GameObject localWindPrefab;
    public float windTimer;
    public static DestructionManager instance;
    List<GameObject> particles = new List<GameObject>();
    List<TrailRenderer> particleTrails = new List<TrailRenderer>();
    List<GameObject> localParticles = new List<GameObject>();
    List<TrailRenderer> localParticlestrails = new List<TrailRenderer>();

    enum DestructionMode
    {
        global,
        local
    };

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(GlobalDestruction());
    }





    IEnumerator GlobalDestruction()
    {
        while (true)
        {
           
            int coloumscounter = 0;
            int coloumsCounterCounter=5;
            int GridLengthX = GridArray.Instance.arrayX;
            int GridLengthZ = GridArray.Instance.arrayZ;
            
          
            while (coloumscounter<GridLengthX)
            {
               
                for (int i = 0; i < GridLengthZ; i++)
                {
                    GridList target = GridArray.Instance.gridArray[coloumscounter, i];

                   if(target.bridgeObjects.Count==0 && target.structureObjects.Count!=0 && target.sizeY*2 +target.foundationObject.transform.position.y >= heightLimit)
                    {
                        target.sizeY= Mathf.RoundToInt(Mathf.Clamp( (heightLimit -target.foundationObject.transform.position.y) /GridArray.Instance.cellSize ,target.branchedStructures,Mathf.Infinity));
                        
                    }
                }

                for (int i = 0; i < particles.Count; i++)
                {

                    particles[i].transform.localScale = Vector3.Lerp(particles[i].transform.localScale, new Vector3(40f, 40f, 40f), .3f);
                    if (particleTrails[i]!=null && particleTrails[i].widthMultiplier >= 0.1f)
                    {
                        particleTrails[i].widthMultiplier -= .1f;
                    }

                }

                
                coloumscounter +=1;
                if (coloumscounter >= coloumsCounterCounter)
                {
                    coloumsCounterCounter += 5;
                    yield return new WaitForEndOfFrame();
                }
                
                
               
            }
            for (int i = particles.Count-1; i >=0; i--)
            {
                
                    Destroy(particles[i]);
                    particles.RemoveAt(i);
                particleTrails.RemoveAt(i);
                
                
            }
            coloumscounter = 0;
            heightLimit = Mathf.FloorToInt(Random.Range(minLocalHeightLimit, maxLocalHeightLimit) / 3 + Random.Range(minLocalHeightLimit, maxLocalHeightLimit) / 3 + Random.Range(minLocalHeightLimit, maxLocalHeightLimit) / 3);
            float windCooldown = Random.Range(minCoolDown, maxCoolDown);
            windTimer = 0;
            
            yield return new WaitForSeconds(windCooldown - musicWindTimer);
            while (windTimer <musicWindTimer)
            {

                windTimer += Time.deltaTime;

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = wind;
                    audioSource.Play();
                }
               
                if (windTimer/musicWindTimer >= particles.Count/maxWindParticles)
                {
                    ParticleInstantiate(LevelGenerator.instance.Groundbounds.transform.position.x, heightLimit, LevelGenerator.instance.Groundbounds.transform.position.z);
                }
                for(int i = 0;i<particles.Count;i++)
                {
                   
                        particles[i].transform.localScale = Vector3.Lerp(windPrefab.transform.localScale, Vector3.zero,windTimer/musicWindTimer);
                                      
                }
               
                yield return new WaitForEndOfFrame();

            }

        }
    }
    public GameObject ParticleInstantiate(float x, float y, float z)
    {
        GameObject particle = Instantiate<GameObject>(windPrefab, new Vector3(x, y, z), Quaternion.AngleAxis(Random.Range(0,360),Vector3.up));
        if (particle != null)
        {
            particles.Add(particle);
            particleTrails.Add(particle.transform.GetComponentInChildren<TrailRenderer>());
        }
        return particle;

    }
    public GameObject LocalParticleInstantiate(float x, float y, float z)
    {
        GameObject particle = Instantiate<GameObject>(localWindPrefab, new Vector3(x, y, z), Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
        if (particle != null)
        {
            localParticles.Add(particle);
            localParticlestrails.Add(particle.transform.GetComponentInChildren<TrailRenderer>());
        }
        return particle;

    }



    IEnumerator SpawnParticles(int Offset)
    {
       
       for(int i =Offset; i < LevelGenerator.instance.Groundbounds.transform.lossyScale.x; i+=18)
        {
            Instantiate(windPrefab, new Vector3(i, heightLimit + Random.Range(-.75f, .75f), 0 + Random.Range(-2.5f, 2.5f)), Quaternion.identity);
            yield return new WaitForEndOfFrame();
        }
       
    }

   
}

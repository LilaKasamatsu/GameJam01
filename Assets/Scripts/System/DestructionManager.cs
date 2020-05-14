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
    [SerializeField] int maxLocalHeightLimit;
    [SerializeField] int maxWindParticles;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (destructionMode)
            {
                case DestructionMode.global:
                   StartCoroutine(GlobalDestruction());
                    break;
                case DestructionMode.local:
                    StartCoroutine(LocalDestruction());
                    break;


            }
           
        }
    }
    IEnumerator LocalDestruction()
    {
        while (true)
        {
            Debug.Log("SANDSTORM on Level:"+heightLimit);
            int coloumscounter = 0;
            int GridLengthX = GridArray.Instance.arrayX;
            int GridLengthZ = GridArray.Instance.arrayZ;
            
            while (coloumscounter < GridLengthX)
            {
                for (int i = 0; i < GridLengthZ; i++)
                {
                    GridList target = GridArray.Instance.gridArray[coloumscounter, i];

                    if (target.sizeY - target.branchedStructures >= heightLimit)
                    {
                        Vector3 targetVector = new Vector3(coloumscounter * GridArray.Instance.cellSize, target.foundationObject.transform.position.y, i * GridArray.Instance.cellSize);
                        Explode(target, targetVector);

                    }
                    else if (heightLimit==0 && target.sizeY == 0 && target.foundationAmount > 0)
                    {
                        Vector3 targetVector = new Vector3(coloumscounter * GridArray.Instance.cellSize, target.foundationObject.transform.position.y, i * GridArray.Instance.cellSize);
                        LookForStructures(target, targetVector);
                    }

                }


                coloumscounter += 1;
                yield return new WaitForEndOfFrame();
            }
            coloumscounter = 0;
            heightLimit =Mathf.FloorToInt( Random.Range(0, maxLocalHeightLimit)/3 + Random.Range(0, maxLocalHeightLimit) / 3 + Random.Range(0, maxLocalHeightLimit) /3);
            remainedHeight = heightLimit;

            yield return new WaitForSeconds(Random.Range(minCoolDown, maxCoolDown));
        }
    }

    IEnumerator GlobalDestruction()
    {
        while (true)
        {
            Debug.Log("SANDSTORM");
            int coloumscounter = 0;
            int GridLengthX = GridArray.Instance.arrayX;
            int GridLengthZ = GridArray.Instance.arrayZ;
            
          
            while (coloumscounter<GridLengthX)
            {
               
                for (int i = 0; i < GridLengthZ; i++)
                {
                    GridList target = GridArray.Instance.gridArray[coloumscounter, i];

                   if( target.sizeY-target.branchedStructures >= heightLimit)
                    {
                        Vector3 targetVector = new Vector3(coloumscounter * GridArray.Instance.cellSize, target.foundationObject.transform.position.y, i * GridArray.Instance.cellSize);
                        Explode(target,targetVector);

                    }
                   else if (target.sizeY==0 && target.foundationAmount > 0)
                    {
                        Vector3 targetVector = new Vector3(coloumscounter * GridArray.Instance.cellSize, target.foundationObject.transform.position.y, i * GridArray.Instance.cellSize);
                        LookForStructures(target,targetVector);
                    }

                }
                for (int i = 0; i < particles.Count; i++)
                {

                    particles[i].transform.localScale = Vector3.Lerp(particles[i].transform.localScale, new Vector3(40f, 40f, 40f), .5f);
                    if (particleTrails[i].widthMultiplier >= 0.1f)
                    {
                        particleTrails[i].widthMultiplier -= .1f;
                    }

                }


                coloumscounter +=1;
                yield return new WaitForEndOfFrame();
                
               
            }
            for (int i = particles.Count-1; i >=0; i--)
            {
                
                    Destroy(particles[i]);
                    particles.RemoveAt(i);
                
                
            }
            coloumscounter = 0;
            float windCooldown = Random.Range(minCoolDown, maxCoolDown);
            windTimer = 0;
            ParticleInstantiate(LevelGenerator.instance.Groundbounds.transform.position.x, heightLimit, LevelGenerator.instance.Groundbounds.transform.position.z);
            while (windTimer <windCooldown)
            {

                windTimer += Time.deltaTime;
               
                if (windTimer/windCooldown >= particles.Count/maxWindParticles)
                {
                    ParticleInstantiate(LevelGenerator.instance.Groundbounds.transform.position.x, heightLimit, LevelGenerator.instance.Groundbounds.transform.position.z);
                }
                for(int i = 0;i<particles.Count;i++)
                {
                   
                        particles[i].transform.localScale = Vector3.Lerp(windPrefab.transform.lossyScale, Vector3.zero,windTimer/windCooldown);
                                      
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

    void Explode(GridList target, Vector3 targetVector)
    {
        List<GameObject> targetList = target.structureObjects;
        for(int i=target.bridgeObjects.Count-1;i>=0;i--)
        {
            GameObject targetBridgeObject = target.bridgeObjects[i];
            BridgeStruct targetscript = targetBridgeObject.GetComponent<BridgeStruct>();
            Vector3 targetBridgeOrigin =targetscript.gridOrigin;
            Vector3 targetBridgeEnd = targetscript.gridEnd;
            GridList targetGridOrigin = GridArray.Instance.gridArray[Mathf.RoundToInt(targetBridgeOrigin.x), Mathf.RoundToInt(targetBridgeOrigin.z)];
            GridList targetGridEnd = GridArray.Instance.gridArray[Mathf.RoundToInt(targetBridgeEnd.x), Mathf.RoundToInt(targetBridgeEnd.z)];
            targetGridOrigin.bridgeObjects.Remove(targetBridgeObject);
            targetGridEnd.bridgeObjects.Remove(targetBridgeObject);
            if (targetGridOrigin.bridgeObjects.Count == 0)
            {
                targetGridOrigin.structureObjects[0].GetComponent<StructureBehavior>().isBridged = false;
                targetGridOrigin.bridge = 0;
            }
            if (targetGridEnd.bridgeObjects.Count == 0)
            {
                targetGridEnd.structureObjects[0].GetComponent<StructureBehavior>().isBridged = false;
                targetGridEnd.bridge = 0;
            }
            Destroy(targetBridgeObject);
        }
        if (target.warningSystemEngaged)
        {
            Destroy(target.windParticle);
            target.warningSystemEngaged = false;
            particles.Remove(target.windParticle);
        }
        target.sizeY = heightLimit += target.branchedStructures;

    }

    void LookForStructures(GridList target, Vector3 targetVector)
    {
        if (GridArray.Instance.searchForStructure(targetVector, platformSearchDistance).Count == 0)
        {
            Destroy(target.foundationObject);
            target.foundationAmount = 0;
        }
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

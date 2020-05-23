using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCubeBehavior : MonoBehaviour
{
    
    [HideInInspector] public int amountOfAgents;
    [SerializeField] GameObject effect1;
    [SerializeField] GameObject effect2;
    [SerializeField] GameObject effect3;
    [SerializeField] GameObject selecterBridge;
    [SerializeField] Color sphereColor;
    [SerializeField] Color sphereColorSelect;
    [SerializeField] bool MegaCube;
    [HideInInspector] public bool spawned;
    [HideInInspector] public bool falling;
    [HideInInspector] public bool played;

    [SerializeField] AudioClip collected;
    [SerializeField] AudioClip dropping;
    private AudioSource audioSource;
    [SerializeField] AudioSource audioSourcePrefab;


    float cooldown;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        cooldown += Time.deltaTime;

        if (falling == false)
        {
            transform.GetChild(0).localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(0f, 0f, 0f), cooldown / ObjectiveSpawn.instance.objectiveCountdown);
            transform.GetChild(3).localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(0f, 0f, 0f), cooldown / ObjectiveSpawn.instance.objectiveCountdown);
            transform.GetChild(4).localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(0f, 0f, 0f), cooldown / ObjectiveSpawn.instance.objectiveCountdown);
            transform.GetChild(5).localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(0f, 0f, 0f), cooldown / ObjectiveSpawn.instance.objectiveCountdown);

            transform.GetChild(1).localScale = Vector3.Lerp(new Vector3(0.03333334f, -166, 0.03333334f), new Vector3(0, -166, 0), cooldown / ObjectiveSpawn.instance.objectiveCountdown);
            transform.GetChild(6).localScale = Vector3.Lerp(new Vector3(0.1666667f, 0.1666667f, 0.1666667f), new Vector3(0, 0.1666667f, 0), cooldown / ObjectiveSpawn.instance.objectiveCountdown);
        }

        if (cooldown >= ObjectiveSpawn.instance.objectiveCountdown && falling==false)
        {
            StartCoroutine(Falling());
        }
        
    }

    IEnumerator Falling()
    {
        while (true)
        {
            if (!audioSource.isPlaying && !played)
            {
                AudioSource audioSourceTemp=  Instantiate(audioSourcePrefab);
                audioSourceTemp.clip = dropping;
                audioSourceTemp.Play();
                played = true;
            }
            falling = true;
            Debug.Log("Cube is falling");
            transform.GetChild(0).localScale = Vector3.Lerp(transform.GetChild(0).localScale, new Vector3(1.5f, 1.5f, 1.5f), .05f);
            transform.GetChild(3).localScale = Vector3.Lerp(transform.GetChild(3).localScale, new Vector3(1.5f, 1.5f, 1.5f), .05f);
            transform.GetChild(4).localScale = Vector3.Lerp(transform.GetChild(4).localScale, new Vector3(1.5f, 1.5f, 1.5f), .05f);
            transform.GetChild(5).localScale = Vector3.Lerp(transform.GetChild(5).localScale, new Vector3(1.5f, 1.5f, 1.5f), .05f);
           
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
            

            yield return new WaitForEndOfFrame();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bridge") || other.CompareTag("ground") )
        {
            if (cooldown >= .25 && spawned == false && falling==false) 
            {
                spawned = true;
                Instantiate(effect1, this.transform.position, Quaternion.identity);
                GridArray.Instance.agentStack.agentAmountStructure += amountOfAgents;
                // hier collectable eingesammelt sound einfügen (neues objekt instantiaten oder die brücke other abspielen lassen)
                ObjectiveSpawn.instance.collectedCubes += 1;
                if (!audioSource.isPlaying && !played)
                {
                    AudioSource audioSourceTemp = Instantiate(audioSourcePrefab);
                    audioSourceTemp.clip = collected;
                    audioSourceTemp.Play();
                    played = true;
                   
                }
                if (MegaCube)
                {
                    CameraRig.instance.StartCoroutine(CameraRig.instance.WinningCameraAnimation());
                }
                else
                    ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(1));

                Destroy(this.gameObject);
            }
            else if(spawned==false && falling == false)
            {
                spawned = true;
                ObjectiveSpawn.instance.StartCoroutine(ObjectiveSpawn.instance.SpawnCube(1));
                Destroy(this.gameObject);
            }
            
        }
        

        if (other.gameObject.CompareTag("previewBridge"))
        {
            transform.GetChild(2).GetComponent<Renderer>().material.EnableKeyword("_Emission");
            transform.GetChild(2).GetComponent<Renderer>().material.SetColor("_EmissionColor", sphereColorSelect);

            Color color = transform.GetChild(2).GetComponent<MeshRenderer>().material.color;
            transform.GetChild(2).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.35f);

        }

    }



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("previewBridge"))
        {
            transform.GetChild(2).GetComponent<Renderer>().material.EnableKeyword("_Emission");
            transform.GetChild(2).GetComponent<Renderer>().material.SetColor("_EmissionColor", sphereColor);
  
            Color color = transform.GetChild(2).GetComponent<MeshRenderer>().material.color;
            transform.GetChild(2).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.05f);

        }



    }
}

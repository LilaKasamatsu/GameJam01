using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterSeconds());

    }

    private IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }
}

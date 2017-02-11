using UnityEngine;
using System.Collections;

public class PointerScript : MonoBehaviour
{

    public AudioClip click;
    public string wheelValue;


    // Use this for initialization
    void Start()
    {

        GetComponent<AudioSource>().playOnAwake = false;
        GetComponent<AudioSource>().clip = click;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "pin")
        {
            wheelValue = col.gameObject.transform.GetChild(0).tag;

            GameObject go = GameObject.Find("GameController");
            go.GetComponent<Main>().lastWedgeValue = wheelValue;
                
            GetComponent<AudioSource>().Play();
        }
    }
}

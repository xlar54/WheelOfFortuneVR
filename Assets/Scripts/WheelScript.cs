using UnityEngine;
using System.Collections;

public class WheelScript : MonoBehaviour {

    public AudioClip audienceClap;

    public bool allowSpin = false;
    public int currentPlayer = 0;
    public float power = 0;
    public float speed = 0;

	// Use this for initialization
	void Start () {

        GetComponent<AudioSource>().playOnAwake = false;
        GetComponent<AudioSource>().volume = 0.5f;
        GetComponent<AudioSource>().clip = audienceClap;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (allowSpin)
        {
            if (currentPlayer == 0)
            {
                if (speed == 0 && Input.GetButton("Fire1"))
                {
                    power = power + 0.5f;
                }

                if (speed == 0 && Input.GetButtonUp("Fire1"))
                {
                    speed = 180 + power;
                }
            }
            else
            {
                if (speed == 0)
                    speed = 185;
            }

            if (speed > 0)
            {
                transform.Rotate(0, speed * Time.deltaTime, 0);
                speed -= 0.3f;

                if (!GetComponent<AudioSource>().isPlaying)
                    GetComponent<AudioSource>().Play();


                if (speed <= 0)
                {
                    speed = 0;
                    power = 0;
                    allowSpin = false;
                    GameObject.Find("GameController").GetComponent<Main>().OnSpin();
                }
            }
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_flip : MonoBehaviour
{
    private GameObject Visible;
    [SerializeField]
    private GameObject face, back;
    private int aux = 0;
    private bool corRunning, facedUp;

    // Start is called before the first frame update
    void Start()
    {
        corRunning = false;
        Visible = face;
        facedUp = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ButtonPress()
    {
        if (!corRunning)
        {
            StartCoroutine(Flip());
        }
    }
    public void assignNewFrontCard(GameObject newFace, GameObject newBack)
    {
        face = newFace;
        back = newBack;
        Visible = face;
        facedUp = true;
        corRunning = false;

    }
    public IEnumerator Flip()
    {
        corRunning = true;
        aux = 0;
        if (facedUp)
        {
            for (float i = 0f; i <= 180f; i+= 5f)
            {
                Visible.transform.rotation = Quaternion.Euler(0f, i+aux, 0f);
                if (i == 90f)
                {
                    face.gameObject.SetActive(false);
                    back.gameObject.SetActive(true);
                    Visible = back;
                    aux = -180;
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (!facedUp)
        {
            for (float i = 0f; i <= 180f; i += 5f)
            {
                Visible.transform.rotation = Quaternion.Euler(0f, i+aux, 0f);
                if (i == 90f)
                {
                    back.gameObject.SetActive(false);
                    face.gameObject.SetActive(true);
                    Visible = face;
                    aux = -180;
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
        corRunning = false;

        facedUp = !facedUp;

    }
}

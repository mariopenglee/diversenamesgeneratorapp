using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class scr_NextCard : MonoBehaviour
{
    private scr_Swipe oSwipe;
    private GameObject oFirstCard;
    private scr_flip oFlip;
    public GameObject Frontside, Backside;
    public TextMeshProUGUI tname;
    public TextMeshProUGUI tipa;
    // Start is called before the first frame update
    void Start()
    {
        oSwipe = FindObjectOfType<scr_Swipe>();
        oFlip = FindObjectOfType<scr_flip>();
        oFirstCard = oSwipe.gameObject;
        oSwipe.aCardMoved += NextCard;
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

    }

    // Update is called once per frame
    void Update()
    {
        float MovedDist = oFirstCard.transform.localPosition.x;
        if (Mathf.Abs(MovedDist) > 0)
        {
            float step = Mathf.SmoothStep(0.8f, 1, Mathf.Abs(MovedDist) / (Screen.width / 2));
            transform.localScale = new Vector3(step, step, step);
        }
    }

    void NextCard()
    {
        gameObject.AddComponent<scr_Swipe>();
        oFlip.assignNewFrontCard(Frontside, Backside);
        Destroy(this);
    }

    public void SetText(string name, string ipa)
    {
        tname.text = name;
        tipa.text = "[" + ipa + "]";
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_backside : MonoBehaviour
{
    public List<GameObject> tabList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenTab(int aux)
    {
        for(int i = 0; i < tabList.Count; i++)
         {
            if (i == aux)
            {
                tabList[i].SetActive(true);
            }
            else { tabList[i].SetActive(false); }
            

        }
    }

}

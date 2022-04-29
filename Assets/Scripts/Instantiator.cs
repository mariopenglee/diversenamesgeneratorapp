using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class Instantiator : MonoBehaviour
{
    public bool Debugging;
    public GameObject cardPrefab;
    public List<DataStream> NamesList;

    FirebaseFirestore db;
    FirebaseFirestore storage;
    

    public class DataStream
    {
        public string name;
        public string ipa;
        public string altName;
        public string altIPA;
        public string orthography;
        public bool audioPerms;
        public string audioRef;
        public int genderPref; //<unknown=-1 male=0 fem=1 gen-neutral=2>
        public string langPrim;
        public string langOther;
        public string famous;
        public string notes;
        public string ecName;
        public string ecClass;
        public bool engPhon;



    }

    /// <summary>
    /// filter parameters
    /// </summary>
    private string search;
    private int genderFilter;
    private bool excSoundless;
    private bool excFeat;
    private bool reshuffling;

    private bool checkRunning;

    public void ReshuffleDeck(string fsearch = "", int fgender = 0, bool fExcSoundless = false, bool fExcFeat = false)
    {
        reshuffling = true;
        search = fsearch;
        genderFilter = fgender;
        excSoundless = fExcSoundless;
        excFeat = fExcFeat;
        StartCoroutine(fetchData());

    }


    public IEnumerator fetchData()
    {
        NamesList.Clear();
        Query query = db.Collection("TestNames");
        query.GetSnapshotAsync().ContinueWith(task =>
        {
            QuerySnapshot querySnapshot = task.Result;

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {


            }
        }

        );
        yield return new WaitForSeconds(1);

    }

    void AddCard()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        NamesList = new List<DataStream>();
        search = "";
        genderFilter = 0;
        excSoundless = false;
        excFeat = false;
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseFirestore.DefaultInstance;

        reshuffling = true;
        StartCoroutine(fetchData());
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (reshuffling)
            return;
        if (checkRunning)
            return;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Storage;
using Firebase.Extensions;
using UnityEngine.Networking;

public class Instantiator : MonoBehaviour
{
    public bool Debugging;
    public GameObject cardPrefab;
    public List<DataStream> NamesList;

    public GameObject loadingMask;
    public GameObject audioButton;
    public AudioSource sourceaudio;

    FirebaseFirestore db;
    FirebaseStorage storage;
    StorageReference storageReference;
    

    public class DataStream
    {
        public string name;
        public string ipa;
        public string altName;
        public string altIPA;
        public string orthography;
        public bool audioPerms;
        public string audioRef;
        public string genderPref; //<unknown=-1 male=0 fem=1 gen-neutral=2>
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

    // Start is called before the first frame update
    void Start()
    {
        NamesList = new List<DataStream>();
        search = "";
        genderFilter = 0;
        excSoundless = false;
        excFeat = false;
        checkRunning = false;
        db = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://random-names-generator.appspot.com/");
        reshuffling = true;
        StartCoroutine(fetchData());


    }
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
            bool excThis = false;
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Dictionary<string, object> nomis = documentSnapshot.ToDictionary();
                DataStream tempdata = new DataStream();

                foreach (KeyValuePair<string, object> pair in nomis)
                {
                    switch (pair.Key.ToString())
                    {
                        case "name":
                            {
                                if(pair.Value.ToString().ToLower().StartsWith(search))
                                tempdata.name = pair.Value.ToString();
                                break;
                            }
                        case "ipa":
                            {
                                tempdata.ipa = pair.Value.ToString();
                                break;
                            }
                        case "altIPA":
                            {
                                tempdata.altIPA = pair.Value.ToString();
                                break;
                            }
                        case "altName":
                            {
                                tempdata.altName = pair.Value.ToString();
                                break;
                            }
                        case "audio":
                            {
                                tempdata.audioRef = pair.Value.ToString();
                                break;
                            }
                        case "famous":
                            {
                                tempdata.famous = pair.Value.ToString();
                                break;
                            }
                        case "orthography":
                            {
                                tempdata.orthography = pair.Value.ToString();
                                break;
                            }
                        case "notes":
                            {
                                tempdata.notes = pair.Value.ToString();
                                break;
                            }
                        case "languagePrimary":
                            {
                                tempdata.langPrim = pair.Value.ToString();
                                break;
                            }
                        case "languageOther":
                            {
                                tempdata.langOther = pair.Value.ToString();
                                break;
                            }
                        case "genderPreference":
                            {
                                switch (int.Parse(pair.Value.ToString()))
                                {
                                    case -1:
                                        {
                                            tempdata.genderPref = "any";
                                            break;
                                        }
                                    case 0:
                                        {
                                            tempdata.genderPref = "generally masculine";
                                            break;
                                        }
                                    case 1:
                                        {
                                            tempdata.genderPref = "generally feminine";
                                            break;
                                        }
                                    default:
                                        Debug.Log("unknown gender type");
                                        break;
                                }

                                 ///<unknown=-1 male=0 fem=1 gen-neutral=2>
                                break;
                            }
                        case "audioPerms":
                            {
                                if (excSoundless && (int)pair.Value == 0)
                                    excThis = true;
                                else
                                    tempdata.audioPerms = (bool)pair.Value;
                                break;
                            }
                        case "engPhonemes":
                            {
                                if (excFeat && (int)pair.Value == 0)
                                    excThis = true;
                                else
                                    tempdata.engPhon = (bool)pair.Value;
                                break;
                            }
                        case "ecName":
                            {
                                tempdata.ecClass = pair.Value.ToString();
                                break;
                            }
                        case "ecClass":
                            {
                                tempdata.ecName = pair.Value.ToString();
                                break;
                            }
                        default:
                            
                            Debug.Log("Unknown field read");
                            break;
                            
                    }
                    


                }

                if (!excThis)
                {
                    NamesList.Add(tempdata);
                }
                else { Debug.Log("entry excluded because of filter constrains"); }
            }
        }

        );
        yield return new WaitForSeconds(1);

    }

    public void RefToAudio(string text)
    {
        if (text.StartsWith("/projects/random-names-generator/databases/(default)/random-names-generator.appspot.com/"))
            text = text.Remove(0, 88);
        Debug.Log(text);
        StorageReference clip = storageReference.Child(text);

        clip.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(LoadAudio(task.Result.ToString()));
            }
            else
            { Debug.Log(task.Exception); }
        }
        );
        
    }

    IEnumerator LoadAudio(string mediaURL)
    {
        Debug.Log("downloading");
        loadingMask.SetActive(true);
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(mediaURL, AudioType.WAV);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(request.error);
        else //successful download
        {
            //Debug.Log("download completed" + DownloadHandlerAudioClip.GetContent(request).GetType().Name);
            sourceaudio.clip = DownloadHandlerAudioClip.GetContent(request);
            audioButton.SetActive(true);
        }
        loadingMask.SetActive(false); //if download ends

    }


    void AddCard()
    {
        GameObject newCard = Instantiate(cardPrefab, transform, false);
        newCard.transform.SetAsFirstSibling();
        int fate = Random.Range(0, NamesList.Count);
        newCard.GetComponent<scr_NextCard>().SetText(NamesList[fate]);
    }


    // Update is called once per frame
    void Update()
    {
        if (reshuffling)
            return;
        if (checkRunning)
            return;
        StartCoroutine(checkIfAdd());
        
    }
    IEnumerator checkIfAdd()
    {
        checkRunning = true;
        int counter = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "card")
                counter++;
        }
        if (counter > 2)
            AddCard();

        yield return null;
        checkRunning = false;
    }
}

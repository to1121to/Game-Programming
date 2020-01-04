using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;

public class GameController : MonoBehaviour
{
    GameObject ItemCanvas;
    bool ItemFlag;
    bool LabMode;

    bool SceneChangeAnimationFlag;//離開
    bool SceneChangeAnimationFlag2;//開場
    string NextScene;
    public float nextx;

    GameObject SceneChangeMask;
    GameObject SceneChangeCanvas;

    GameObject MessageCanvas;
    int MessageNumber;//總共幾句話
    int MessageNow;//現在第幾句
    bool MessageFlag;
    List<string> MessageArray;//message儲存用
    GameObject MessageBackground;
    GameObject MessageText;

    public GameObject Player;//player原型
    public GameObject ItemBack;//道具格原型
    public GameObject ItemFrame;//選擇框原型
    GameObject[] ShowItem;//儲存道具格子
    int[] ShowItemID;//儲存每個格子是第幾號道具
    GameObject SelectFrame;//選擇框
    GameObject CurrentPlayer;
    GameObject ItemInfo;//道具說明的text
    GameObject ItemName;//道具名稱的text
    bool Lock;//看lab資料時用來鎖定特定lab以換頁
    int LabPage;//lab資料共有幾頁
    int LabPageNow;//lab資料目前為第幾頁

    float window_width;
    float window_height;
    int SelectedItem;//選擇第幾個道具，0-7
    public AudioClip SE;
    AudioSource Audio;

    [Serializable]
    public struct EventData
    {
        public int EventID;
        public bool Interactable;
        public bool Reinteractable;
        public string[] Message;
        public string[] Message2;
        public int ItemAmount;
        public int[] ItemID;
        public int LabAmount;
        public int[] LabID;
    }
    [Serializable]
    public struct EventArray
    {
        public List<EventData> EventList;
    }
    [Serializable]
    public struct ItemData
    {
        public int ItemID;
        public string ItemName;
        public string ItemInfo;
    }
    [Serializable]
    public struct ItemArray
    {
        public List<ItemData> ItemList;
    }
    [Serializable]
    public struct LabData
    {
        public int LabID;
        public string LabName;
        public string[] LabInfo;
    }
    [Serializable]
    public struct LabArray
    {
        public List<LabData> LabList;
    }

    EventArray Events;
    ItemArray Items;
    LabArray Labs;

    readonly int EventClass = 8;//總共event種類
    List<bool> EventTriggered;//event是否已被觸發
    readonly int ItemClass = 8;//總共item種類
    List<int> ItemAmount;//item數量
    readonly int LabClass = 8;//總共lab種類
    List<bool> LabGotten;//是否已取得lab
    int ItemPage;//顯示道具的第幾頁，尚未實作

    static GameController instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != instance)
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        LoadEvent();
        LoadItem();
        LoadLab();
        ItemCanvas = GameObject.FindGameObjectWithTag("ItemCanvas");
        ItemInfo = GameObject.FindGameObjectWithTag("ItemInfo");
        ItemName = GameObject.FindGameObjectWithTag("ItemName");
        MessageCanvas = GameObject.FindGameObjectWithTag("MessageCanvas");
        MessageBackground = GameObject.FindGameObjectWithTag("MessageBackground");
        MessageText = GameObject.FindGameObjectWithTag("Message");
        Audio = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();

        ItemCanvas.SetActive(false);
        MessageCanvas.SetActive(false);
        ItemFlag = false;
        LabMode = false;
        MessageFlag = false;
        SceneChangeAnimationFlag = false;
        SceneChangeAnimationFlag2 = true;
        CurrentPlayer = GameObject.FindGameObjectWithTag("Player");
        SceneChangeCanvas = GameObject.FindGameObjectWithTag("SceneChangeCanvas");
        SceneChangeMask = GameObject.FindGameObjectWithTag("SceneChangeMask");

        if (CurrentPlayer == null)
        {
            CurrentPlayer = Instantiate(Player, new Vector2(0f, -1.89f), Quaternion.identity);
        }
        else
        {
            CurrentPlayer.transform.position = new Vector2(nextx, -1.89f);
        }
        ShowItem = new GameObject[8];
        ShowItemID = new int[8];
        SelectedItem = 0;
        ItemPage = 0;
        EventTriggered = new List<bool>();
        ItemAmount = new List<int>();
        LabGotten = new List<bool>();
        MessageNow = 0;
        LabPageNow = 0;
        for (int i = 0; i < ItemClass; i++)
        {
            ItemAmount.Add(0);
        }
        for (int i = 0; i < LabClass; i++)
        {
            LabGotten.Add(false);
        }
        for (int i = 0; i < EventClass; i++)
        {
            EventTriggered.Add(false);
        }
        MessageArray = new List<string>();
        SetItemCanvas();
        SetMessageCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("OpenItem") && !SceneChangeAnimationFlag2 && !MessageFlag)
        {
            if (!ItemFlag)
            {
                ItemCanvas.SetActive(true);
                Time.timeScale = 0;
                ItemFlag = true;
            }
            else
            {
                ItemCanvas.SetActive(false);
                Time.timeScale = 1.0f;
                ItemFlag = false;
                SelectedItem = 0;
            }
        }
        if (ItemFlag)
        {
            SetItemCanvas();
            if (!Lock)
            {
                if (Input.GetButtonDown("Left"))
                {
                    if (SelectedItem % 2 == 1) SelectedItem--;
                    else SelectedItem++;
                    LabPageNow = 0;
                }
                if (Input.GetButtonDown("Right"))
                {
                    if (SelectedItem % 2 == 0) SelectedItem++;
                    else SelectedItem--;
                    LabPageNow = 0;
                }
                if (Input.GetButtonDown("Up"))
                {
                    if (SelectedItem / 2 != 0) SelectedItem -= 2;
                    else SelectedItem += 6;
                    LabPageNow = 0;
                }
                if (Input.GetButtonDown("Down"))
                {
                    if (SelectedItem / 2 != 3) SelectedItem += 2;
                    else SelectedItem -= 6;
                    LabPageNow = 0;
                }
            }
            else
            {
                if (Input.GetButtonDown("Left"))
                {
                    if (LabPageNow == 0) LabPageNow = LabPage;
                    else LabPageNow--;
                }
                if (Input.GetButtonDown("Right"))
                {
                    if (LabPageNow == LabPage) LabPageNow = 0;
                    else LabPageNow++;
                }
            }
            if (Input.GetButtonDown("Next") && EventTriggered[1])
            {
                LabMode = !LabMode;
                if (!LabMode) Lock = false;
            }
            if (Input.GetButtonDown("Choose") && LabMode)
            {
                Lock = !Lock;
            }
        }
        if (MessageFlag)
        {
            SetMessageCanvas();
            if (Input.GetButtonDown("Next"))
            {
                MessageNow++;
                if (MessageNow == MessageNumber) MessageCanvasControl();
            }
        }
    }
    private void FixedUpdate()
    {
        /*if (SceneChangeAnimationFlag)
        {
            SceneChangeMask.GetComponent<RectTransform>().localScale -= new Vector3(0.4f, 0.4f);
            SetMaskPosition(CurrentPlayer);
            if (SceneChangeMask.GetComponent<RectTransform>().localScale.x <= 0)
            {
                SceneChangeAnimationFlag = false;
                SceneManager.LoadScene(NextScene);
            }
        }
        if (SceneChangeAnimationFlag2)
        {
            SceneChangeMask.GetComponent<RectTransform>().localScale += new Vector3(0.4f, 0.4f);
            SetMaskPosition(CurrentPlayer);
            if (SceneChangeMask.GetComponent<RectTransform>().sizeDelta.x * SceneChangeMask.GetComponent<RectTransform>().localScale.x >= window_width * 2)
            {
                SceneChangeAnimationFlag2 = false;
                SceneChangeCanvas.SetActive(false);
            }
        }
        //SetMaskPosition(CurrentPlayer);*/
        if (SceneChangeAnimationFlag)
        {
            SceneChangeMask.GetComponent<RectTransform>().localScale -= new Vector3(0.4f, 0.4f);
            SetMaskPosition(CurrentPlayer);
            if (SceneChangeMask.GetComponent<RectTransform>().localScale.x <= 0)
            {
                SceneChangeAnimationFlag = false;
                SceneManager.LoadScene(NextScene);
            }
        }
        if (SceneChangeAnimationFlag2)
        {
            SceneChangeMask.GetComponent<RectTransform>().localScale += new Vector3(0.4f, 0.4f);
            SetMaskPosition(CurrentPlayer);
            if (SceneChangeMask.GetComponent<RectTransform>().sizeDelta.x * SceneChangeMask.GetComponent<RectTransform>().localScale.x >= window_width * 2)
            {
                SceneChangeAnimationFlag2 = false;
                SceneChangeCanvas.SetActive(false);
            }
        }
        //SetMaskPosition(CurrentPlayer);
    }
    public void ChangeScene(string SceneName)
    {
        if(!SceneChangeAnimationFlag2)
        {
            NextScene = SceneName;
            SceneChangeCanvas.SetActive(true);
            SceneChangeAnimationFlag = true;
            SetMaskPosition(CurrentPlayer);
        }
    }
    /*void SetMaskPosition(GameObject player)
    {
        RectTransform r = SceneChangeCanvas.GetComponent<RectTransform>();
        Vector2 screenPos = Camera.main.WorldToViewportPoint(player.transform.position);
        Vector2 viewPos = (screenPos - r.pivot) * 2;
        float width = r.rect.width / 2;
        float height = r.rect.height / 2;
        SceneChangeMask.GetComponent<RectTransform>().anchoredPosition = new Vector2(viewPos.x * width, viewPos.y * height);
    }*/
    void SetItemCanvas()
    {
        window_width = Screen.width;
        window_height = Screen.height;
        GameObject ItemBackground = ItemCanvas.transform.GetChild(0).gameObject;
        RectTransform rt = ItemBackground.GetComponent<RectTransform>();
        float width = window_width / 10;
        float height = window_height / 10;
        rt.offsetMin = new Vector2(width, height);
        rt.offsetMax = new Vector2(-1 * width, -1 * height);
        width = window_width - width * 2;
        height = window_height - height * 2;
        for(int i = 0; i < 8; i++)
        {
            ShowItemID[i] = -1;
        }
        for (int i = 0; i < 8; i++)
        {
            int ItemCount = 0;
            if (ShowItem[i] == null)
            {
                GameObject newItem = Instantiate(ItemBack, ItemBackground.transform);
                RectTransform rt2 = newItem.GetComponent<RectTransform>();
                rt2.anchorMax = new Vector2(0, 0);
                rt2.anchorMin = new Vector2(0, 0);
                rt2.offsetMax = new Vector2(width / 20 * (i % 2 + 1) + (height / 16 * 3 * (i % 2 + 1)), (height / 20 * (4 - i / 2)) + (height / 16 * 3 * (4 - i / 2)));
                rt2.offsetMin = new Vector2(width / 20 * (i % 2 + 1) + (height / 16 * 3 * (i % 2)), (height / 20 * (4 - i / 2)) + (height / 16 * 3 * (3 - i / 2)));
                ShowItem[i] = newItem;
            }
            else
            {
                ShowItem[i].GetComponent<RectTransform>().offsetMax = new Vector2(width / 20 * (i % 2 + 1) + (height / 16 * 3 * (i % 2 + 1)), (height / 20 * (4 - i / 2)) + (height / 16 * 3 * (4 - i / 2)));
                ShowItem[i].GetComponent<RectTransform>().offsetMin = new Vector2(width / 20 * (i % 2 + 1) + (height / 16 * 3 * (i % 2)), (height / 20 * (4 - i / 2)) + (height / 16 * 3 * (3 - i / 2)));
            }
            if (!LabMode)
            {
                for (int j = 0; j < ItemClass; j++)
                {
                    if (ItemAmount[j] > 0)
                    {
                        ItemCount++;
                        if (ItemCount > i + 8 * ItemPage && ShowItemID[i] == -1)
                        {
                            ItemData GetItem = GetItemData(j);
                            ShowItem[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(GetItem.ItemName);
                            ShowItemID[i] = j;
                        }
                    }
                }
            }
            else
            {
                if(LabGotten[i + 8 * ItemPage])
                {
                    LabData GetLab = GetLabData(i + 8 * ItemPage);
                    ShowItem[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(GetLab.LabName);
                    ShowItemID[i] = i + 8 * ItemPage;
                }
            }
        }
        if(SelectFrame == null)
        {
            SelectFrame = Instantiate(ItemFrame, ItemBackground.transform);
            RectTransform rt2 = SelectFrame.GetComponent<RectTransform>();
            rt2.anchorMax = new Vector2(0, 0);
            rt2.anchorMin = new Vector2(0, 0);
            float framewidth = width / 60;
            rt2.offsetMax = new Vector2(width / 20 * (SelectedItem % 2 + 1) + (height / 16 * 3 * (SelectedItem % 2 + 1)) + framewidth, (height / 20 * (4 - SelectedItem / 2)) + (height / 16 * 3 * (4 - SelectedItem / 2)) + framewidth);
            rt2.offsetMin = new Vector2(width / 20 * (SelectedItem % 2 + 1) + (height / 16 * 3 * (SelectedItem % 2)) - framewidth, (height / 20 * (4 - SelectedItem / 2)) + (height / 16 * 3 * (3 - SelectedItem / 2)) - framewidth);
        }
        else
        {
            RectTransform rt2 = SelectFrame.GetComponent<RectTransform>();
            float framewidth = width / 60;
            rt2.offsetMax = new Vector2(width / 20 * (SelectedItem % 2 + 1) + (height / 16 * 3 * (SelectedItem % 2 + 1)) + framewidth, (height / 20 * (4 - SelectedItem / 2)) + (height / 16 * 3 * (4 - SelectedItem / 2)) + framewidth);
            rt2.offsetMin = new Vector2(width / 20 * (SelectedItem % 2 + 1) + (height / 16 * 3 * (SelectedItem % 2)) - framewidth, (height / 20 * (4 - SelectedItem / 2)) + (height / 16 * 3 * (3 - SelectedItem / 2)) - framewidth);
        }
        RectTransform rt3 = ItemName.GetComponent<RectTransform>();
        rt3.offsetMax = new Vector2(width / 12 * 11, height / 10 * 9);
        rt3.offsetMin = new Vector2(width / 12 * 5, height / 10 * 7);
        RectTransform rt4 = ItemInfo.GetComponent<RectTransform>();
        rt4.offsetMax = new Vector2(width / 12 * 11, height / 10 * 7);
        rt4.offsetMin = new Vector2(width / 12 * 5, height / 10 * 1);
        if(ShowItemID[SelectedItem] != -1)
        {
            if (!LabMode)
            {
                ItemData GetItem2 = GetItemData(ShowItemID[SelectedItem]);
                ItemName.GetComponent<TMPro.TextMeshProUGUI>().text = GetItem2.ItemName;
                ItemInfo.GetComponent<TMPro.TextMeshProUGUI>().text = GetItem2.ItemInfo;
            }
            else
            {
                LabData GetLab2 = GetLabData(ShowItemID[SelectedItem]);
                LabPage = GetLab2.LabInfo.Length - 1;
                ItemName.GetComponent<TMPro.TextMeshProUGUI>().text = GetLab2.LabName;
                ItemInfo.GetComponent<TMPro.TextMeshProUGUI>().text = GetLab2.LabInfo[LabPageNow];
            }
        }
        else
        {
            ItemName.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            ItemInfo.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }
    }
    public bool Interactable(int EventID)
    {
        EventData GetEvent = Events.EventList.Find(x => x.EventID == EventID);
        return GetEvent.Interactable;
    }
    public bool HadItem(int ItemID)
    {
        if (ItemID == -1) return true;
        return (ItemAmount[ItemID] > 0);
    }
    public void EventTrigger(int EventID, bool correct)
    {
        EventData GetEvent = Events.EventList.Find(x => x.EventID == EventID);
        if (correct)
        {
            Audio.PlayOneShot(SE);
            if (!GetEvent.Reinteractable)
            {
                GetEvent.Interactable = false;
                Events.EventList[Events.EventList.FindIndex(x => x.EventID == EventID)] = GetEvent;
            }
            for (int i = 0; i < GetEvent.ItemAmount; i++)
            {
                ItemAmount[GetEvent.ItemID[i]]++;
            }
            for (int i = 0; i < GetEvent.LabAmount; i++)
            {
                LabGotten[GetEvent.LabID[i]] = true;
            }
            ShowMessage(GetEvent.Message);
            EventTriggered[GetEvent.EventID] = true;
        }
        else ShowMessage(GetEvent.Message2);
        CurrentPlayer.GetComponent<PlayerController>().Event = false;
    }
    public bool EventExecuted(int EventID)
    {
        if (EventID == -1) return true;
        return EventTriggered[EventID];
    }
    public void UseItem(int ItemID)
    {
        if(ItemAmount[ItemID] > 0) ItemAmount[ItemID]--;
    }
    void LoadEvent()
    {
        string LoadJson = File.ReadAllText(Application.streamingAssetsPath + "/Event.json");
        Events = JsonUtility.FromJson<EventArray>(LoadJson);
        for(int i = 0; i < Events.EventList.Count; i++)
        {
            for(int j = 0; j < Events.EventList[i].Message.Length; j++)
            {
                Debug.Log(Events.EventList[i].Message[j]);
            }
        }
    }
    void LoadItem()
    {
        //StringReader file = new StringReader(Application.dataPath + "/Resources/Event.json"));
        string LoadJson = File.ReadAllText(Application.streamingAssetsPath + "/Item.json");
        //file.Close();
        Items = JsonUtility.FromJson<ItemArray>(LoadJson);
    }
    void LoadLab()
    {
        //StringReader file = new StringReader(Application.dataPath + "/Resources/Event.json"));
        string LoadJson = File.ReadAllText(Application.streamingAssetsPath + "/Lab.json");
        //file.Close();
        Labs = JsonUtility.FromJson<LabArray>(LoadJson);
    }
    void SetMessageCanvas()
    {
        window_width = Screen.width;
        window_height = Screen.height;
        
        RectTransform rt = MessageBackground.GetComponent<RectTransform>();
        float width = window_width / 10;
        float height = window_height / 10;
        rt.offsetMin = new Vector2(width, height / 2);
        rt.offsetMax = new Vector2(-1 * width, -7 * height);
        
        MessageText.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        MessageText.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        if(MessageNow != MessageNumber) MessageText.GetComponent<TMPro.TextMeshProUGUI>().text = MessageArray[MessageNow];
    }
    public void ShowMessage(string[] Message)
    {
        MessageArray.Clear();
        for (int i = 0; i < Message.Length; i++)
        {
            MessageArray.Add(Message[i]);
        }
        MessageNumber = Message.Length;
        MessageCanvasControl();
    }
    void MessageCanvasControl()
    {
        if (!MessageFlag)
        {
            MessageCanvas.SetActive(true);
            MessageFlag = true;
        }
        else
        {
            MessageCanvas.SetActive(false);
            MessageFlag = false;
            MessageNow = 0;
        }
    }
    ItemData GetItemData(int ItemID)
    {
        return Items.ItemList.Find(x => x.ItemID == ItemID);
    }
    LabData GetLabData(int LabID)
    {
        return Labs.LabList.Find(x => x.LabID == LabID);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) return;
        ItemCanvas = GameObject.FindGameObjectWithTag("ItemCanvas");
        ItemInfo = GameObject.FindGameObjectWithTag("ItemInfo");
        ItemName = GameObject.FindGameObjectWithTag("ItemName");
        MessageCanvas = GameObject.FindGameObjectWithTag("MessageCanvas");
        MessageBackground = GameObject.FindGameObjectWithTag("MessageBackground");
        MessageText = GameObject.FindGameObjectWithTag("Message");
        ItemCanvas.SetActive(false);
        MessageCanvas.SetActive(false);
        ItemFlag = false;
        LabMode = false;
        MessageFlag = false;
        SceneChangeAnimationFlag = false;
        SceneChangeAnimationFlag2 = true;
        CurrentPlayer = GameObject.FindGameObjectWithTag("Player");
        SceneChangeCanvas = GameObject.FindGameObjectWithTag("SceneChangeCanvas");
        SceneChangeMask = GameObject.FindGameObjectWithTag("SceneChangeMask");

        if (CurrentPlayer == null)
        {
            CurrentPlayer = Instantiate(Player, new Vector2(5f, -1.89f), Quaternion.identity);
        }
        else
        {
            CurrentPlayer.transform.position = new Vector2(nextx, -1.89f);
        }
        SelectedItem = 0;
        ItemPage = 0;
        MessageNow = 0;
        LabPageNow = 0;
        SetItemCanvas();
        SetMessageCanvas();
    }
}

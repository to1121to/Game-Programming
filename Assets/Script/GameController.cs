using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject Item;
    bool ItemFlag;
    bool SceneChangeAnimationFlag;
    bool SceneChangeAnimationFlag2;
    string NextScene;

    public GameObject SceneChangeMask;
    public GameObject SceneChangeCanvas;
    public GameObject Player;
    public GameObject ItemBack;
    GameObject[] ShowItem;
    public Sprite[] ItemImage;
    GameObject CurrentPlayer;
    // Start is called before the first frame update
    void Start()
    {
        Item.SetActive(false);
        ItemFlag = false;
        SceneChangeAnimationFlag = false;
        SceneChangeAnimationFlag2 = true;
        CurrentPlayer = GameObject.FindGameObjectWithTag("Player");
        if (CurrentPlayer == null)
        {
            CurrentPlayer = Instantiate(Player, new Vector2(-5f, -2.3f), Quaternion.identity);
            SetMaskPosition(CurrentPlayer);
        }
        else
        {
            CurrentPlayer.transform.position = new Vector2(CurrentPlayer.GetComponent<PlayerController>().nextx, -2.3f);
            SetMaskPosition(CurrentPlayer);
            GameObject[] Items = GameObject.FindGameObjectsWithTag("Item");
            for(int i = 0; i < Items.Length; i++)
            {
                if (CurrentPlayer.GetComponent<PlayerController>().GotItem[Items[i].GetComponent<ItemController>().ItemID] != 0)
                {
                    Destroy(Items[i]);
                }
            }
        }
        ShowItem = new GameObject[8];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!ItemFlag)
            {
                Item.SetActive(true);
                Time.timeScale = 0;
                ItemFlag = true;
                for(int i = 0; i < 8; i++)
                {
                    if(ShowItem[i] == null)
                    {
                        GameObject newItem = Instantiate(ItemBack, GameObject.Find("Item").transform);
                        newItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100 + 66.6f * (i % 4), 80 - 66.7f * (i / 4));
                        ShowItem[i] = newItem;
                    }
                    
                    if(CurrentPlayer.GetComponent<PlayerController>().GotItem[i] != 0)
                    {
                        ShowItem[i].GetComponent<Image>().sprite = ItemImage[i];
                    }
                }
            }
            else
            {
                Item.SetActive(false);
                Time.timeScale = 1.0f;
                ItemFlag = false;
            }
        }
    }
    private void FixedUpdate()
    {
        if (SceneChangeAnimationFlag)
        {
            SceneChangeMask.GetComponent<RectTransform>().localScale -= new Vector3(0.2f, 0.2f);
            //SceneChangeMask.GetComponent<RectTransform>().position = gameObject.transform.position;
            if (SceneChangeMask.GetComponent<RectTransform>().localScale.x <= 0)
            {
                SceneChangeAnimationFlag = false;
                SceneManager.LoadScene(NextScene);
            }
        }
        if (SceneChangeAnimationFlag2)
        {
            SceneChangeMask.GetComponent<RectTransform>().localScale += new Vector3(0.2f, 0.2f);
            //SceneChangeMask.GetComponent<RectTransform>().position = gameObject.transform.position;
            if (SceneChangeMask.GetComponent<RectTransform>().localScale.x >= 12)
            {
                SceneChangeAnimationFlag2 = false;
            }
        }
    }
    public void ChangeScene(string SceneName)
    {
        NextScene = SceneName;
        SceneChangeAnimationFlag = true;
        SetMaskPosition(CurrentPlayer);
    }
    void SetMaskPosition(GameObject player)
    {
        RectTransform r = SceneChangeCanvas.GetComponent<RectTransform>();
        Vector2 screenPos = Camera.main.WorldToViewportPoint(player.transform.position);
        Vector2 viewPos = (screenPos - r.pivot) * 2;
        float width = r.rect.width / 2;
        float height = r.rect.height / 2;
        SceneChangeMask.GetComponent<RectTransform>().anchoredPosition = new Vector2(viewPos.x * width, viewPos.y * height);
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using StarterAssets;


public class InventoryItem : MonoBehaviour
{
    public static Action<InventoryItem> onSelected;

    // Power-Up values
    [HideInInspector] public float JumpPercentage;
    [HideInInspector] public float MovementPercentage;
    [HideInInspector] public int ExtraTime;
    // Metadata values
    [HideInInspector] public string myTokenId;
    [HideInInspector] public MetadataObject myMetadataObject;

    [Header("UI Components")]
    [SerializeField] private Image myIcon;
    [SerializeField] private Button myButton;
    [SerializeField] private ThirdPersonController thirdPersonController;
    bool isSelected = false;

    private void Start()
    {
        thirdPersonController = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
    }
    public void Init(string tokenId, MetadataObject metadataObject)
    {
        myTokenId = tokenId;
        myMetadataObject = metadataObject;
            
        if (myMetadataObject.attributes is null)
        {
            Debug.Log("No attributes found");
            return;
        }
        //Debug.Log("========2=========");
        // Get Power-Up values
        foreach (var attribute in myMetadataObject.attributes)
        {
            //Debug.Log(attribute.value);
            //Debug.Log(attribute);
            if (attribute.display_type == "Booster")
            {
                if (attribute.trait_type == "Jump")
                {
                    Debug.Log(attribute.value);
                    JumpPercentage = float.Parse(attribute.value);   
                }
            }
            else
            {
                Debug.Log("No Jump Booster");
            }

            if (attribute.display_type == "Booster")
            {
                if (attribute.trait_type == "Speed")
                {
                    Debug.Log(attribute.value);
                    MovementPercentage = float.Parse(attribute.value);
                }
            }
            else
            {
                Debug.Log("No Speed Booster");
            }

            if (attribute.display_type == "Booster")
            {
                if (attribute.trait_type == "Time")
                {
                    Debug.Log(attribute.value);
                    ExtraTime = int.Parse(attribute.value);
                }
            }
            else
            {
                Debug.Log("No Time Booster");
            }
        }
        

        string imageurl = myMetadataObject.image.Replace("ipfs://", "");
        Debug.Log(imageurl);
        StartCoroutine(GetTexture("https://cloudflare-ipfs.com/ipfs/" + imageurl));
    }

    public void Selected()
    {
        onSelected?.Invoke(this);
        if (isSelected == false)
        {
            isSelected = true;
            Debug.Log("Selected");
            Debug.Log($"Jump: {JumpPercentage} Move: {MovementPercentage} Extra Time: {ExtraTime}");
            myIcon.color = new Color(0.2f,0.2f,0.2f) ;
            //IconMove();
            AbilityBoost(JumpPercentage, MovementPercentage);
        }
        else if (isSelected == true)
        {
            isSelected = false;
            Debug.Log("UnSelected");
            myIcon.color = new Color(1,1,1);
            //IconMove();
            RemoveBoost(JumpPercentage, MovementPercentage);
        }

        
    }
    private void IconMove()
    {
        gameObject.transform.DOMove(new Vector3(450, 143, 0), 0.5f);
    }
    private void RemoveBoost(float jump, float movement)
    {
        thirdPersonController.JumpHeight /= (100 + jump) / 100;
        thirdPersonController.MoveSpeed /= (100 + movement) / 100;
    }
    public void AbilityBoost(float jump, float movement)
    {
        thirdPersonController.JumpHeight *= (100+jump) / 100;
        thirdPersonController.MoveSpeed *= (100 + movement) / 100;
    }

    private IEnumerator GetTexture(string imageUrl)
    {
        using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl);
        
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
            uwr.Dispose();
        }
        else
        {
            var tex = DownloadHandlerTexture.GetContent(uwr);
            myIcon.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.height);
                
            //Now we are able to click the button and we will pass the loaded sprite :)
            myIcon.gameObject.SetActive(true);
            myButton.interactable = true;
            
            uwr.Dispose();
        }
    }
}   

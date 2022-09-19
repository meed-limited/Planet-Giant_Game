using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using StarterAssets;
using TMPro;


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
    private Timer _timer;
    bool isSelected = false;
    
    

    int _abilitySelectLimit = 3;
    


    private TMPro.TextMeshProUGUI _speedBoostText;
    private TMPro.TextMeshProUGUI _jumpBoostText;
    private TMPro.TextMeshProUGUI _timeBoostText;
    private TMPro.TextMeshProUGUI _selectLimit;

    private void Start()
    {
        thirdPersonController = GameObject.Find("Model"+PlayerPrefs.GetInt("Selected")).GetComponent<ThirdPersonController>();
        _timer = GameObject.Find("Timer").GetComponent<Timer>();
        _speedBoostText = GameObject.Find("SpeedBoost").GetComponent<TMPro.TextMeshProUGUI>();
        _jumpBoostText = GameObject.Find("JumpBoost").GetComponent<TMPro.TextMeshProUGUI>();
        _timeBoostText = GameObject.Find("TimeBoost").GetComponent<TMPro.TextMeshProUGUI>();
        _selectLimit = GameObject.Find("SelectLimit").GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!isSelected && _timer._currentSelected == 3)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
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

        foreach (var attribute in myMetadataObject.attributes)
        {

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
        StartCoroutine(GetTexture("https://ipfs.io/ipfs/" + imageurl));
    }

    public void Selected()
    {

        onSelected?.Invoke(this);
        if (isSelected == false )
        {
            isSelected = true;
            //Debug.Log("Selected");
            Debug.Log($"Jump: {JumpPercentage} Move: {MovementPercentage} Extra Time: {ExtraTime}");
            myIcon.color = new Color(0.2f, 0.2f, 0.2f);
            //IconMove();
            AbilityBoost(JumpPercentage, MovementPercentage, ExtraTime);
            _timer._currentSelected += 1;
            _selectLimit.text = _timer._currentSelected.ToString() + "/3";
            Debug.Log(_timer._currentSelected);
        }
        else if (isSelected == true)
        {
            isSelected = false;
            Debug.Log("UnSelected");
            myIcon.color = new Color(1, 1, 1);
            //IconMove();
            _timer._currentSelected -= 1;
            _selectLimit.text = _timer._currentSelected.ToString() + "/3";
            RemoveBoost(JumpPercentage, MovementPercentage, ExtraTime);
        }


        
    }
    private void IconMove()
    {
        gameObject.transform.DOMove(new Vector3(450, 143, 0), 0.5f);
    }
    private void RemoveBoost(float jump, float movement, int time)
    {
        thirdPersonController.JumpHeight /= (100 + jump) / 100;
        thirdPersonController.MoveSpeed /= (100 + movement) / 100;
        _timer._timeRemaining -= time;
        _timer._totaljumpboosted -= jump;
        _timer._totalspeedboosted -= movement;
        _timer._totaltimeboosted -= time;
        _speedBoostText.text = "+" + _timer._totalspeedboosted + "%";
        _jumpBoostText.text = "+" + _timer._totaljumpboosted + "%";
        _timeBoostText.text = "+" + _timer._totaltimeboosted + "s";
    }
    public void AbilityBoost(float jump, float movement, int time)
    {
        thirdPersonController.JumpHeight *= (100+jump) / 100;
        _timer._totaljumpboosted += jump;

        thirdPersonController.MoveSpeed *= (100 + movement) / 100;
        _timer._totalspeedboosted += movement;

        _timer._timeRemaining += time;
        _timer._totaltimeboosted += time;

        _speedBoostText.text = "+" + _timer._totalspeedboosted + "%";
        _jumpBoostText.text = "+" + _timer._totaljumpboosted + "%";
        _timeBoostText.text = "+" + _timer._totaltimeboosted + "s";
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

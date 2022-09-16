using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using StarterAssets;

using MoralisUnity;
using MoralisUnity.Web3Api;
using MoralisUnity.Platform.Objects;

using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;
using MoralisUnity.Web3Api.Models;

using Newtonsoft.Json;
using JetBrains.Annotations;
using System;

using System.Linq;
using UnityEngine.SceneManagement;
using Nethereum.Contracts;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

public class MetadataObject
{
    public string name { get; set; }
    public string description { get; set; }
    public string image { get; set; }
    [CanBeNull] public List<AttributeObject> attributes { get; set; }
}

public class AttributeObject
{
    [CanBeNull] public string display_type { get; set; }
    public string trait_type { get; set; }
    public string value { get; set; }
}
public class MoralisHandling : MonoBehaviour
{

    [Header("WEB3")]
    private StarterAssetsInputs _startAssetInputs;
    [Header("UI")]
    [SerializeField] TMPro.TextMeshPro textMesh;
    //[SerializeField] GameObject _background;
    MoralisUser moralisUser;
    bool isLoggedIn = false;
    private string userAddress;
    private ChainList chainId;
    private InventoryItem _currentSelectedItem;
    private int _currentItemsCount;
    [SerializeField] private Transform content;
    [SerializeField] private InventoryItem itemPrefab;
    [SerializeField] private GameObject abilityBoard;
    private string _mintNftAddress = "0x8208Bb3a2e25310BAd343fD80968F061F23a707B";

    private void Awake()
    {
        Moralis.Start();
        chainId = ChainList.cronos_testnet;
    }
    private void Start()
    {
        int index = PlayerPrefs.GetInt("CharSelected", 0);
        textMesh = GameObject.Find("Address" + PlayerPrefs.GetInt("Selected").ToString()).GetComponent<TextMeshPro>();
        _startAssetInputs = GameObject.Find("Model" + index).GetComponent<StarterAssetsInputs>();
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            ShowName();
        }

    }
    public async void ShowName()
    {
        moralisUser = await Moralis.GetUserAsync();
        isLoggedIn = moralisUser != null;
        Debug.Log($"isLoggedIn = {isLoggedIn}");
        if (!isLoggedIn)
        {
            Debug.Log("Not Loggined!");
            return;
        }
        userAddress = moralisUser.ethAddress;
        textMesh.text = userAddress;
        //_background.SetActive(false);
        //LoadItems(userAddress, "0xe841128435D71364BeadD05E5d71CEF5016f0547", chainId);
        abilityBoard.SetActive(true);
        LoadItems(userAddress, _mintNftAddress, chainId);
    }
    
    public void StartGame()
    {
        _startAssetInputs.cursorLocked = true;
        _startAssetInputs.cursorInputForLook = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public async void LoadItems(string playerAddress, string contractAddress, ChainList contractChain)
    {
        //InitializeWeb3();
        try
        {
            NftOwnerCollection noc =
                await Moralis.GetClient().Web3Api.Account.GetNFTsForContract(playerAddress.ToLower(),
                    contractAddress, contractChain);

            List<NftOwner> nftOwners = noc.Result;
            
            // We only proceed if we find some
            if (!nftOwners.Any())
            {
                Debug.Log("You don't own items");
                return;
            }

            if (nftOwners.Count == _currentItemsCount)
            {
                Debug.Log("There are no new items to load");
                return;
            }

            // We clear the grid before adding new items
            ClearAllItems();
            int index = 0;
            // If we own one or more NFTs...
            foreach (var nftOwner in nftOwners)
            {


                if (nftOwner.Metadata == null)
                {
                    // Sometimes GetNFTsForContract fails to get NFT Metadata. We need to re-sync
                    await Moralis.GetClient().Web3Api.Token.ReSyncMetadata(nftOwner.TokenAddress, nftOwner.TokenId, contractChain);

                    Debug.Log("We couldn't get NFT Metadata. Re-syncing...");
                    continue;
                }

                // Check if tokenUri is null. If it's null it means it has probably been burned but it still appears
                if (nftOwner.TokenUri is null)
                {
                    Debug.Log("Token already burned");
                    return;
                }
                //Debug.Log("========1=========");
                // Deserialize metadata JSON to MetadataObject
                var metadata = nftOwner.Metadata;
                MetadataObject metadataObject = DeserializeUsingNewtonSoftJson(metadata);

                // We ONLY want objects with attributes. If metadataObject is null or metadataObject.attributes is null, we don't continue
                if (metadataObject?.attributes is null)
                {
                    Debug.Log("No attributes");
                    return;
                }
                Debug.Log(nftOwner.Metadata);
                // Populate new item
                PopulatePlayerItem(nftOwner.TokenId, metadataObject, index);
                index++;
                //Debug.Log("========3=========");
            }
        }
        catch (Exception exp)
        {
            Debug.LogError(exp.Message);
        }
        
    }

    public void DeleteCurrentSelectedItem()
    {
        Destroy(_currentSelectedItem.gameObject);
        _currentItemsCount--;
    }

    private void PopulatePlayerItem(string tokenId, MetadataObject metadataObject, int i)
    {
        InventoryItem newItem = Instantiate(itemPrefab, new Vector3(content.position.x + i * 230, content.position.y, content.position.z), Quaternion.identity, content);
        //new Vector3(content.position.x+i*150, content.position.y, content.position.z)
        
        newItem.Init(tokenId, metadataObject);

        _currentItemsCount++;
    }

    public void DeleteItem(string id)
    {
        foreach (Transform item in content)
        {
            InventoryItem itemClass = item.GetComponent<InventoryItem>(); // Assuming every item has InventoryItem script

            if (itemClass.myTokenId == id)
            {
                Destroy(item.gameObject);
                _currentItemsCount--;
            }
        }
    }

    private void ClearAllItems()
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }

        _currentItemsCount = 0;
    }

    [CanBeNull]
    private MetadataObject DeserializeUsingNewtonSoftJson(string json)
    {
        var metadataObject = JsonConvert.DeserializeObject<MetadataObject>(json);
        //Debug.Log("Error");
        return metadataObject;
    }



    public async void ClaimRewardAsync()
    {
        
        string ABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_recipient\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"values\",\"type\":\"uint256[]\"}],\"name\":\"TransferBatch\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"TransferSingle\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"value\",\"type\":\"string\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"URI\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"accounts\",\"type\":\"address[]\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"}],\"name\":\"balanceOfBatch\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"perks\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"perks\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"mintLimited\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"amounts\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeBatchTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"uri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
        string FunctioName = "mint";
        int _index = Random.Range(11, 14);
        object[] inputParams = { _index, userAddress };
        Nethereum.Hex.HexTypes.HexBigInteger value = new Nethereum.Hex.HexTypes.HexBigInteger("0x0");
        Nethereum.Hex.HexTypes.HexBigInteger gas = new Nethereum.Hex.HexTypes.HexBigInteger("800000");
        Nethereum.Hex.HexTypes.HexBigInteger gasprice = new Nethereum.Hex.HexTypes.HexBigInteger("230000");

        try
        {
            string result = await Moralis.ExecuteContractFunction(_mintNftAddress, ABI, FunctioName, inputParams, value, gas, gasprice);
            Debug.Log("Txhash :" + result);
        }
        catch (Exception error)
        {
            Debug.Log("Error :" + error);
        }
    }

}




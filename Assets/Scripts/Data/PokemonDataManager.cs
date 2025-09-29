using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokemonDefine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class PokemonDataManager : Singleton<PokemonDataManager>
{
    [Header("PokemonStats AssetBundls")]
    public string statsAssetBundlesName;

    public string statsFileName;

    [Header("SkillInfo AssetBundles")]
    public string skillInfoAssetBundlesName;

    public string skillInfoFileName;

    [Header("PokemonSkills AssetBundles")]
    public string pokemonSkillsAssetBundlesName;

    public string pokemonSkillsFileName;

    [Header("PokemonSprite AssetBundles")]
    public string pokemonSpritesAssetBundlesName;

    public string pokemonSpritesFileName;

    [Header("PokemonCrySound AssetBundles")]
    public string pokemonCrySoundAssetBundlesName;
    public string pokemonCrySoundFileName;

    [Header("PokemonSkillEffectSound AssetBundles")]
    public string pokemonSkillEffectSoundAssetBundlesName;

    [Header("PokemonEffectSound AssetBundles")]
    public string pokemonEffectSoundAssetBundlesName;


    private PokemonBaseStats[] pokemonBaseStatList;

    private Dictionary<string, PokemonSkillInfoData> pokemonSkillInfoDataList;

    private PokemonSkillData[] pokemonSkillDataList;

    private Dictionary<string, Skill> pokemonSkillClasses;

    private Dictionary<int, Sprite> pokemonSprites = new Dictionary<int, Sprite>();

    private Dictionary<int, AudioClip> pokemonCry = new Dictionary<int, AudioClip>();

    private Dictionary<string, AudioClip> pokemonSkillEffectSound = new Dictionary<string, AudioClip>();

    void Start()
    {
        Init();
    }

    private void Init()
    {
        LoadPokemonStatData();
        LoadSkillInfoData();
        LoadPokemonSkillData();
    }

    private void LoadPokemonStatData()
    {
        if (null != pokemonBaseStatList)
        {
            return;
        }

        TextAsset loadAsset = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(statsAssetBundlesName, statsFileName) as TextAsset;

        if (null == loadAsset)
        {
            return;
        }

        string loadStr = loadAsset.ToString();

        PokemonStatsList toJson = JsonUtility.FromJson<PokemonStatsList>(loadStr);

        if (null == toJson.pokemonData)
        {
            Debug.LogError("LoadPokemonStatData ToJson is Fail");
            return;
        }

        pokemonBaseStatList = toJson.pokemonData;

        return;
    }

    private void LoadSkillInfoData()
    {
        if (null != pokemonSkillInfoDataList)
        {
            return;
        }

        TextAsset loadAsset = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(skillInfoAssetBundlesName, skillInfoFileName) as TextAsset;

        if (null == loadAsset)
        {
            return;
        }

        string loadStr = loadAsset.ToString();

        PokemonSkillInfoDataList toJson = JsonUtility.FromJson<PokemonSkillInfoDataList>(loadStr);

        if (null == toJson.pokemonSkillInfoData)
        {
            Debug.LogError("LoadSkillInfoData ToJson is Fail");
            return;
        }

        PokemonSkillInfoData[] skillTempData = toJson.pokemonSkillInfoData;

        pokemonSkillInfoDataList = new Dictionary<string, PokemonSkillInfoData>();

        for (int i = 0; i < skillTempData.Length; i++)
        {
            pokemonSkillInfoDataList.Add(skillTempData[i].enName, skillTempData[i]);
        }

        return;
    }

    private void LoadPokemonSkillData()
    {
        if (null != pokemonSkillDataList)
        {
            return;
        }

        TextAsset loadAsset = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(pokemonSkillsAssetBundlesName, pokemonSkillsFileName) as TextAsset;

        if (null == loadAsset)
        {
            return;
        }

        string loadStr = loadAsset.ToString();

        PokemonSkillDataList toJson = JsonUtility.FromJson<PokemonSkillDataList>(loadStr);

        if (null == toJson.pokemonSkills)
        {
            Debug.LogError("LoadPokemonSkillData ToJson is Fail");
            return;
        }

        pokemonSkillDataList = toJson.pokemonSkills;

        return;
    }

    public PokemonBaseStats GetPokemonBaseStatData(int pokemonNumber)
    {
        if (null == pokemonBaseStatList)
        {
            LoadPokemonStatData();
        }

        return pokemonBaseStatList[pokemonNumber-1];
    }

    public PokemonSkillInfoData GetPokemonSkillData(string skillName)
    {
        if (null == pokemonSkillInfoDataList)
        {
            LoadSkillInfoData();
        }

        return pokemonSkillInfoDataList[skillName];
    }

    public PokemonType GetPokemonType(string typeName)
    {
        return (PokemonType)Enum.Parse(typeof(PokemonType),typeName);
    }

    public string[] GetPokemonSkillLearnData(int pokemonNumber)
    {
        if (null == pokemonSkillDataList)
        {
            LoadPokemonSkillData();
        }

        return pokemonSkillDataList[pokemonNumber - 1].skillList;
    }

    public Sprite GetPokemonSprite(int pokemonNumber)
    {
        // 먼저 Dictionary에 저장된것이 있는지 검사해본다.
        if (true == pokemonSprites.ContainsKey(pokemonNumber))
        {
            return pokemonSprites[pokemonNumber];
        }

        SpriteAtlas spriteAtlas = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(pokemonSpritesAssetBundlesName, pokemonSpritesFileName) as SpriteAtlas;

        if (null == spriteAtlas)
        {
            Debug.LogError("Pokemon.cs / atlas is not found");
            return null;
        }

        Sprite sprite = spriteAtlas.GetSprite(pokemonNumber.ToString("D3") + "_00");

        pokemonSprites.Add(pokemonNumber, sprite);

        return pokemonSprites[pokemonNumber];
    }

    public AudioClip GetPokemonCrySound(int pokemonNumber)
    {
        if (true == pokemonCry.ContainsKey(pokemonNumber))
        {
            return pokemonCry[pokemonNumber];
        }

        AudioClip cry = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(pokemonCrySoundAssetBundlesName, pokemonNumber.ToString("D3")) as AudioClip;

        if (null == cry)
        {
            Debug.LogError("PokemonDataManager.cs / GetPokemonCrySound / cry is null : " + pokemonNumber.ToString("D3"));
            return null;
        }

        pokemonCry.Add(pokemonNumber, cry);

        return pokemonCry[pokemonNumber];
    }

    public AudioClip GetPokemonSkillEffectSound(string name)
    {
        if (true == pokemonSkillEffectSound.ContainsKey(name))
        {
            return pokemonSkillEffectSound[name];
        }

        AudioClip skillEffectSound = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(pokemonSkillEffectSoundAssetBundlesName, name) as AudioClip;

        if (null == skillEffectSound)
        {
            Debug.LogError("PokemonDataManager.cs / GetPokemonSkillEffect / skillEffectSound is null");
            return null;
        }

        pokemonSkillEffectSound.Add(name, skillEffectSound);

        return pokemonSkillEffectSound[name];
    }

    public AudioClip GetPokemonEffectSound(string name)
    {
        if (true == pokemonSkillEffectSound.ContainsKey(name))
        {
            return pokemonSkillEffectSound[name];
        }

        AudioClip effectSound = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(pokemonEffectSoundAssetBundlesName, name) as AudioClip;

        if (null == effectSound)
        {
            Debug.LogError("PokemonDataManager.cs / GetPokemonEffectSound / effectSound is null");
            return null;
        }

        pokemonSkillEffectSound.Add(name, effectSound);

        return pokemonSkillEffectSound[name];
    }



}

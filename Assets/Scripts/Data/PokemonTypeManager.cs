using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokemonDefine;

public class PokemonTypeManager : Singleton<PokemonTypeManager>
{
    [SerializeField]
    public string assetBundlesName;

    [SerializeField]
    public string fileName;

    [SerializeField]
    public float[,] pokemonTypeData;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        TextAsset loadAsset = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(assetBundlesName, fileName) as TextAsset;

        if (null == loadAsset)
        {
            return;
        }

        string assetStr = loadAsset.ToString();

        // 줄바꿈 단위로 끊기
        string[] strList = assetStr.Split('\n');

        int arrHeight = strList.Length;

        if (arrHeight <= 0)
        {
            Debug.LogError("포켓몬 타입 데이터가 하나도 없음");
        }

        // 콤마(,) 단위로 끊기
        int arrWidth = strList[0].Split(',').Length;

        pokemonTypeData = new float[arrWidth, arrHeight];

        for (int i = 0; i < arrHeight; i++)
        {
            string[] dataArr = strList[i].Split(',');
            for (int j = 0; j < arrWidth; j++)
            {
                pokemonTypeData[i, j] = float.Parse(dataArr[j]);
            }
        }

        return;
    }

    // 공격 기술을 보고 받아야하는 데미지가 얼마나 되는지 상성 판단.
    // 공격 기술은 무조건 단일 타입이며.
    // 포켓몬의 타입은 하나 또는 둘이다.
    public TypeResult TypeCompare(PokemonType attackType, PokemonType defenceType)
    {
        TypeResult resultData = new TypeResult() { resultValue = 1.0f, status = PokemonStatus.NORMAL };
        ArrayList typeDefenceNumber = new ArrayList();
        int attackTypeNumber = 0;
        int checkType = 0;

        // 상성에 따른 데미지 배율 계산.
        {
            float result = 1.0f;
            int typeLength = System.Enum.GetValues(typeof(PokemonType)).Length;

            // 공격 기술의 타입을 알아보자
            for (int i = 1; i < typeLength; i++)
            {
                checkType = 1 << i;
                if (((int)attackType & checkType) != 0 ? true : false)
                {
                    // 공격 타입은 단일 타입이므로 들어가면 끝!
                    attackTypeNumber = i - 1;
                    break;
                }
            }

            // 공격 받는 타입을 알아보자..
            for (int i = 1; i < typeLength; i++)
            {
                checkType = 1 << i;

                if (((int)defenceType & checkType) != 0 ? true : false)
                {
                    // 해당 되는 타입은 들어가라..
                    typeDefenceNumber.Add(i - 1);
                }
            }

            // 찾은 정보를 통해서 상성표를 참조해 결과를 계산.
            for (int i = 0; i < typeDefenceNumber.Count; i++)
            {
                int defenceIndex = (int)typeDefenceNumber[i];
                result *= pokemonTypeData[attackTypeNumber, defenceIndex];
            }

            // 만일 0배가 나온 경우...
            if (0.0f == result)
            {
                // 원작은 0배라 아예 데미지가 무효이지만 여기서는 엄청 조금이라도 들어간다...
                result = 0.1f;
            }

            // 상성 결과를 집어 넣는다.
            resultData.resultValue = result;
        }

        // 공격기술 타입에 따른 상태이상 계산.
        {
            // 데미지가 없이 상태 이상 공격같은 경우는 아예 다른 함수를 이용 예정.(최면술, 하품, 기타 등등..)
            // 상태 이상에 따른 정확한 확률을 모르므로 확실한것은 30%의 마비 확률로 통일 시킨다.

            // 공격 기술의 타입과 포켓몬의 타입이 서로 같은 타입인 경우 상태이상에 걸리지 않는다.
            if ((attackType & defenceType) != 0 ? false : true)
            {
                float randomResult = Random.Range(0.0f, 100.0f);

                if (randomResult <= 30.0f)
                {
                    switch (attackType)
                    {
                        // 화상
                        case PokemonType.FIRE:
                            resultData.status = PokemonStatus.BURN;
                            break;
                        // 마비
                        case PokemonType.ELECTRIC:
                            resultData.status = PokemonStatus.PARALYSIS;
                            break;
                        // 얼음
                        case PokemonType.ICE:
                            resultData.status = PokemonStatus.FREEZE;
                            break;
                        // 중독
                        case PokemonType.POISON:
                            resultData.status = PokemonStatus.POISONING;
                            break;
                    }
                }
            }
        }
        return resultData;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using PokemonDefine;

public class CreateSkillClass
{
    // 스킬 클래스 관련 경로와 파일명
    public static string skillClassFileDirectory = "Assets/Scripts/Game/Skill/Skills";
    public static string skillClassTemplateFilePath = skillClassFileDirectory + "/Template.txt";
    public static string T = "##########";
    public static string templateString = "";

    // 스킬파일에대한 경로와 파일명
    public static string skillInfoFilePath = "Assets/Resources/Data/PokemonInfo/skillInfo_min.json";

    public static PokemonSkillInfoData[] skillInfo;

    [MenuItem("Assets/Create SkillClassFile")]
    static void CreateSkillClassFiles()
    {
        Debug.Log("= 스킬 클래스 파일 생성 시작 =");

        // 디렉토리가 존재하지 않는다면 생성한다.
        if (false == Directory.Exists(skillClassFileDirectory))
        {
            Debug.Log("디렉토리가 존재하지 않으므로 생성합니다.");
            Directory.CreateDirectory(skillClassFileDirectory);
            Debug.Log("디렉토리가 생성 완료.");
        }

        // 스킬 정보 데이터 로드
        if (true == File.Exists(skillInfoFilePath))
        {
            Debug.Log("스킬정보 데이터 파일 확인");

            Debug.Log("스킬 정보 데이터 파일 읽기 시작");
            string readSkillInfo = File.ReadAllText(skillInfoFilePath);
            Debug.Log("스킬 정보 데이터 파일 읽기 완료");

            Debug.Log("스킬 정보 데이터 Json 변환 시작");
            PokemonSkillInfoDataList loadedData = JsonUtility.FromJson<PokemonSkillInfoDataList>(readSkillInfo);
            if (null == loadedData.pokemonSkillInfoData)
            {
                Debug.LogError("Json 변환 실패");
                return;
            }

            skillInfo = loadedData.pokemonSkillInfoData;
            Debug.Log("스킬 정보 데이터 Json 변환 완료");
            Debug.Log("스킬 갯수 : " + skillInfo.Length);
        }
        else
        {
            Debug.LogError("스킬정보 데이터 파일이 존재하지 않습니다. 경로를 확인해 주세요");
            return;
        }

        // 템플릿 파일 로드
        if (true == File.Exists(skillClassTemplateFilePath))
        {
            Debug.Log("클래스 템플릿 파일 확인");

            Debug.Log("클래스 템플릿 파일 읽기 시작");
            templateString = File.ReadAllText(skillClassTemplateFilePath);

            if (0 == templateString.Length)
            {
                Debug.LogError("템플릿 파일 내용이 비어있습니다.");
                return;
            }
            Debug.Log("클래스 템플릿 파일 읽기 완료");

            Debug.Log("스킬 클래스 생성 시작!");

            for (int i = 0; i < skillInfo.Length; i++)
            {
                string temp = templateString;
                temp = temp.Replace(T, skillInfo[i].enName);


                string newSkillFilePath = skillClassFileDirectory + "/" + skillInfo[i].enName + ".cs";

                File.WriteAllText(newSkillFilePath, temp);
                Debug.Log(newSkillFilePath + " 생성 성공");
            }

            Debug.Log("모든 스킬 클래스 생성 완료");
        }
        else
        {
            Debug.LogError("클래스 템플릿 파일이 존재하지 않습니다. 경로를 확인해 주세요");
            return;
        }
    }
}

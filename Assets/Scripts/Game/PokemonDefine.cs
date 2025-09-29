using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PokemonDefine
{
    public enum PokemonType
    {
        NONE = 0    // 없음 (제2의 타입이 없거나 혹은 미싱노)
        , NORMAL = 1 << 1    // 노말
        , FIRE = 1 << 2    // 불꽃
        , WATER = 1 << 3   // 물
        , GRASS = 1 << 4    // 풀
        , ELECTRIC = 1 << 5    // 전기
        , ICE = 1 << 6    // 얼음
        , FIGHTING = 1 << 7    // 격투
        , POISON = 1 << 8    // 독
        , GROUND = 1 << 9    // 땅
        , FLYING = 1 << 10    // 비행
        , PSYCHIC = 1 << 11    // 에스퍼
        , BUG = 1 << 12    // 벌레
        , ROCK = 1 << 13    // 바위
        , GHOST = 1 << 14    // 고스트
        , DRAGON = 1 << 15    // 드래곤
        , DARK = 1 << 16    // 악
        , STEEL = 1 << 17    // 강철
        , FAIRY = 1 << 18    // 페어리
    }

    // 포켓몬 상태
    public enum PokemonStatus
    {
        NORMAL      // 평상시
        , FREEZE     // 얼음
        , PARALYSIS  // 마비
        , BURN       // 화상
        , SLEEP      // 잠듦
        , POISONING  // 중독
        , FAINT      // 기절(HP 모두 소진시)
    }

    [System.Serializable]
    public struct PokemonStatsList
    {
        public PokemonBaseStats[] pokemonData;
    }

    // 포켓몬 능력치
    [System.Serializable]
    public struct PokemonBaseStats
    {
        public int number;                     // 도감번호
        public float hp;                         // 체력
        public float attack;                     // 공격력
        
        public string type1;              // 포켓몬 타입1
        public string type2;              // 포켓몬 타입2

        public string skill1;
        public string skill2;

        public string name;
    }

    [System.Serializable]
    public struct PokemonSkillInfoDataList
    {
        public PokemonSkillInfoData[] pokemonSkillInfoData;
    }

    [System.Serializable]
    public struct PokemonSkillInfoData
    {
        public string enName;       // 영문 이름
        public string name;         // 한글 이름
        public string type;         // 스킬 타입
        public int attack;          // 공격력
        public bool buff;           // 버프스킬인지 유무
        public float delay;         // 대기시간
        public bool waitless;       // 빨리쏘기
        public bool whackWhack;     // 리피트
        public bool broadBurst;     // 넓히기
        public bool scatterShot;    // 늘리기
        public bool sharing;        // 결속
        public bool stayStrong;     // 오래가기
        public string skillInfo;    // 스킬설명
    }

    [System.Serializable]
    public struct PokemonSkillDataList
    {
        public PokemonSkillData[] pokemonSkills;
    }

    [System.Serializable]
    public struct PokemonSkillData
    {
        public int pokemonNumber;
        public string[] skillList;
    }

    [System.Serializable]
    public class TypeResult
    {
        public float resultValue;
        public PokemonStatus status;
    }

}


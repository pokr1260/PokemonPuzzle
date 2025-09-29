using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("보스 웨이브를 제외한 일반 포켓몬 웨이브")]
    [Range(1,5)]
    public int totalWave = 1;
    private int currentWave = 0;

    //public Pokemon[] enemyPokemons { get; private set; }

    private int[] pokemonList;
    public List<Pokemon> currentWavePokemons { get; private set; }


    private List<List<Pokemon>> totalEnemyPokemonWaves;

    private void Start()
    {
        currentWavePokemons = new List<Pokemon>();
        totalEnemyPokemonWaves = new List<List<Pokemon>>();
    }

    public void Init(int[] pokemonList, int bossPokemonNumber)
    {
        if (0 == pokemonList.Length)
        {
            Debug.LogError("Enemy.cs / pokemonList is null");
            return;
        }
        this.pokemonList = pokemonList;

        InitWave();
        InitBossWave(bossPokemonNumber);
    }

    /*
    
    Enemy ┬ 1 포켓몬1
          │   포켓몬2
          │
          ├ 2 포켓몬1
          │   포켓몬2
          │   포켓몬3
          │
          └ 3 포켓몬1

    이런식으로 Hierarchy가 되어있다.

     */

    private void InitWave()
    {
        GameObject gameObject = new GameObject();
        for (int i = 1; i <= totalWave; i++)            // 웨이브 GameObject
        {
            gameObject.name = i.ToString();
            Transform waveTransform = Instantiate(gameObject, transform.position, Quaternion.identity, this.transform).transform;

            int count = Random.Range(0, 3);
            List<Pokemon> wavePokemons = new List<Pokemon>();

            for (int j = 0; j <= count; j++)            // 웨이브 내의 포켓몬들
            {
                int getPokemon = Random.Range(0, pokemonList.Length);
                Pokemon pokemon = Instantiate<Pokemon>(GameManager.Instance.pokemonPrefab, new Vector3(), Quaternion.identity, waveTransform.transform);

                // 임시레벨 1
                pokemon.Init(pokemonList[getPokemon], false, 1, j, null);
                wavePokemons.Add(pokemon);

                // HUD
                GameObject hud = Instantiate(GameManager.Instance.pokemonHUDPrefab, new Vector3(), Quaternion.identity, GameManager.Instance.pokemonHUDRoot.transform);
                pokemon.hud = hud.GetComponent<HUD>();
                pokemon.hud.Init(pokemon.level, pokemon.hp, pokemon.hp, pokemon.pokemonName, pokemon.transform.localPosition);
            }

            totalEnemyPokemonWaves.Add(wavePokemons);

            // 첫 웨이브가 아닌 경우 hide
            if (1 != i)
            {
                waveTransform.gameObject.SetActive(false);
            }

        }
        Destroy(gameObject);

        NextWave();
    }

    private void InitBossWave(int bossPokemonNumber)
    { 
        
    }

    public Pokemon GetLiveEnemyPokemon()
    {
        for (int i = 0; i < currentWavePokemons.Count; i++)
        {
            if (true == currentWavePokemons[i].gameObject.activeInHierarchy)
            {
                return currentWavePokemons[i];
            }
        }
        return null;
    }


    public void NextWave()
    {
        Debug.Log("다음 웨이브");
        currentWave++;

        // 보스전 돌입!
        if (totalWave <= currentWave)
        {
            Debug.Log("보스전 웨이브 돌입!");
        }

        currentWavePokemons.Clear();

        currentWavePokemons = totalEnemyPokemonWaves[currentWave-1];

        if (null == currentWavePokemons && currentWavePokemons.Count == 0)
        {
            Debug.LogError("Enemy.cs NextWave Get enemyPokemonWaves Dequeue is Null");
            return;
        }

        // null이 아니고 최소 1개의 element가 있으므로 무조건 0번 인덱스에 값은 존재한다.
        // 그러므로 부모의 active를 true로 둔다.
        currentWavePokemons[0].transform.parent.gameObject.SetActive(true);
        GameManager.Instance.SetTarget(currentWavePokemons[0]);
    }


}

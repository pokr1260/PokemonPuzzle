using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const int MAXPOKEMON = 3;
    public int[] playerPokemonNumbers = new int[MAXPOKEMON];
    public Pokemon[] playerPokemons = new Pokemon[MAXPOKEMON];
    public GameObject[] pokemonSkillSlots = new GameObject[MAXPOKEMON];

    List<Pokemon> playerPokemonData = null;

    public int a = 10;

    public void Foo(int a)
    { 
    
    }

    public int[] GetPlayerPokemonNumbers()
    {
        return playerPokemonNumbers;
    }

    public void Init()
    {
        for (int i = 0; i < playerPokemonNumbers.Length; i++)
        {
            if (0 < playerPokemonNumbers[i] && playerPokemonNumbers[i] < 152)
            {
                //Pokemon pokemon = Instantiate<Pokemon>(pokemonPrefab, new Vector3(), Quaternion.identity, this.transform);
                Pokemon pokemon = Instantiate<Pokemon>(GameManager.Instance.pokemonPrefab, new Vector3(), Quaternion.identity, this.transform);

                // 임시로 레벨 1로둠...
                pokemon.Init(playerPokemonNumbers[i],true, 1 , i , pokemonSkillSlots[i].GetComponent<SkillSlot>());
                playerPokemons[i] = pokemon;

                // HUD
                GameObject hud = Instantiate(GameManager.Instance.pokemonHUDPrefab, new Vector3(), Quaternion.identity, GameManager.Instance.pokemonHUDRoot.transform);

                pokemon.hud = hud.GetComponent<HUD>();
                pokemon.hud.Init(pokemon.level, pokemon.hp, pokemon.hp, pokemon.pokemonName, pokemon.transform.localPosition);

                

                // 스킬슬롯 세팅
                pokemonSkillSlots[i].GetComponent<SkillSlot>().Init(i, playerPokemonNumbers[i], pokemon.GetPokemonSkillData());
            }
        }
    }

    public Pokemon[] GetPlayerPokemons()
    {
        return playerPokemons;
    }

    public Pokemon GetPlayerPokemon(int index)
    {
        return playerPokemons[index];
    }

}

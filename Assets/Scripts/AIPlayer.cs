using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>AIPlayer</c> a variation of <c>Player</c> that runs autonomously and makes intelligent game decisions.
/// </summary>
public class AIPlayer : Player
{
    public AIPlayer(string name, Color color) : base(name, color) { }

    /// <summary>
    /// <c>TakeTurn</c> allows the AI to play for a whole game turn (setup OR draft + attack + fortify).
    /// </summary>
    override public void TakeTurn()
    {
        if (GameController.Get().HandleObjectClick.GetMethodInfo().Name.Equals("SetupPhase")) // AI takes a setup claiming a single country
        {
            Wait.Start(1f, () => // wait 0 to 2 seconds to take a country so it looks like a real player
            {
                // IMPORTANT! the following line prevents resource wastage / crashing
                // do not randomly look for a country if not many are left unclaimed
                if (GameController.Get().populatedCountries >= GameController.Get().countryMap.Count - 5)
                {
                    foreach (var v in GameController.Get().countryMap)
                    {
                        if (v.Value.GetOwner() == null)
                        {
                            v.Value.SetOwner(this);
                            v.Value.ChangeTroops(1);
                            this.ChangeNumberOfTroops(-1);
                            GameController.Get().populatedCountries++;

                            GameObject.Find("SoundDraft").GetComponent<AudioSource>().Play();
                            Killfeed.Update($"{this.GetName()}: now owns {v.Value.GetName()}");

                            if (GameController.Get().populatedCountries == GameController.Get().countryMap.Count)
                            {
                                GameController.Get().currentPhase.text = "deploy phase";
                                GameController.Get().HandleObjectClick = GameController.Get().SetupDeployPhase;
                                GameController.Get().ResetTurn();
                                return;
                            }

                            GameController.Get().NextTurn();
                            return;
                        }
                    }
                }
                else
                {
                    while (true) // loops until finding an unowned country
                    {
                        var kvp = GameController.Get().countryMap.ElementAt(UnityEngine.Random.Range(0, GameController.Get().countryMap.Count));

                        if (kvp.Value.GetOwner() == null)
                        {
                            kvp.Value.SetOwner(this);
                            kvp.Value.ChangeTroops(1);
                            this.ChangeNumberOfTroops(-1);
                            GameController.Get().populatedCountries++;

                            GameObject.Find("SoundDraft").GetComponent<AudioSource>().Play();
                            Killfeed.Update($"{this.GetName()}: now owns {kvp.Value.GetName()}");

                            GameController.Get().NextTurn();
                            return;
                        }
                    }
                }
            });
        }
        else if (GameController.Get().HandleObjectClick.GetMethodInfo().Name.Equals("SetupDeployPhase")) // AI takes a setup deploy turn
        {
            Wait.Start(UnityEngine.Random.Range(2f, 3f), () => // wait 2 to 4 seconds to take a country so it looks like a real player
            {
                List<Country> ownedCountries = this.GetCountries();
                Country selected = ownedCountries.ElementAt(UnityEngine.Random.Range(0, ownedCountries.Count));

                int troops = this.GetNumberOfTroops();
                selected.ChangeTroops(troops);
                this.ChangeNumberOfTroops(-troops);

                GameObject.Find("SoundDraft").GetComponent<AudioSource>().Play();
                Killfeed.Update($"{this.GetName()}: sent {troops} troop(s) to {selected.GetName()}");

                GameController.Get().NextTurn();

                if (GameController.Get().turnPlayer.GetNumberOfTroops() == 0) // next player has no troops to deploy
                {
                    GameController.Get().currentPhase.text = "draft phase";
                    GameController.Get().HandleObjectClick = GameController.Get().DraftPhase;
                    GameController.Get().ResetTurn(); // AI agent is never first player, do not worry about this
                    GameController.Get().turnPlayer.GetNewTroopsAndCards();

                    GameObject.Find("EndPhase").GetComponent<Image>().enabled = true;
                    GameObject.Find("EndPhase").GetComponent<Button>().enabled = true;
                }

                return;
            });
        }
        else // AI player takes a normal turn
        {
            GameController.Get().currentPhase.text = "draft phase";
            GameController.Get().HandleObjectClick = GameController.Get().DraftPhase;
            Wait.Start(UnityEngine.Random.Range(1f, 3f), () => // wait 1 to 3 seconds to draft to a country so it looks like a real player
            {
                List<Country> ownedCountries = this.GetCountries();

                Country selected = ownedCountries.ElementAt(UnityEngine.Random.Range(0, ownedCountries.Count));
                int troops = this.GetNumberOfTroops();
                selected.ChangeTroops(troops);
                this.ChangeNumberOfTroops(-troops);

                GameObject.Find("SoundDraft").GetComponent<AudioSource>().Play();
                Killfeed.Update($"{this.GetName()}: sent {troops} troop(s) to {selected.GetName()}");

                GameController.Get().currentPhase.text = "attack phase";
                GameController.Get().HandleObjectClick = GameController.Get().AttackPhase;
            });

            Wait.Start(UnityEngine.Random.Range(6f, 9f), () => // wait to attack a country so it looks like a real player
            {
                for (int i = 0; i < this.GetNumberOfOwnedCountries(); i++)
                {
                    Country selectedCountry = this.GetCountries()[i];
                    if (selectedCountry.GetTroops() >= 2)
                    {
                        foreach (Country neighborCountry in selectedCountry.GetNeighbors())
                        {
                            if (neighborCountry.GetOwner() != this)
                            {
                                if (GameController.Get().Attack(selectedCountry, neighborCountry, Math.Min(selectedCountry.GetTroops() - 1, 3), Math.Min(2, neighborCountry.GetTroops())))
                                {
                                    Wait.Start(2f, () =>
                                    {
                                        if ((selectedCountry.GetTroops() > 2) && (neighborCountry.GetOwner() == this))
                                        {
                                            GameController.Get().Transfer(selectedCountry, neighborCountry, UnityEngine.Random.Range(1, selectedCountry.GetTroops()));
                                        }
                                    });
                                }
                                GameController.Get().currentPhase.text = "fortify phase";
                                GameController.Get().HandleObjectClick = GameController.Get().FortifyPhase;
                                return;
                            }
                        }
                    }
                }
            });

            Wait.Start(UnityEngine.Random.Range(13f, 14f), () => // wait to fortify a country so it looks like a real player
            {
                Country owned = this.GetCountries()[0];
                if (owned.GetTroops() > 2)
                {
                    List<Country> considered = new List<Country>();
                    List<Country> visited = new List<Country>();
                    Action<List<Country>, Country> recurse = null;
                    recurse = (visited, country) =>
                    {
                        visited.Add(country);

                        foreach (Country neighbor in country.GetNeighbors())
                        {
                            if (neighbor.GetOwner() != owned.GetOwner() || visited.Contains(neighbor)) continue;
                            recurse(visited, neighbor);
                        }
                    };

                    recurse(visited, owned);
                    considered = visited;

                    Country send = owned;
                    Country receive = null;
                    if (considered.Count != 0) receive = considered[UnityEngine.Random.Range(0, considered.Count)];
                    if ((send != receive) && (receive.GetOwner() == this)) GameController.Get().Transfer(send, receive, (owned.GetTroops() - 1) / 2);
                    return;
                }
            });

            Wait.Start(15f, () =>
            {
                GameController.Get().currentPhase.text = "draft phase";
                GameController.Get().HandleObjectClick = GameController.Get().DraftPhase;
                GameController.Get().NextTurn();
                GameController.Get().turnPlayer.GetNewTroopsAndCards();
                GameController.Get().turnPlayer.InitializeSlot();
            });
        }
    }
}

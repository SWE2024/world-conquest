using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AIPlayer : Player
{
    public AIPlayer(string name, Color color) : base(name, color) { }

    override public void TakeTurn()
    {
        if (GameController.Get().flagSetupPhase) // AI takes a setup claiming a single country
        {
            Wait.Start(Random.Range(1, 2), () => // wait 1 to 2 seconds to take a country so it looks like a real player
            {
                while (true)
                {
                    var kvp = GameController.Get().countryMap.ElementAt(Random.Range(0, GameController.Get().countryMap.Count - 1));

                    if (kvp.Value.GetOwner() == null)
                    {
                        kvp.Value.SetOwner(this);
                        kvp.Value.ChangeTroops(1);
                        this.ChangeNumberOfTroops(-1);
                        GameController.Get().populatedCountries++;

                        Killfeed.Update($"{this.GetName()}: now owns {kvp.Value.GetName()}");

                        if (GameController.Get().populatedCountries == GameController.Get().countryMap.Count)
                        {
                            GameController.Get().ResetTurn();
                            GameController.Get().currentPhase.text = "deploy phase";
                            GameController.Get().flagSetupPhase = false;
                            GameController.Get().flagSetupDeployPhase = true;
                            GameController.Get().HandleObjectClick = GameController.Get().SetupDeployPhase;
                            return;
                        }

                        GameController.Get().NextTurn();
                        return;
                    }
                }
            });
            return;
        }
        else if (GameController.Get().flagSetupDeployPhase) // AI takes a setup deploy turn
        {
            Wait.Start(Random.Range(2, 4), () => // wait 2 to 4 seconds to take a country so it looks like a real player
            {
                bool flagDone = false;
                List<Country> ownedCountries = this.GetCountries();

                while (!flagDone)
                {
                    Country selected = ownedCountries.ElementAt(Random.Range(0, ownedCountries.Count - 1));

                    if (selected.GetOwner() == this)
                    {
                        int troops = this.GetNumberOfTroops();
                        selected.ChangeTroops(troops);
                        this.ChangeNumberOfTroops(-troops);

                        Killfeed.Update($"{this.GetName()}: sent {troops} troops to {selected.GetName()}");

                        GameController.Get().NextTurn();

                        if (GameController.Get().turnPlayer.GetNumberOfTroops() == 0) // next player has not troops to deploy
                        {
                            GameController.Get().ResetTurn(); // AI agent is never first player, do not worry about this
                            GameController.Get().currentPhase.text = "attack phase";
                            GameController.Get().flagSetupDeployPhase = false;
                            GameController.Get().HandleObjectClick = GameController.Get().AttackPhase;

                            GameObject.Find("EndPhase").GetComponent<Image>().enabled = true;
                            GameObject.Find("EndPhase").GetComponent<Button>().enabled = true;
                        }

                        flagDone = true;
                    }
                }
                return;
            });
            return;
        }
        else // AI player takes a normal turn
        {
            GameController.Get().currentPhase.text = "draft phase";
            GameController.Get().HandleObjectClick = GameController.Get().DraftPhase;
            Wait.Start(Random.Range(2, 4), () => // wait 2 to 4 seconds to draft to a country so it looks like a real player
            {
                bool flagDone = false;
                List<Country> ownedCountries = this.GetCountries();

                while (!flagDone)
                {
                    Country selected = ownedCountries.ElementAt(Random.Range(0, ownedCountries.Count - 1));

                    if (selected.GetOwner() == this)
                    {
                        int troops = this.GetNumberOfTroops();
                        selected.ChangeTroops(troops);
                        this.ChangeNumberOfTroops(-troops);

                        Killfeed.Update($"{this.GetName()}: sent {troops} troops to {selected.GetName()}");

                        flagDone = true;
                    }
                }
                return;
            });

            GameController.Get().currentPhase.text = "attack phase";
            GameController.Get().HandleObjectClick = GameController.Get().AttackPhase;
            Wait.Start(Random.Range(6, 8), () => // wait 2 to 4 more seconds to attack countries so it looks like a real player
            {
                Killfeed.Update($"{this.GetName()}: attack not implemented");
                return;
            });

            GameController.Get().currentPhase.text = "fortify phase";
            GameController.Get().HandleObjectClick = GameController.Get().FortifyPhase;
            Wait.Start(Random.Range(9, 10), () => // wait 1 to 2 more seconds to fortify a country so it looks like a real player
            {
                Killfeed.Update($"{this.GetName()} fortify not implemented");
                return;
            });

            GameController.Get().currentPhase.text = "draft phase";
            GameController.Get().NextTurn();
        }
    }
}

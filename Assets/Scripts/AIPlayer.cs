using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AIPlayer : Player
{
    public AIPlayer(string name, Color color) : base(name, color) { }

    override public void TakeTurn()
    {
        if (GameController.Get().flagSetupPhase)
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

                        Killfeed.Update($"{kvp.Value.GetName()}: now owned by {this.GetName()}");

                        if (GameController.Get().populatedCountries < GameController.Get().countryMap.Count)
                        {
                            GameController.Get().NextTurn();
                        }
                        else
                        {
                            GameController.Get().ResetTurn();
                            GameController.Get().currentPhase.text = "deploy phase";
                            GameController.Get().HandleObjectClick = GameController.Get().SetupDeployPhase;
                            GameController.Get().flagSetupPhase = false;
                            GameController.Get().flagSetupDeployPhase = true;

                            GameObject.Find("EndPhase").GetComponent<Image>().enabled = true;
                            GameObject.Find("EndPhase").GetComponent<Button>().enabled = true;
                        }

                        return;
                    }
                }
            });
            return;
        }
        else if (GameController.Get().flagSetupDeployPhase)
        {
            Wait.Start(Random.Range(2, 5), () => // wait 2 to 5 seconds to take a country so it looks like a real player
            {
                while(GetNumberOfTroops() > 0)
                {
                    foreach (var kvp in GameController.Get().countryMap)
                    {
                        if (kvp.Value.GetOwner() == this)
                        {
                            int troopsToDistribute = Random.Range(1, GetNumberOfTroops());

                            kvp.Value.ChangeTroops(troopsToDistribute);
                            this.ChangeNumberOfTroops(-troopsToDistribute);

                            Killfeed.Update($"{this.GetName()}: transferred {troopsToDistribute} to {kvp.Value.GetName()}");

                            GameController.Get().NextTurn();
                            return;
                        }
                    }
                }
            });
            return;
        }
        else
        {
            GameController.Get().HandleObjectClick = GameController.Get().DraftPhase;
            Wait.Start(2f, () =>
            {
                Killfeed.Update("AI PLAYER TAKING DRAFT PHASE");
            });

            GameController.Get().HandleObjectClick = GameController.Get().AttackPhase;
            Wait.Start(2f, () =>
            {
                Killfeed.Update("AI PLAYER TAKING ATTACK PHASE");
            });

            GameController.Get().HandleObjectClick = GameController.Get().FortifyPhase;
            Wait.Start(2f, () =>
            {
                Killfeed.Update("AI PLAYER TAKING FORTIFY PHASE");
            });

            GameController.Get().NextTurn();
        }
    }
}

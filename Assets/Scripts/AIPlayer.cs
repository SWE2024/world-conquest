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
                bool flagDone = false;
                while (!flagDone)
                {
                    var kvp = GameController.Get().countryMap.ElementAt(Random.Range(0, GameController.Get().countryMap.Count - 1));

                    if (kvp.Value.GetOwner() == null)
                    {
                        kvp.Value.SetOwner(this);
                        kvp.Value.ChangeTroops(1);
                        this.ChangeNumberOfTroops(-1);
                        GameController.Get().populatedCountries++;

                        Killfeed.Update($"{this.GetName()}: now owns {kvp.Value.GetName()}");

                        if (GameController.Get().populatedCountries < GameController.Get().countryMap.Count)
                        {
                            GameController.Get().NextTurn();
                        }
                        else
                        {
                            GameController.Get().ResetTurn();
                            GameController.Get().currentPhase.text = "deploy phase";
                            GameController.Get().flagSetupPhase = false;
                            GameController.Get().flagSetupDeployPhase = true;
                            GameController.Get().HandleObjectClick = GameController.Get().SetupDeployPhase;

                            GameObject.Find("EndPhase").GetComponent<Image>().enabled = true;
                            GameObject.Find("EndPhase").GetComponent<Button>().enabled = true;

                            flagDone = true;
                        }
                        return;
                    }
                }
            });
            return;
        }
        else if (GameController.Get().flagSetupDeployPhase)
        {
            Wait.Start(Random.Range(1, 2), () => // wait 1 to 2 seconds to take a country so it looks like a real player
            {
                bool flagDone = false;
                while (!flagDone)
                {
                    var kvp = GameController.Get().countryMap.ElementAt(Random.Range(0, GameController.Get().countryMap.Count - 1));

                    if (kvp.Value.GetOwner() == this)
                    {
                        kvp.Value.ChangeTroops(this.GetNumberOfTroops());
                        this.ChangeNumberOfTroops(-this.GetNumberOfTroops());

                        Killfeed.Update($"{this.GetName()}: transferred {this.GetNumberOfTroops()} to {kvp.Value.GetName()}");

                        GameController.Get().NextTurn();
                        flagDone = true;
                        return;
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

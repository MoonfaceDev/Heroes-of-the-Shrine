using System;
using System.Linq;

[Serializable]
public class PlayerAttackExecutor
{
    /// <summary>
    /// Pairing between attack and required button
    /// </summary>
    [Serializable]
    public class AttackProperty
    {
        public BaseAttack attack;
        public Button button;
        public float energyCost;
    }

    public EnergySystem energySystem;

    /// <value>
    /// List of attacks that can be played using the controller
    /// </value>
    public AttackProperty[] attacks;

    private AttackProperty GetNextAttack(BaseAttack.Command command, Button[] downButtons)
    {
        return (
            from property in attacks
            where downButtons.Any(button => button == property.button) && property.attack.CanPlay(command) &&
                  energySystem.Energy >= property.energyCost
            select property
        ).LastOrDefault();
    }

    public bool Play(Button[] downButtons)
    {
        var command = new BaseAttack.Command();
        var nextAttack = GetNextAttack(command, downButtons);

        if (nextAttack == null)
        {
            return false;
        }

        energySystem.TakeEnergy(nextAttack.energyCost);
        nextAttack.attack.Play(command);
        return true;
    }
}
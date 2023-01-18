using UnityEngine;

public class PlayerDistanceBrainModule : BrainModule
{
    private GameEntity playerEntity;
    private static readonly int PlayerDistance = Animator.StringToHash(PlayerDistanceParameter);
    private static readonly int PlayerDistanceX = Animator.StringToHash(PlayerDistanceXParameter);
    private static readonly int PlayerDistanceY = Animator.StringToHash(PlayerDistanceYParameter);
    private static readonly int PlayerDistanceZ = Animator.StringToHash(PlayerDistanceZParameter);

    private const string PlayerDistanceParameter = "playerDistance";
    private const string PlayerDistanceXParameter = "playerDistanceX";
    private const string PlayerDistanceYParameter = "playerDistanceY";
    private const string PlayerDistanceZParameter = "playerDistanceZ";

    public override void Awake()
    {
        base.Awake();
        playerEntity = EntityManager.Instance.GetEntity(Tag.Player);
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.SetFloat(PlayerDistance, MovableEntity.GroundDistance(playerEntity.WorldPosition));
        StateMachine.SetFloat(PlayerDistanceX, Mathf.Abs((MovableEntity.WorldPosition - playerEntity.WorldPosition).x));
        StateMachine.SetFloat(PlayerDistanceY, Mathf.Abs((MovableEntity.WorldPosition - playerEntity.WorldPosition).y));
        StateMachine.SetFloat(PlayerDistanceZ, Mathf.Abs((MovableEntity.WorldPosition - playerEntity.WorldPosition).z));
    }

    public override string[] GetParameters()
    {
        return new[]
            { PlayerDistanceParameter, PlayerDistanceXParameter, PlayerDistanceYParameter, PlayerDistanceZParameter };
    }
}
using UnityEngine;

public class EnemyState
{
    public StateMachine stateMachine;
    public Enemy5 advancedEnemy;

    public EnemyState(Enemy5 enemy, StateMachine machine)
    {
        advancedEnemy = enemy;
        stateMachine = machine;
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void FrameUpdate() { }
}

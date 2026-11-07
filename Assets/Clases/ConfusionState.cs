using UnityEngine;

public class ConfusionState : EnemyState
{
    private float confusionTimer;

    public ConfusionState(Enemy5 enemy, StateMachine machine)
        : base(enemy, machine)
    {
    }

    public override void EnterState()
    {
        confusionTimer = 0f;

        advancedEnemy.agent.isStopped = true;
        advancedEnemy.agent.ResetPath();

        Debug.Log("Enemigo confundido.");
    }

    public override void FrameUpdate()
    {
        if (advancedEnemy.DetectarJugador())
        {
            advancedEnemy.CambiarEstado(
                advancedEnemy.chaseState
            );

            return;
        }

        confusionTimer += Time.deltaTime;

        if (confusionTimer >= advancedEnemy.confusionTime)
        {
            Debug.Log("El enemigo vuelve a patrullar.");

            advancedEnemy.CambiarEstado(
                advancedEnemy.idleState
            );
        }
    }

    public override void ExitState()
    {
        advancedEnemy.agent.isStopped = false;
    }
}

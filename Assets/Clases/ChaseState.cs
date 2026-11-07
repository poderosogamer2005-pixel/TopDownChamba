using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(Enemy5 enemy, StateMachine machine)
       : base(enemy, machine)
    {
    }

    public override void EnterState()
    {
        advancedEnemy.agent.isStopped = false;

        Debug.Log("Enemigo entra en persecución.");
    }

    public override void FrameUpdate()
    {
        // Si detecta al jugador, actualiza su última posición.
        if (advancedEnemy.DetectarJugador())
        {
            float distancia = Vector3.Distance(
                advancedEnemy.transform.position,
                advancedEnemy.player.transform.position
            );

            // Si está dentro del rango, comienza a atacar.
            if (distancia <= advancedEnemy.attackRadius)
            {
                advancedEnemy.CambiarEstado(
                    advancedEnemy.attackState
                );

                return;
            }
        }

        // Persigue la última posición conocida.
        advancedEnemy.agent.SetDestination(
            advancedEnemy.lastKnownPosition
        );

        // Si llega y no encuentra al jugador, se confunde.
        if (!advancedEnemy.agent.pathPending &&
            advancedEnemy.agent.remainingDistance <=
            advancedEnemy.agent.stoppingDistance + 0.2f)
        {
            if (!advancedEnemy.DetectarJugador())
            {
                advancedEnemy.CambiarEstado(
                    advancedEnemy.confusionState
                );
            }
        }
    }
}

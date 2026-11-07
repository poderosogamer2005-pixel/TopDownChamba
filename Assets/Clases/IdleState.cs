using UnityEngine;
using UnityEngine.AI;

public class IdleState : EnemyState
{
    private Vector3 targetPosition;
    private float patrolTimer;

    public IdleState(Enemy5 enemy, StateMachine machine)
        : base(enemy, machine)
    {
    }

    public override void EnterState()
    {
        patrolTimer = 0f;

        advancedEnemy.agent.isStopped = false;

        GetRandomPosition();
    }

    public override void FrameUpdate()
    {
        if (advancedEnemy.DetectarJugador())
        {
            advancedEnemy.CambiarEstado(advancedEnemy.chaseState);
            return;
        }

        patrolTimer += Time.deltaTime;

        if (patrolTimer >= advancedEnemy.patrolInterval)
        {
            patrolTimer = 0f;
            GetRandomPosition();
        }

        if (!advancedEnemy.agent.pathPending &&
            advancedEnemy.agent.remainingDistance <=
            advancedEnemy.agent.stoppingDistance + 0.1f)
        {
            GetRandomPosition();
        }
    }

    private void GetRandomPosition()
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 randomPosition =
                advancedEnemy.transform.position +
                Random.insideUnitSphere *
                advancedEnemy.randomMovementRange;

            randomPosition.y = advancedEnemy.transform.position.y;

            if (NavMesh.SamplePosition(
                randomPosition,
                out NavMeshHit hit,
                advancedEnemy.randomMovementRange,
                NavMesh.AllAreas))
            {
                targetPosition = hit.position;

                advancedEnemy.agent.SetDestination(targetPosition);
                return;
            }
        }
    }
}

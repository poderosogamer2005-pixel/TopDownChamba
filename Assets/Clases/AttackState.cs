using UnityEngine;

public class AttackState : EnemyState
{
    private float attacksTimer;

    public AttackState(Enemy5 enemy, StateMachine machine)
        : base(enemy, machine)
    {
    }

    public override void EnterState()
    {
        attacksTimer = 0f;

        advancedEnemy.agent.isStopped = true;
        advancedEnemy.agent.ResetPath();

        Debug.Log("Enemigo entra en estado de ataque.");
    }

    public override void FrameUpdate()
    {
        if (advancedEnemy.player == null)
            return;

        // Comprueba si todavía puede detectar al jugador.
        bool jugadorDetectado = advancedEnemy.DetectarJugador();

        if (!jugadorDetectado)
        {
            advancedEnemy.CambiarEstado(
                advancedEnemy.chaseState
            );

            return;
        }

        Vector3 direccion =
            advancedEnemy.player.transform.position -
            advancedEnemy.transform.position;

        direccion.y = 0f;

        float distancia = direccion.magnitude;

        // Si el jugador sale del rango, vuelve a perseguirlo.
        if (distancia > advancedEnemy.attackRadius)
        {
            advancedEnemy.CambiarEstado(
                advancedEnemy.chaseState
            );

            return;
        }

        // Girar hacia el jugador.
        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacion =
                Quaternion.LookRotation(direccion);

            advancedEnemy.transform.rotation =
                Quaternion.Slerp(
                    advancedEnemy.transform.rotation,
                    rotacion,
                    Time.deltaTime * 8f
                );
        }

        // Temporizador de ataques.
        attacksTimer += Time.deltaTime;

        if (attacksTimer >= advancedEnemy.attackInterval)
        {
            attacksTimer = 0f;

            AtacarJugador();
        }
    }

    private void AtacarJugador()
    {
        Vector3 origen =
            advancedEnemy.transform.position + Vector3.up;

        Vector3 objetivo =
            advancedEnemy.player.transform.position + Vector3.up;

        Vector3 direccion = (objetivo - origen).normalized;

        Debug.DrawRay(
            origen,
            direccion * advancedEnemy.attackRadius,
            Color.blue,
            1f
        );

        RaycastHit hit;

        if (Physics.Raycast(
            origen,
            direccion,
            out hit,
            advancedEnemy.attackRadius,
            advancedEnemy.playerMask,
            QueryTriggerInteraction.Ignore))
        {
            PlayerController5 jugador =
                hit.collider.GetComponentInParent<PlayerController5>();

            if (jugador != null)
            {
                jugador.RecibirDanio(
                    advancedEnemy.playerDamage
                );

                Debug.Log("¡El enemigo golpeó al jugador!");
            }
        }
    }

    public override void ExitState()
    {
        advancedEnemy.agent.isStopped = false;

        attacksTimer = 0f;

        Debug.Log("El enemigo salió del estado de ataque.");
    }
}

// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using System.Collections;
using RG_GameCamera.Input;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Extras
{
    public class EnemyAI : CharacterController.HitEntity
    {
        public float AttackDistance = 2.0f;
        public float AttackTimeout = 1.0f;
        public bool Aggresive;

        protected NavMeshAgent agent;
        protected float attackTimer;
        protected CharacterController.HitEntity player;
        protected InputFilter velocityFilter;
        protected float targetSpeed;

        protected static int corpseCounter;
        protected float deadTimeout;
        protected float agentSpeed;

        protected enum AnimationState
        {
            Idle,
            Walk,
            Run,
            Attack,
            Dead,
        }

        protected AnimationState animState;

        protected override void Start()
        {
            base.Start();
            agent = GetComponent<NavMeshAgent>();
            animState = AnimationState.Idle;
            player = EntityManager.Instance.Player;
            velocityFilter = new InputFilter(10, 1.0f);
        }

        protected virtual void UpdateAnimState()
        {
        }

        protected override void Update()
        {
            if (IsDead)
            {
                deadTimeout += Time.deltaTime;

                if (corpseCounter > 10 && deadTimeout > 5.0f)
                {
                    Destroy(gameObject);
                    corpseCounter--;
                }

                return;
            }

            if (!player || player.IsDead)
            {
                animState = AnimationState.Idle;
            }

            UpdateAnimState();

            if (!player)
            {
                return;
            }

            var distance2 = (player.transform.position - transform.position).sqrMagnitude;

            if (distance2 < AttackDistance * AttackDistance)
            {
                animState = AnimationState.Attack;
                agent.Stop();
                attackTimer -= Time.deltaTime;

                // hit player
                if (attackTimer < 0.0f)
                {
                    player.OnHit(this, 10, transform.position + transform.forward + Vector3.up * 0.5f);
                    attackTimer = AttackTimeout;
                }
            }
            else
            {
                var targetPos = player.transform.position - (player.transform.position - transform.position).normalized;
                targetPos = player.transform.position;
                agent.SetDestination(targetPos);

                if (agent.hasPath)
                {
                    if (Aggresive)
                    {
                        animState = AnimationState.Run;
                    }
                    else
                    {
                        animState = AnimationState.Walk;
                    }

                    //
                    // move the enemy
                    //
                    var v0 = (-(transform.position - agent.steeringTarget)).normalized;
                    var moveVector = SmoothVelocityVector(v0);
                    transform.position += moveVector * agentSpeed * Time.deltaTime;

                    v0.y = 0.0f;

                    if (v0 != Vector3.zero)
                    {
                        transform.forward = v0;
                    }
                }
                else
                {
                    animState = AnimationState.Idle;
                }
            }
        }

        protected Vector3 SmoothVelocityVector(Vector3 v)
        {
            velocityFilter.AddSample(new Vector2(v.x, v.z));
            var filtered = velocityFilter.GetValue();

            return new Vector3(filtered.x, 0.0f, filtered.y).normalized;
        }

        protected override void OnDie()
        {
            base.OnDie();
            animState = AnimationState.Dead;
            UpdateAnimState();
            GetComponent<Collider>().enabled = false;
            agent.enabled = false;
            corpseCounter++;
            deadTimeout = 0.0f;
        }

        public override void OnHit(CharacterController.HitEntity owner, float damage, Vector3 hitPos)
        {
            base.OnHit(owner, damage, hitPos);
            Aggresive = true;
        }

        protected override void FixedUpdate()
        {
            if (animState == AnimationState.Attack)
            {
                // always look at player
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(player.transform.position -
                                                                              transform.position), Time.deltaTime * 10);
            }
        }
    }
}
 
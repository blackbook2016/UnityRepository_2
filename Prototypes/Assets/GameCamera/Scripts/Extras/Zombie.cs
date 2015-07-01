// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Extras
{
    public class Zombie : EnemyAI
    {
        private Animator animator;
        private bool injured;

        protected override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();
            animator.applyRootMotion = false;
            injured = Random.value > 0.5f;
        }

        protected override void UpdateAnimState()
        {
            var newSpeed = targetSpeed;

            switch (animState)
            {
                case AnimationState.Idle:
                    newSpeed = 0.0f;
                    agent.Stop();
                    break;

                case AnimationState.Walk:
                    newSpeed = 0.5f;
                    break;

                case AnimationState.Run:
                    newSpeed = 1.0f;
                    break;

                case AnimationState.Attack:
                    newSpeed = 0.5f;
                    break;

                case AnimationState.Dead:
                    targetSpeed = 0.0f;
                    agent.Stop();
                    break;
            }

            targetSpeed = Mathf.Lerp(targetSpeed, newSpeed, Time.deltaTime*5);

            var animSpeed = targetSpeed;
            agentSpeed = 0.0f;

            if (targetSpeed <= 0.5f)
            {
                agentSpeed = targetSpeed/0.5f;
            }
            else
            {
                const float maxSpeed = 4.0f;
                agentSpeed = (targetSpeed - 0.5f)/0.5f*(maxSpeed - 0.5f) + 0.5f;
            }

            animator.SetFloat("Speed", animSpeed);
            animator.SetBool("Injured", injured);
        }

        protected override void OnDie()
        {
            base.OnDie();

            if (Demo.ZombieHUD.Instance)
            {
                Demo.ZombieHUD.Instance.ZombieKilled();
            }

            animator.SetBool("Die", true);

            corpseCounter++;
            deadTimeout = 0.0f;
        }
    }
}

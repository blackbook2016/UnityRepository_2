// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Extras
{
    public class CaveWorm : EnemyAI
    {
        public AnimationClip ClipIdle;
        public AnimationClip ClipMove;
        public AnimationClip ClipAttack;
        public AnimationClip ClipDead;

        protected override void UpdateAnimState()
        {
            var newSpeed = targetSpeed;

            switch (animState)
            {
                case AnimationState.Idle:
                    GetComponent<Animation>().clip = ClipIdle;
                    GetComponent<Animation>().Play();
                    agent.Stop();
                    newSpeed = 0.0f;
                    break;

                case AnimationState.Walk:
                    GetComponent<Animation>().clip = ClipMove;
                    GetComponent<Animation>().Play();
                    newSpeed = 2.0f;
                    break;

                case AnimationState.Attack:
                    GetComponent<Animation>().clip = ClipAttack;
                    GetComponent<Animation>().Play();
                    newSpeed = 0.5f;
                    break;

                case AnimationState.Dead:
                    GetComponent<Animation>().clip = ClipDead;
                    GetComponent<Animation>().wrapMode = WrapMode.Once;
                    GetComponent<Animation>().Play();
                    agent.Stop();
                    newSpeed = 0.5f;
                    break;
            }

            targetSpeed = Mathf.Lerp(targetSpeed, newSpeed, Time.deltaTime * 5);

            agentSpeed = 0.0f;

            if (targetSpeed <= 0.5f)
            {
                agentSpeed = targetSpeed / 0.5f;
            }
            else
            {
                const float maxSpeed = 4.0f;
                agentSpeed = (targetSpeed - 0.5f) / 0.5f * (maxSpeed - 0.5f) + 0.5f;
            }

            agentSpeed = targetSpeed;
        }

        protected override void OnDie()
        {
            base.OnDie();

            if (Demo.ZombieHUD.Instance)
            {
                Demo.ZombieHUD.Instance.WormKilled();
            }
        }
    }
}

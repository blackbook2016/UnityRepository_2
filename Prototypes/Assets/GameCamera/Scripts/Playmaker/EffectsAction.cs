// uncomment next line to work with Playmaker
//#define PLAYMAKER
#if PLAYMAKER

// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
    [Tooltip("Run game camera effect")]
    public class GameCameraEffect : FsmStateAction
    {
        [Tooltip("Game camera effect")]
        public RG_GameCamera.Effects.Type Effect;

        public override void OnEnter()
        {
            var em = RG_GameCamera.Effects.EffectManager.Instance;

            if (em != null)
            {
                var effect = em.Create(Effect);

                if (effect)
                {
                    effect.Play();
                }
            }

            Finish();
        }
    }
}

#endif

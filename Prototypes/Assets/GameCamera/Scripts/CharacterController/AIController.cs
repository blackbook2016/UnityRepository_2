// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

//
// define one of these:
//

// no pathfinding, character will go to mouse position directly without any path check
//#define NO_PATHFINDING

// using Unity navmesh pathfinding
#define USING_NAVMESH

// using A*Pathfinding (tested with version 3.5.2)
//#define USING_ASTARPATHFINDING

#if NO_PATHFINDING
    #undef USING_NAVMESH
    #undef USING_ASTARPATHFINDING
#elif USING_NAVMESH
    #undef NO_PATHFINDING
    #undef USING_ASTARPATHFINDING
#elif USING_ASTARPATHFINDING
    #undef NO_PATHFINDING
    #undef USING_NAVMESH
#endif

using RG_GameCamera.Extras;
using RG_GameCamera.Input;
using RG_GameCamera.Modes;
using UnityEngine;

#if USING_ASTARPATHFINDING
using Pathfinding;
#endif

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// simple ai controller for controlling animated character
    /// this class is used for demonstration of RTS and RPG camera
    /// features:
    /// * moving to position
    /// * aim at target
    /// * shoot at target
    /// </summary>
#if USING_NAVMESH
    [RequireComponent(typeof (NavMeshAgent))]
#elif USING_ASTARPATHFINDING
    [RequireComponent(typeof(Seeker))]
#endif

    [RequireComponent(typeof (AnimationController))]
    public class AIController : MonoBehaviour
    {
        public float AttackDistance = 5.0f;
        public float TargetChangeTolerance = 1;
        public float Speed = 100;
        public float TargetLookOffset = 1.4f;

#if USING_NAVMESH
        protected NavMeshAgent agent;
#elif USING_ASTARPATHFINDING
        protected Seeker seeker;
        protected Path path;
        protected float nextWaypointDistance = 1;
        protected int currentWaypoint = 0;
#endif

        protected AnimationController animController;
        private Vector3 targetPos;
        private InputManager inputManager;
        private Human human;
        private InputFilter velocityFilter;
        private HitEntity enemyTarget;
        private bool attackEnemy;
        private bool stickMove;
        private Modes.Type cameraMode;

        /// <summary>
        /// activate this component and navmesh agent
        /// </summary>
        public void Activate(bool status)
        {
            enabled = status;
            targetPos = transform.position;

#if USING_NAVMESH
            agent.enabled = status;
#endif
        }

        private void Awake()
        {
#if USING_NAVMESH
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = true;
#elif USING_ASTARPATHFINDING
            seeker = GetComponent<Seeker>();
#else
            var agent = GetComponent<NavMeshAgent>();
            if (agent)
            {
                Destroy(agent);
//                agent.enabled = false;
            }
#endif
            animController = GetComponent<AnimationController>();
            inputManager = RG_GameCamera.Input.InputManager.Instance;
            human = GetComponent<Human>();
            velocityFilter = new InputFilter(10, 1.0f);
        }

        private void UpdateInput()
        {
            //
            // in case of remote player we can ignore direct input
            //
            if (human.Remote)
            {
                return;
            }

            if (!gameObject.GetComponent<HitEntity>().IsDead)
            {
                // update waypoint
                var waypoint = inputManager.GetInput(InputType.WaypointPos);

                if (waypoint.Valid)
                {
                    enemyTarget = EntityManager.Instance.Find((Vector3) waypoint.Value, 2.0f, "Player");
                    SetTarget((Vector3) waypoint.Value);
                    stickMove = false;
                }
                else
                {
                    var move = inputManager.GetInput(InputType.Move);
                    var rpg = inputManager.InputPreset == InputPreset.RPG;

                    if (move.Valid && rpg)
                    {
                        var moveVec = (Vector2) move.Value;
                        var cam = CameraManager.Instance.UnityCamera.transform;
                        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                        Vector3 moveVector = moveVec.y * camForward + moveVec.x * cam.right;
                        stickMove = true;
                        SetTarget(gameObject.transform.position + moveVector*10);
                    }
                }
            }
        }

        private void UpdateEnemyTarget()
        {
            attackEnemy = false;

            if (enemyTarget)
            {
                if (RTSProjector.Instance)
                {
                    RTSProjector.Instance.Project(enemyTarget.transform.position, Color.red);
                }

                if (enemyTarget.IsDead)
                {
                    enemyTarget = null;
                    attackEnemy = false;
#if USING_NAVMESH
                    agent.ResetPath();
#endif
                }
                else
                {
                    // check the distance from enemy
                    var enemyDist2 = (enemyTarget.transform.position - transform.position).sqrMagnitude;

                    // we are close enough for attack
                    if (enemyDist2 < AttackDistance*AttackDistance)
                    {
                        attackEnemy = true;
                    }
                }
            }
        }

        private void Update()
        {
            UpdateInput();

            UpdateEnemyTarget();

            // update the agents positon
            //agent.transform.position = transform.position;

            var moveVector = Vector3.zero;
            var aimMoveVector = Vector3.zero;
            var lookPos = transform.position + transform.forward + Vector3.up * TargetLookOffset;
            var aim = false;

#if USING_NAVMESH
            if (agent.hasPath)
            {
                if (attackEnemy)
                {
                    aim = true;
                    human.ShootAt(enemyTarget);
                    agent.Stop();
                    moveVector = SmoothVelocityVector((lookPos - transform.position).normalized);
                    lookPos = enemyTarget.transform.position + Vector3.up*1.6f;
                }
                else if ((transform.position - agent.destination).sqrMagnitude > 1.0f)
                {
                    moveVector = SmoothVelocityVector((agent.steeringTarget - transform.position).normalized) * 1.0f;
                    aimMoveVector.y = 1.0f;
                    lookPos = transform.position + moveVector + Vector3.up*TargetLookOffset;
                }
			}
#elif USING_ASTARPATHFINDING
            if (path != null)
            {
                if (attackEnemy)
                {
                    lookPos = enemyTarget.transform.position;
                    aim = true;
                    human.ShootAt(enemyTarget);
                    currentWaypoint = int.MaxValue;
                    path.Reset();
                }
                else if (currentWaypoint < path.vectorPath.Count)
                {
                    //Direction to the next waypoint
                    Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
//                    dir *= Speed * Time.fixedDeltaTime;
                    moveVector = SmoothVelocityVector(dir) * 1.0f;
                    lookPos = transform.position + transform.forward * Speed;

                    //Check if we are close enough to the next waypoint
                    //If we are, proceed to follow the next waypoint
                    if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
                    {
                        currentWaypoint++;
                    }
                }
            }
#else
            if (attackEnemy)
            {
                lookPos = enemyTarget.transform.position;
                aim = true;
                human.ShootAt(enemyTarget);
            }
            else if ((transform.position - targetPos).sqrMagnitude > 1.0f)
            {
                moveVector = SmoothVelocityVector(-(transform.position - targetPos)) * 1.0f;
                aimMoveVector.y = 1.0f;
                lookPos = transform.position + transform.forward * Speed;
            }
#endif

            human.Aim(aim);

            //UnityEngine.Debug.DrawRay(transform.position, (lookPos - transform.position), Color.red);

            var reset = inputManager.GetInput(InputType.Reset, false);
            if (reset)
            {
                CameraManager.Instance.SetMode(cameraMode);
                gameObject.GetComponent<HitEntity>().Resurect();
            }
            var isDeath = gameObject.GetComponent<HitEntity>().IsDead;
            if (isDeath)
            {
                if (CameraManager.Instance.GetCameraMode().Type != Type.Dead)
                {
                    cameraMode = CameraManager.Instance.GetCameraMode().Type;
                }
                CameraManager.Instance.SetMode(Modes.Type.Dead);
            }

            // use the values to move the character
            animController.Move(new AnimationController.Input
            {
                camMove = moveVector,
                inputMove = aimMoveVector,
                smoothAimRotation = true,
                aim = aim,
                crouch = false,
                jump = false,
                lookPos = lookPos,
                die =  isDeath,
                reset = reset,
            });
        }

        public void SetTarget(Vector3 target)
        {
            if ((target - targetPos).magnitude > TargetChangeTolerance)
            {
                targetPos = target;

#if USING_NAVMESH
                agent.SetDestination(targetPos);
#elif USING_ASTARPATHFINDING
                seeker.StartPath(transform.position, targetPos, OnPathComplete);
#endif

                if (!enemyTarget && !stickMove)
                {
                    if (RTSProjector.Instance)
                    {
                        RTSProjector.Instance.Project(target, Color.white);
                    }
                }
            }
        }

        private Vector3 SmoothVelocityVector(Vector3 v)
        {
            velocityFilter.AddSample(new Vector2(v.x, v.z));
            var filtered = velocityFilter.GetValue();

            return new Vector3(filtered.x, 0.0f, filtered.y).normalized;
        }

#if USING_ASTARPATHFINDING
        public void OnPathComplete(Path p)
        {
            Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }
#endif
    }
}

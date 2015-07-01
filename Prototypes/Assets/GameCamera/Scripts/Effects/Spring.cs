// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

namespace RG_GameCamera.Effects
{
    /// <summary>
    /// 1-dimensional spring simulation
    /// </summary>
    public class Spring
    {
        private float mass;
        private float distance;
        private float springConstant;
        private float damping;
        private float acceleration;
        private float velocity;
        private float springForce;

        /// <summary>
        /// setup new spring simulation
        /// </summary>
        /// <param name="mass">mass of the object on spring</param>
        /// <param name="distance">distance from zero (length of spring)</param>
        /// <param name="springStrength">strength of spring</param>
        /// <param name="damping">damping factor</param>
        public void Setup(float mass, float distance, float springStrength, float damping)
        {
            this.mass = mass;
            this.distance = distance;
            this.springConstant = springStrength;
            this.damping = damping;

            velocity = 0.0f;
        }

        /// <summary>
        /// add a simple force
        /// </summary>
        public void AddForce(float force)
        {
            velocity += force;
        }

        /// <summary>
        /// update spring with time step (this should be called every frame)
        /// </summary>
        /// <returns>distance of spring from zero</returns>
        public float Calculate(float timeStep)
        {
            springForce = -springConstant * distance - velocity * damping;
            acceleration = springForce / mass;
            velocity += acceleration * timeStep;
            distance += velocity * timeStep;

            return distance;
        }
    }
}

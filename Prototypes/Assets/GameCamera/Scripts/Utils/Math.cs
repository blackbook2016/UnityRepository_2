// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Utils
{
    public static class Math
    {
        /// <summary>
        /// y = e^(n * ln(x)) ... y = x^n 
        /// </summary>
	    public static float ExpN(float x, float n)
	    {
		    return Mathf.Exp(n * Mathf.Log(x));
	    }

        /// <summary>
        /// normalize value 'a' within range <min,max> to range <0, 1>
        /// </summary>
	    public static float NormalizeValueInRange(float a, float min, float max)
	    {
		    if (max <= min)
		    {
			    return 1.0f;
		    }
		    else
		    {
			    return (Mathf.Clamp(a, min, max) - min) / (max - min);
		    }
	    }

	    /// <summary>
	    /// return value 'a' within range <min, max>
	    /// </summary>
	    public static float DeNormalizeValueInRange(float a, float min, float max)
	    {
		    return Mathf.Clamp(a, 0.0f, 1.0f) * (max - min) + min;
	    }

        /// <summary>
        /// return true if value 'a' is in range <min,max>
        /// </summary>
	    public static bool IsInRange(float a, float min, float max)
	    {
		    return (a >= min && a <= max);
	    }

        /// <summary>
        /// clamp angle value to range <-PI, PI>
        /// </summary>
	    public static void ToPIRange(ref float angle)
	    {
		    if (angle < -Mathf.PI) angle += 2*Mathf.PI;
		    if (angle >  Mathf.PI) angle -= 2*Mathf.PI;
	    }

        /// <summary>
        /// return square value of x
        /// </summary>
	    public static float Sqr(float x)
	    {
		    return x*x;
	    }

        /// <summary>
        /// convert from cartesian coordinate system to spherical
        /// </summary>
	    public static void ToSpherical(Vector3 dir, out float rotX, out float rotZ)
	    {
		    var xyLen = Mathf.Sqrt(Sqr(dir.x) + Sqr(dir.z));
		    rotX = Mathf.Atan2(dir.x, dir.z); // yaw
		    rotZ = Mathf.Atan2(dir.y, xyLen); // pitch
	    }

        /// <summary>
        /// convert from spherical system to cartesian
        /// </summary>
	    public static void ToCartesian(float rotX,  float rotZ, out Vector3 dir)
	    {
		    var sinZ = Mathf.Sin(rotZ);
		    var cosZ = Mathf.Cos(rotZ);
		    var sinX = Mathf.Sin(rotX);
		    var cosX = Mathf.Cos(rotX);

		    dir.x = sinX * cosZ;
		    dir.y = sinZ;
            dir.z = cosX*cosZ;
	    }

        /// <summary>
        /// convergent value to target value in time
        /// </summary>
	    public static float ConvergeToValue(float target, float val, float timeRel, float speedPerSec)
	    {
		    if (val>target)
            {
			    val -= timeRel * speedPerSec;
			    if (val<target)
                {
				    val = target;
			    }
		    }
		    else
		    {
			    val += timeRel * speedPerSec;
			    if (val>target)
                {
				    val = target;
			    }
		    }
		    return val;
	    }

        /// <summary>
        /// swap two elements
        /// </summary>
        public static void Swap<T>(ref T a, ref T b)
	    {
		    T tmp = a;
		    a = b;
		    b = tmp;
	    }

        /// <summary>
        /// return normalized vector with zeroed y componnent
        /// </summary>
        public static Vector3 VectorXZ(Vector3 v)
        {
            var xz = v;
            xz.y = 0.0f;
            return xz.normalized;
        }

        /// <summary>
        /// change quaternion rotation to have correct up vector
        /// </summary>
        /// <param name="rot"></param>
        public static void CorrectRotationUp(ref Quaternion rot)
        {
            var forward = rot*Vector3.forward;
            rot = Quaternion.LookRotation(forward, Vector3.up);
        }

        //public static void SlerpDamp(Quaternion curr, Quaternion target, ref )

        /// <summary>
        /// smooth damp for single float
        /// </summary>
        public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = Mathf.Max(0.0001f, smoothTime);
            float num = 2f / smoothTime;
            float num2 = num * deltaTime;
            float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            float num4 = current - target;
            float num5 = target;
            float num6 = maxSpeed * smoothTime;
            num4 = Mathf.Clamp(num4, -num6, num6);
            target = current - num4;
            float num7 = (currentVelocity + num * num4) * deltaTime;
            currentVelocity = (currentVelocity - num * num7) * num3;
            float num8 = target + (num4 + num7) * num3;
            if (num5 - current > 0f == num8 > num5)
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }

        /// <summary>
        /// returns array of corner points of the near clipping plane
        /// </summary>
        /// <returns></returns>
        public static Vector3[] GetNearPlanePoints(Camera camera)
        {
            //  left, right, bottom, top, near, far
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);

            var array = new Vector3[4];
            array[0] = Intersection3Planes(planes[1], planes[2], planes[4]); // bottom-right
            array[1] = Intersection3Planes(planes[1], planes[3], planes[4]); // right-top
            array[2] = Intersection3Planes(planes[0], planes[3], planes[4]); // left-top
            array[3] = Intersection3Planes(planes[0], planes[2], planes[4]); // bottom-left
            return array;
        }

        /// <summary>
        /// Intersection of 3 planes, Graphics Gems 1 pg 305
        /// http://stackoverflow.com/questions/6408670/intersection-between-two-planes
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3 Intersection3Planes(Plane p0, Plane p1, Plane p2)
        {
            var det = p0.normal[0]*p1.normal[1]*p2.normal[2] - p0.normal[0]*p1.normal[2]*p2.normal[1] + 
                      p1.normal[0]*p2.normal[1]*p0.normal[2] - p1.normal[0]*p0.normal[1]*p2.normal[2] + 
                      p2.normal[0]*p0.normal[1]*p1.normal[2] - p2.normal[0]*p1.normal[1]*p0.normal[2];

            Debug.Assert(Mathf.Abs(det) > Mathf.Epsilon);

            return (Vector3.Cross(p1.normal, p2.normal)*-p0.distance +
                    Vector3.Cross(p2.normal, p0.normal)*-p1.distance +
                    Vector3.Cross(p0.normal, p1.normal)*-p2.distance)/det;
        }
    }
}

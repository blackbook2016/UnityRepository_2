// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Input
{
    /// <summary>
    /// weighted average filter
    /// </summary>
	public class PositionFilter
	{
		Vector3 value;
	    readonly Vector3[] samples;
	    readonly float weightCoef;
	    readonly int numSamples;

        public PositionFilter(int samplesNum, float coef)
		{
			value = new Vector3();
			weightCoef = coef;
			numSamples = samplesNum;
			samples = new Vector3[samplesNum];
		}

        public void AddSample(Vector3 sample)
		{
			var wxSum = new Vector3();
        	float wSum  = 0.0f;

        	float w0         = 1.0f;
        	float currWeight = 1.0f;

        	var n1 = samples[0];
        	samples[0] = sample;

        	for (int i=1; i<numSamples; i++)
        	{
          		wSum += currWeight;
          		wxSum += samples[i-1] * currWeight;

          		var n2 = samples[i];
          		samples[i] = n1;
          		n1 = n2;

          		currWeight = w0 * weightCoef;
          		w0 = currWeight;
        	}

        	value = wxSum / wSum;
		}

        public Vector3 GetValue()
		{
			return value;
		}

		public Vector3[] GetSamples()
		{
			return samples;
		}

		public void Reset(Vector3 resetVal)
		{
			for (int i=0; i<numSamples; i++)
			{
				samples[i] = resetVal;
			}
		}
	}
}

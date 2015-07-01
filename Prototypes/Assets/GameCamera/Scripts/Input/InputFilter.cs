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
	public class InputFilter
	{
		Vector2 value;
	    readonly Vector2[] samples;
	    readonly float weightCoef;
	    readonly int numSamples;

        public InputFilter(int samplesNum, float coef)
		{
			value = new Vector2();
			weightCoef = coef;
			numSamples = samplesNum;
			samples = new Vector2[samplesNum];
		}

        public void AddSample(Vector2 sample)
		{
			Vector2 wxSum = new Vector2();
        	float wSum  = 0.0f;

        	float w0         = 1.0f;
        	float currWeight = 1.0f;

        	Vector2 n1 = samples[0];
        	samples[0] = sample;

        	for (int i=1; i<numSamples; i++)
        	{
          		wSum += currWeight;
          		wxSum += samples[i-1] * currWeight;

          		Vector2 n2 = samples[i];
          		samples[i] = n1;
          		n1 = n2;

          		currWeight = w0 * weightCoef;
          		w0 = currWeight;
        	}

        	value = wxSum / wSum;
		}

        public Vector2 GetValue()
		{
			return value;
		}

		public Vector2[] GetSamples()
		{
			return samples;
		}

		public void Reset(Vector2 resetVal)
		{
			for (int i=0; i<numSamples; i++)
			{
				samples[i] = resetVal;
			}
		}
	}
}

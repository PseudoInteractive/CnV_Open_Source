using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG
{
	public readonly struct KeyFrame<T>
	{
		public readonly float t;
		//	public readonly bool easeIn;
		//	public readonly bool easeOut;
		public readonly T v;
		public KeyFrame(float _t, T _v)
		{
			t = _t;
			v = _v;
		}
	}


	public static class AKeyFrame
	{

		// Todo: make this generic
		public static float Eval(this KeyFrame<float> [] me, float t) 
		{
			int count = me.Length;
			for(int i=1;i<count;++i)
			{
				ref KeyFrame<float> k0 =ref me[i-1];
				ref KeyFrame<float> k1 = ref me[i];
				if (t <= k1.t)
				{
					return ((t-k0.t)/(k1.t-k0.t)).Lerp(k0.v, k1.v);
				}
			}
			return me[count - 1].v;
		}
	}
}

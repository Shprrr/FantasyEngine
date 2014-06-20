using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasyEngineData.Battles
{
	public class Cursor
	{
		public int Index { get; set; }
		public int IndexSelf { get; set; }
		public eTargetType Target { get; set; }

		public Cursor(eTargetType defaultTarget, int indexSelf)
		{
			Index = 0;
			IndexSelf = indexSelf;
			Target = defaultTarget;
		}
	}
}

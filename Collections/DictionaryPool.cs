#region Header
//   Vorspire    _,-'/-'/  DictionaryPool.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2016  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System.Collections.Generic;
#endregion

namespace VitaNex.Collections
{
	public sealed class DictionaryPool<TKey, TVal> : ObjectPool<Dictionary<TKey, TVal>>
	{
		public DictionaryPool()
		{ }

		public DictionaryPool(int capacity)
			: base(capacity)
		{ }

		public override void Free(Dictionary<TKey, TVal> o)
		{
			if (o != null)
			{
				o.Clear();
			}

			base.Free(o);
		}
	}
}
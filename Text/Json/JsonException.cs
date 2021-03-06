#region Header
//   Vorspire    _,-'/-'/  JsonException.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2016  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Runtime.Serialization;
#endregion

namespace VitaNex.Text
{
	public class JsonException : Exception
	{
		public JsonException()
		{ }

		public JsonException(string message)
			: base(message)
		{ }

		public JsonException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		protected JsonException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
using System.Collections.Generic;

namespace Car.Com.Common.Combinatorics
{
	internal interface IMetaCollection<T> : IEnumerable<IList<T>>
	{
		long Count { get; }
		GenerateOption Type { get; }
		int UpperIndex { get; }
		int LowerIndex { get; }
	}
}
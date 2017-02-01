using System;
using System.Collections;
using System.Collections.Generic;

namespace Car.Com.Common.Combinatorics
{
    public class Permutations<T> : IMetaCollection<T>
    {
        #region Constructors
        protected Permutations() { }

        public Permutations(IList<T> values)
        {
            Initialize(values, GenerateOption.WithoutRepetition, null);
        }

        public Permutations(IList<T> values, GenerateOption type)
        {
            Initialize(values, type, null);
        }

        public Permutations(IList<T> values, IComparer<T> comparer)
        {
            Initialize(values, GenerateOption.WithoutRepetition, comparer);
        }

        #endregion

        #region IEnumerable Interface

        public virtual IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<IList<T>> IEnumerable<IList<T>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Enumerator Inner-Class

        public sealed class Enumerator : IEnumerator<IList<T>>
        {
            #region Constructors

            public Enumerator(Permutations<T> source)
            {
                _myParent = source;
                _myLexicographicalOrders = new int[source._myLexicographicOrders.Length];
                source._myLexicographicOrders.CopyTo(_myLexicographicalOrders, 0);
                Reset();
            }

            #endregion

            #region IEnumerator Interface

            public void Reset()
            {
                _myPosition = Position.BeforeFirst;
            }

            public bool MoveNext()
            {
                switch (_myPosition)
                {
                    case Position.BeforeFirst:
                        {
                            _myValues = new List<T>(_myParent._myValues.Count);
                            _myValues.AddRange(_myParent._myValues);
                            Array.Sort(_myLexicographicalOrders);
                            _myPosition = Position.InSet;
                            break;
                        }

                    case Position.InSet:
                        {
                            if (_myValues.Count < 2)
                                _myPosition = Position.AfterLast;
                            else if (NextPermutation() == false)
                                _myPosition = Position.AfterLast;
                            break;
                        }
                }

                return _myPosition != Position.AfterLast;
            }

            public object Current
            {
                get
                {
                    if (_myPosition == Position.InSet)
                        return new List<T>(_myValues);

                    throw new InvalidOperationException();
                }
            }

            IList<T> IEnumerator<IList<T>>.Current
            {
                get
                {
                    if (_myPosition == Position.InSet)
                        return new List<T>(_myValues);

                    throw new InvalidOperationException();
                }
            }

            public void Dispose() { }

            #endregion

            #region Heavy Lifting Methods

            private bool NextPermutation()
            {
                var i = _myLexicographicalOrders.Length - 1;
                while (_myLexicographicalOrders[i - 1] >= _myLexicographicalOrders[i])
                {
                    --i;

                    if (i == 0)
                        return false;
                }

                var j = _myLexicographicalOrders.Length;
                while (_myLexicographicalOrders[j - 1] <= _myLexicographicalOrders[i - 1])
                {
                    --j;
                }

                Swap(i - 1, j - 1);
                ++i;
                j = _myLexicographicalOrders.Length;

                while (i < j)
                {
                    Swap(i - 1, j - 1);
                    ++i;
                    --j;
                }

                return true;
            }

            private void Swap(int i, int j)
            {
                _myTemp = _myValues[i];
                _myValues[i] = _myValues[j];
                _myValues[j] = _myTemp;
                _myKviTemp = _myLexicographicalOrders[i];
                _myLexicographicalOrders[i] = _myLexicographicalOrders[j];
                _myLexicographicalOrders[j] = _myKviTemp;
            }

            #endregion

            #region Data and Internal Members

            private T _myTemp;
            private int _myKviTemp;
            private Position _myPosition = Position.BeforeFirst;
            private readonly int[] _myLexicographicalOrders;
            private List<T> _myValues;
            private readonly Permutations<T> _myParent;

            private enum Position
            {
                BeforeFirst,
                InSet,
                AfterLast
            }

            #endregion
        }

        #endregion

        #region IMetaList Interface

        public long Count { get; private set; }

        public GenerateOption Type
        {
            get { return _myMetaCollectionType; }
        }

        public int UpperIndex
        {
            get { return _myValues.Count; }
        }

        public int LowerIndex
        {
            get { return _myValues.Count; }
        }

        #endregion

        #region Heavy Lifting Members

        private void Initialize(IList<T> values, GenerateOption type, IComparer<T> comparer)
        {
            _myMetaCollectionType = type;
            _myValues = new List<T>(values.Count);
            _myValues.AddRange(values);
            _myLexicographicOrders = new int[values.Count];

            switch (type)
            {
                case GenerateOption.WithRepetition:
                    {
                        for (var i = 0; i < _myLexicographicOrders.Length; ++i)
                            _myLexicographicOrders[i] = i;

                        break;
                    }

                default:
                    {
                        if (comparer == null)
                            comparer = new SelfComparer<T>();

                        _myValues.Sort(comparer);
                        var j = 1;

                        if (_myLexicographicOrders.Length > 0)
                            _myLexicographicOrders[0] = j;

                        for (var i = 1; i < _myLexicographicOrders.Length; ++i)
                        {
                            if (comparer.Compare(_myValues[i - 1], _myValues[i]) != 0)
                                ++j;

                            _myLexicographicOrders[i] = j;
                        }
                        break;
                    }

            }

            Count = GetCount();
        }

        private long GetCount()
        {
            var runCount = 1;
            var divisors = new List<int>();
            var numerators = new List<int>();

            for (var i = 1; i < _myLexicographicOrders.Length; ++i)
            {
                numerators.AddRange(SmallPrimeUtility.Factor(i + 1));

                if (_myLexicographicOrders[i] == _myLexicographicOrders[i - 1])
                    ++runCount;
                else
                {
                    for (var f = 2; f <= runCount; ++f)
                        divisors.AddRange(SmallPrimeUtility.Factor(f));

                    runCount = 1;
                }
            }

            for (var f = 2; f <= runCount; ++f)
                divisors.AddRange(SmallPrimeUtility.Factor(f));

            return SmallPrimeUtility.EvaluatePrimeFactors(SmallPrimeUtility.DividePrimeFactors(numerators, divisors));
        }

        #endregion

        #region Data and Internal Members

        private List<T> _myValues;
        private int[] _myLexicographicOrders;
        private class SelfComparer<TU> : IComparer<TU>
        {
            public int Compare(TU x, TU y)
            {
                return ((IComparable<TU>)x).CompareTo(y);
            }
        }
        private GenerateOption _myMetaCollectionType;

        #endregion
    }
}
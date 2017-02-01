using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Car.Com.Common.Combinatorics
{
    public class Combinations<T> : IMetaCollection<T>
    {
        //Ref: http://www.codeproject.com/KB/recipes/Combinatorics.aspx
        #region Constructors

        protected Combinations() { }

        public Combinations(IList<T> values, int lowerIndex)
        {
            Initialize(values, lowerIndex, GenerateOption.WithoutRepetition);
        }

        public Combinations(IList<T> values, int lowerIndex, GenerateOption type)
        {
            Initialize(values, lowerIndex, type);
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<IList<T>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region Enumerator Inner Class

        public sealed class Enumerator : IEnumerator<IList<T>>
        {
            #region Constructors

            public Enumerator(Combinations<T> source)
            {
                _myParent = source;
                _myPermutationsEnumerator = (Permutations<bool>.Enumerator)_myParent._myPermutations.GetEnumerator();
            }

            #endregion

            #region IEnumerator interface

            public void Reset()
            {
                _myPermutationsEnumerator.Reset();
            }

            public bool MoveNext()
            {
                _myCurrentList = null;
                return _myPermutationsEnumerator.MoveNext();
            }

            public IList<T> Current
            {
                get
                {
                    ComputeCurrent();
                    return _myCurrentList;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    ComputeCurrent();
                    return _myCurrentList;
                }
            }

            public void Dispose() { }

            #endregion

            #region Heavy Lifting Members

            private void ComputeCurrent()
            {
                if (_myCurrentList != null) return;

                _myCurrentList = new List<T>();
                var index = 0;
                var currentPermutation = (IList<bool>)_myPermutationsEnumerator.Current;

                foreach (var t in currentPermutation)
                {
                    if (t == false)
                    {
                        _myCurrentList.Add(_myParent._myValues[index]);

                        if (_myParent.Type == GenerateOption.WithoutRepetition)
                            ++index;
                    }
                    else
                        ++index;
                }
            }

            #endregion

            #region Data

            private readonly Combinations<T> _myParent;
            private List<T> _myCurrentList;
            private readonly Permutations<bool>.Enumerator _myPermutationsEnumerator;

            #endregion
        }

        #endregion

        #region IMetaList Interface

        public long Count
        {
            get { return _myPermutations.Count; }
        }

        public GenerateOption Type
        {
            get { return _myMetaCollectionType; }
        }

        public int UpperIndex
        {
            get { return _myValues.Count; }
        }

        public int LowerIndex { get; private set; }

        #endregion

        #region Heavy Lifting Members

        private void Initialize(ICollection<T> values, int lowerIndex, GenerateOption type)
        {
            _myMetaCollectionType = type;
            LowerIndex = lowerIndex;
            _myValues = new List<T>();
            _myValues.AddRange(values);
            var myMap = new List<bool>();

            switch (type)
            {
                case GenerateOption.WithoutRepetition:
                    {
                        myMap.AddRange(_myValues.Select((t, i) => i < _myValues.Count - LowerIndex));
                        break;
                    }

                default:
                    {
                        for (var i = 0; i < values.Count - 1; ++i)
                            myMap.Add(true);

                        for (var i = 0; i < LowerIndex; ++i)
                            myMap.Add(false);

                        break;
                    }
            }

            _myPermutations = new Permutations<bool>(myMap);
        }

        #endregion

        #region Data

        private List<T> _myValues;
        private Permutations<bool> _myPermutations;
        private GenerateOption _myMetaCollectionType;

        #endregion
    }
}
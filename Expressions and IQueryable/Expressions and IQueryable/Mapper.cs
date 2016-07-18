using System;

namespace ExpressionsAndIQueryable
{
    public sealed class Mapper<TSource, TDestination>
    {
        private readonly Func<TSource, TDestination> m_MapFunction;

        internal Mapper(Func<TSource, TDestination> func)
        {
            m_MapFunction = func;
        }

        public TDestination Map(TSource source)
        {
            return m_MapFunction(source);
        }
    }
}

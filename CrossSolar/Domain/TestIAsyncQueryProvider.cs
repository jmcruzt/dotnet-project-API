using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CrossSolar.Domain
{
    public class TestIAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestIAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestIAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        //public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        //{
        //    return Task.FromResult(Execute(expression));
        //}

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }


        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TestIAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }
    }

    public class TestIAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestIAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestIAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestIAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator() 
        {
            return GetAsyncEnumerator();
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestIAsyncQueryProvider<T>(this); }
        }


    }

    public class TestIAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestIAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

        public T Current
        {
            get { return _inner.Current; }
        }

        T IAsyncEnumerator<T>.Current  
        {
            get { return Current; }
        }
    }
}

namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class ODataQueryable<T> : IODataQueryable<T>
    {
        /// <summary>The values.</summary>
        private readonly IQueryable<T> values;

        /// <summary>Initializes a new instance of the &lt;see cref="IODataQueryable&lt;T&gt;" /&gt; interface.</summary>
        /// <param name="values">The values.</param>
        /// <param name="total">The total number of items.</param>
        public ODataQueryable(IQueryable<T> values, int total)
        {
            this.values = values;
            this.Total = total;
        }

        /// <summary>Gets the total number of items.</summary>
        /// <value>The total number of items.</value>
        public int Total { get; }

        /// <summary>Gets or sets the link to the next item set.</summary>
        /// <value>The link to the next item set.</value>
        public Uri NextLink { get; set; }

        /// <summary>Converts this object to a list asynchronously.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The list.</returns>
        public Task<List<T>> ToListAsync()
        {
            // TODO: Find a way to support entity frameworks ToListAsync()
            var tcs = new TaskCompletionSource<List<T>>();

            Task.Run(
                () =>
                    {
                        tcs.SetResult(this.values.ToList());
                    }).ConfigureAwait(false);

            return tcs.Task;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </returns>
        public Expression Expression => this.values.Expression;

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        /// </returns>
        public Type ElementType => this.values.ElementType;

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
        /// </returns>
        public IQueryProvider Provider => this.values.Provider;
    }
}
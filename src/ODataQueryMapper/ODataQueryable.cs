namespace Microsoft.AspNet.OData.Extensions.ODataQueryMapper
{
    using Microsoft.AspNet.OData.Extensions.ODataQueryMapper.Interfaces;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class ODataQueryable<T> : IODataQueryable<T>
    {
        /// <summary>The values.</summary>
        private IQueryable<T> values;

        /// <summary>Initializes a new instance of the &lt;see cref="ODataQueryable&lt;T&gt;" /&gt; class.</summary>
        public ODataQueryable()
        {
            this.values = Enumerable.Empty<T>().AsQueryable();

            this.Count = 0;
        }

        /// <summary>Initializes a new instance of the &lt;see cref="IODataQueryable&lt;T&gt;" /&gt; interface.</summary>
        /// <param name="values">The values.</param>
        /// <param name="total">The total number of items.</param>
        public ODataQueryable(IQueryable<T> values, int total)
        {
            this.values = values;

            this.Count = total;
        }

        /// <summary>Gets the total number of items.</summary>
        /// <value>The total number of items.</value>
        public int Count { get; set; }

        /// <summary>Gets or sets the link to the next item set.</summary>
        /// <value>The link to the next item set.</value>
        public string NextLink { get; set; }

        /// <summary>Initializes this object.</summary>
        /// <param name="collection">The collection.</param>
        /// <param name="count">The total number of items.</param>
        /// <param name="nextLink">The link to the next item set.</param>
        public void Initialize(IEnumerable collection, int count, string nextLink = null)
        {
            this.Count = count;
            this.NextLink = nextLink;

            if (collection == null)
            {
                this.Value = Enumerable.Empty<T>();
            }
            else if (collection is JArray)
            {
                this.Value = ((JArray)collection).Select(x => x.ToObject<T>());
            }
            else
            {
                this.Value = collection.Cast<T>().ToList();
            }
        }

        /// <summary>Gets the data.</summary>
        /// <returns>The data.</returns>
        public IEnumerable GetValue()
        {
            return this.Value;
        }

        /// <summary>Gets or sets the data.</summary>
        /// <value>The data.</value>
        public IEnumerable<T> Value
        {
            get
            {
                return this.values;
            }

            set
            {
                this.values = value?.AsQueryable();
            }
        }

        /// <summary>Converts this object to a list asynchronously.</summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>The list.</returns>
        public async Task<IODataQueryable<T>> ToListAsync()
        {
            // TODO: Find a better way to support entity frameworks ToListAsync()
            var tcs = new TaskCompletionSource<IQueryable<T>>();

            await Task.Run(
                () =>
                    {
                        tcs.SetResult(this.values.ToList().AsQueryable());
                    }).ConfigureAwait(false);

            var r = new ODataQueryable<T>(await tcs.Task, this.Count);
            r.NextLink = this.NextLink;

            return r;
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
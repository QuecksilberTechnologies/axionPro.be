using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.Queries
{
    public class GenericFilterQuery<T> : IRequest<List<T>> where T : class
    {
        public Expression<Func<T, bool>> Filter { get; }

        public GenericFilterQuery(Expression<Func<T, bool>> filter)
        {
            Filter = filter;
        }
    }

}

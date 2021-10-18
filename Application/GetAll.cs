using Domain.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application
{
    public class GetAll
    {
        public class Query : IRequest<List<Client>> { }
        public class Handler : IRequestHandler<Query, List<Client>>
        {
            readonly DataContext _context;
            public Handler(DataContext context) => _context = context;
            public async Task<List<Client>> Handle(Query request, CancellationToken cancellationToken) => await _context.Clients.ToListAsync(cancellationToken);

        }
    }
}

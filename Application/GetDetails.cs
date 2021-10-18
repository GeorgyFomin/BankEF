using Domain.Model;
using MediatR;
using Persistence.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application
{
    public class GetDetails
    {
        public class Query : IRequest<Client>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Client>
        {
            private readonly DataContext _context;
            public Handler(DataContext context) => _context = context;
            public async Task<Client> Handle(Query request, CancellationToken cancellationToken) => await _context.Clients.FindAsync(request.Id, cancellationToken);

        }

    }
}

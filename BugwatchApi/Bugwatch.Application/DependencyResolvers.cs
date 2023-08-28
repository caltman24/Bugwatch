using Bugwatch.Application.Interfaces;

namespace Bugwatch.Application;

/// <summary>
///     Resolves which implementation of an interface to use
/// </summary>
public delegate IRoleProjectRepository ProjectRepositoryResolver(string role);

/// <summary>
///     Resolves which implementation of an interface to use
/// </summary>
public delegate IRoleTicketRepository TicketRepositoryResolver(string role);
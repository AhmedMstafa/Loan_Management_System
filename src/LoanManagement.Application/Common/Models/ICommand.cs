using MediatR;

namespace LoanManagement.Application.Common.Models;

public interface ICommand<out TResponse> : IRequest<TResponse>, ITransactionalCommand;
public interface ICommand : IRequest, ITransactionalCommand;

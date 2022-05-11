﻿using MediatR;
using DcslGs.Template.Common.Application.Commands.Contracts;

namespace DcslGs.Template.Common.Application.Commands;

public abstract class CommandBase : IRequest<ICommandResult>
{
    protected CommandBase()
    {
    }

    protected CommandBase(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
}

public abstract class CommandBase<TResult> : IRequest<ICommandResult<TResult>>
{
    protected CommandBase()
    {
    }

    protected CommandBase(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
}
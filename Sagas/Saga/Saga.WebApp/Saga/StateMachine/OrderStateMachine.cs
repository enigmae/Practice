﻿using Automatonymous;
using Saga.WebApp.RB;

namespace Saga.WebApp.Saga.StateMachine;

public class OrderStateMachine : MassTransitStateMachine<OrderStateData>
{
    public State Validation { get; private set; }

    public Event<IOrderStartEvent> StartOrderProcess { get; private set; }

    public OrderStateMachine()
    {
        InstanceState(s => s.CurrentState);

        Event(() => StartOrderProcess, x => x.CorrelateById(m => m.Message.OrderId));

        Initially(
            When(StartOrderProcess)
                .Then(context =>
                {
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.PaymentCardNumber = context.Data.CardNumber;
                    context.Instance.ProductName = context.Data.ProductName;
                })
                .TransitionTo(Validation)
                .Publish(context => new CardValidateEvent(context.Instance))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}
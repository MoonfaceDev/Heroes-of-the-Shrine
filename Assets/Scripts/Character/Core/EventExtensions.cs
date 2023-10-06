using System;
using ExtEvents;

public static class EventExtensions
{
    public static Action SubscribeOnce(this ExtEvent @event, Action action){
        Action invokeThenUnsubscribe = null;
        invokeThenUnsubscribe = () => {
            action.Invoke();
            @event -= invokeThenUnsubscribe;
        };
    
        @event += invokeThenUnsubscribe;
        return invokeThenUnsubscribe;
    }
}
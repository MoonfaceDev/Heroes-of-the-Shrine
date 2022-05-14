public delegate bool CallbackCondition();
public delegate void CallbackAction();

public class CallbackEvent : IEventListener
{
    private readonly EventManager eventManager;
    private readonly CallbackCondition callbackCondition;
    private readonly CallbackAction callbackAction;

    public CallbackEvent(EventManager eventManager, CallbackCondition callbackCondition, CallbackAction callbackAction)
    {
        this.eventManager = eventManager;
        this.callbackCondition = callbackCondition;
        this.callbackAction = callbackAction;
    }

    public void Callback()
    {
        callbackAction();
        eventManager.DetachEvent(this);
    }

    public bool Validate()
    {
        return this.callbackCondition();
    }
}
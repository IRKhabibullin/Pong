using System;
using UnityEngine;

public abstract class BaseChannel : ScriptableObject
{
    public void SetCallbacks(object subscriberObject)
    {
        foreach (var @event in GetType().GetEvents())
        {
            var handlerType = @event.EventHandlerType;
            var d = Delegate.CreateDelegate(handlerType, subscriberObject, @event.Name + "Handler", false, false);
            if (d != null)
            {
                @event.AddEventHandler(this, d);
            }
        }
    }
    
    public void ResetCallbacks(object subscriberObject)
    {
        foreach (var @event in GetType().GetEvents())
        {
            var handlerType = @event.EventHandlerType;
            var d = Delegate.CreateDelegate(handlerType, subscriberObject, @event.Name + "Handler", false, false);
            if (d != null)
            {
                @event.RemoveEventHandler(this, d);
            }
        }
    }
}
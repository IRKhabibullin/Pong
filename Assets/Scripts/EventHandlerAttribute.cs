using System;

[AttributeUsage(AttributeTargets.Method)  
] 
public class EventHandlerAttribute : Attribute
{
    public Type channel;
    public int action;

    public EventHandlerAttribute(Type channel, int action)
    {
        this.channel = channel;
        this.action = action;
    }
}
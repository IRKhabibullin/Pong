using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscoveryGamesList : MonoBehaviour
{
    public List<MatchData> messages;
    

    public void HandleFoundMessage(MatchData message)
    {
        if (messages.Any(broadcastMessage => broadcastMessage.name == message.name))
        {
            return;
        }

        messages.Add(message);
    }
}
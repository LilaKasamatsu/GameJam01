using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentStack 
{
    public int agentAmount;
    public int agentFoundation;
    public int agentStructure;

    //Contains the currently available to place agents
    public AgentStack(int newAgentAmount, int newAgentFoundation, int newAgentStructure)
    {
        agentAmount = newAgentAmount;
        agentFoundation = newAgentFoundation;
        agentStructure = newAgentStructure;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentStack 
{
    public int agentStructure;
    public int agentFoundation;
    public int agentPoint;

    //Contains the currently available to place agents
    public AgentStack(int newAgentStructure, int newAgentFoundation, int newAgentPoint)
    {
        agentStructure = newAgentStructure;
        agentFoundation = newAgentFoundation;
        agentPoint = newAgentPoint;

    }
}

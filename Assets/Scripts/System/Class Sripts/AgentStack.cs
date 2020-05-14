using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentStack 
{
    public int agentAmount;

    public int agentAmountFoundation;
    public int agentAmountStructure;
    public int agentAmountBranch;



    public int agentFoundation;
    public int agentStructure;
    public int agentBranch;

    //Contains the currently available to place agents
    public AgentStack(int newAgentAmount, int newAgentAmountStructure, int newAgentAmountFoundation, int newAgentAmountBranch, int newAgentFoundation, int newAgentStructure, int newAgentBranch)
    {
        agentAmount = newAgentAmount;
        agentAmountFoundation = newAgentAmountFoundation;
        agentAmountStructure = newAgentAmountStructure;
        agentAmountBranch = newAgentAmountBranch;


        agentFoundation = newAgentFoundation;
        agentStructure = newAgentStructure;
        agentBranch = newAgentBranch;

    }
}

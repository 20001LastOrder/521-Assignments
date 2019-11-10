using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSpaceSearch
{
    private List<Action> _actions;
    private Func<WorldState, WorldState, int> _stateGrader;
    private WorldState _goalState;

    public StateSpaceSearch( WorldState goalState, Func<WorldState, WorldState, int> stateGrader, List<Action> actions)
    {
        _stateGrader = stateGrader;
        _goalState = goalState;
        _actions = actions;
    }

    public Stack<Action> Plan(WorldState init_state)
    {
        var stateSpaces = new PriorityQueue<int, WorldState>();
        var init_score = _stateGrader(init_state, _goalState);
        stateSpaces.Enqueue(init_score, init_state.Clone());

        var plan = new Stack<Action>();
        WorldState lastState = null;
        var visitedStates = new HashSet<WorldState>(); ;
        int i = 0;
        bool findSolution = false;
        while(!stateSpaces.IsEmpty() && !findSolution)
        {
            i += 1;
            var state = stateSpaces.Dequeue();
            visitedStates.Add(state);
            foreach (var action in _actions)
            {
                if (action.PreCondition(state))
                {
                    var nextState = action.Effect(state);
                    nextState.LastAction = action;
                    nextState.PreviousState = state;
                   // nextState.steps = state.steps + action.Cost;

                    if (nextState.Reaches(_goalState))
                    {
                        lastState = nextState;
                        findSolution = true;
                        break;
                    }

                    var score = _stateGrader(nextState, _goalState);
                    if (!visitedStates.Contains(nextState))
                    {
                        stateSpaces.Enqueue(score, nextState);
                    }
                }
            }
        }

        Debug.Log(i);
        if (lastState == null)
        {
            throw new Exception("Failed");
        }

        while(lastState != null)
        {
            if(lastState.LastAction != null)
            {
                plan.Push(lastState.LastAction);
            }
            lastState = lastState.PreviousState;
        }

        return plan;
    }
}

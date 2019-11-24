using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// State Space search implementing the Search (Can be any kind of search depends on the state Grader function)
public class StateSpaceSearch
{
    private List<Action> _actions;
    private Func<WorldState, WorldState, int> _stateGrader;
    private WorldState _goalState;

    // given goal state, a function to evaluate the state and a set of actions
    public StateSpaceSearch( WorldState goalState, Func<WorldState, WorldState, int> stateGrader, List<Action> actions)
    {
        _stateGrader = stateGrader;
        _goalState = goalState;
        _actions = actions;
    }

    // go from a init_state to the goal state with the list of actions specified
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

        // planning loop
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
                    nextState.steps = state.steps + action.Cost; // compute forward cost for the current state

                    // check if any of the next states is the goal state
                    if (nextState.Reaches(_goalState))
                    {
                        lastState = nextState;
                        findSolution = true;
                        break;
                    }

                    // calculate score for the state
                    var score = _stateGrader(nextState, _goalState);
                    if (!visitedStates.Contains(nextState))
                    {
                        stateSpaces.Enqueue(score, nextState);
                    }
                }
            }
        }

        // log steps
        Debug.Log(i);
        if (lastState == null)
        {
            throw new Exception("Failed");
        }

        // get the plan
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

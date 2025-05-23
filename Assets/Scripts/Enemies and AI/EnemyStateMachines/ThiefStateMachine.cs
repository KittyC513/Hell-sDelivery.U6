using UnityEngine;

public class ThiefStateMachine : StateManager<ThiefStateMachine.ThiefStates>
{
    public enum ThiefStates { idle, sneak, steal, run }
    [SerializeField] private ThiefBase thiefBase;

    [SerializeField] private ThiefStates showCurrentState;

    private void Awake()
    {
        states.Add(ThiefStates.idle, new ThiefIdle(ThiefStates.idle, thiefBase));
        states.Add(ThiefStates.sneak, new ThiefSneak(ThiefStates.sneak, thiefBase));
        states.Add(ThiefStates.run, new ThiefRun(ThiefStates.run, thiefBase));
        states.Add(ThiefStates.steal, new ThiefSteal(ThiefStates.steal, thiefBase));

        currentState = states[ThiefStates.idle];
    }

    private void LateUpdate()
    {
        showCurrentState = currentState.stateKey;
    }

    public void OverrideState(ThiefStates state)
    {
        TransitionToState(state);
    }
}

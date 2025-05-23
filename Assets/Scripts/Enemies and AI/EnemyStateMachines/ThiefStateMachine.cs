using UnityEngine;

public class ThiefStateMachine : StateManager<ThiefStateMachine.ThiefStates>
{
    public enum ThiefStates { idle, sneak, steal, run }
    [SerializeField] private ThiefBase thiefBase;

    private void Awake()
    {
        states.Add(ThiefStates.sneak, new ThiefSneak(ThiefStates.sneak, thiefBase));
    }
}

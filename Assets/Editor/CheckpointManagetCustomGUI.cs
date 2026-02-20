using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(CheckpointManager))]
public class CheckpointManagetCustomGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //target is the object being inspected
        CheckpointManager checkpointManager = (CheckpointManager)target;

     
        if (GUILayout.Button("Respawn Players"))
        {
            if (Application.isPlaying)
            {
            checkpointManager.RespawnPlayers();
            }
            
        }
        
      
    }
}

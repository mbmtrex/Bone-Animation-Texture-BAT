using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BATRecorder))]
public class BATRecorderMenu : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default Inspector UI
        base.OnInspectorGUI();
        
        BATRecorder myTarget2 = (BATRecorder)target;
        
        if (GUILayout.Button("Update Info"))
        {
            myTarget2.Initalize();
        }
        
        if (GUILayout.Button("Record"))
        {
            myTarget2.Initalize();
            myTarget2.RecordAnim();
        }
        
        GUILayout.Label("There are " + myTarget2._clipCount + " clips in the Animator");
        GUILayout.Label("Default shader graph only supports 3 Animations to support more extend the graph");
        
        for (int i=0;i<myTarget2._clipCount;i++)
        {
            GUILayout.Label("Clip " + i + " is " + myTarget2._animationClip[i].length*myTarget2._animationClip[0].frameRate +" Frame Long");
        }
        
        GUILayout.Label("Texture width (Bone Count) : " + myTarget2._textureWidth);
        GUILayout.Label("Texture height (Total Frame Count) : " + myTarget2._textureHeight);
    }
}
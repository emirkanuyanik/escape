using UnityEditor;
using UnityEngine;

public class AutoGameSetup : EditorWindow
{
    [MenuItem("CardGame/Setup Scene Automatically")]
    public static void SetupScene()
    {
        if (EditorUtility.DisplayDialog("Sahne Kur",
            "Sahne temizlenip oyun otomatik kurulacak.\nDevam etmek istiyor musun?", "Evet", "Hayir"))
        {
            foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
            {
                DestroyImmediate(obj);
            }
        }
        else return;

        // Camera
        GameObject cam = new GameObject("Main Camera");
        Camera c = cam.AddComponent<Camera>();
        c.clearFlags = CameraClearFlags.SolidColor;
        c.backgroundColor = new Color(0.05f, 0.15f, 0.05f);
        cam.transform.position = new Vector3(0, 0, -10);
        cam.tag = "MainCamera";

        // Single bootstrapper - does EVERYTHING
        GameObject game = new GameObject("_GAME_");
        game.AddComponent<GameBootstrapper>();

        Debug.Log("<color=green>SAHNE KURULDU! Play tusuna bas ve oyna!</color>");
    }
}

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RR95Tools
{
    [MenuItem("RR95Tools/PlayerPrefs/Clear All Data")]
    public static void ClearAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog(
            "PlayerPrefs 삭제 확인",
            "모든 PlayerPrefs 데이터를 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다.",
            "삭제",
            "취소"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("<color=red>[PlayerPrefs]</color> 모든 데이터가 삭제되었습니다.");
            EditorUtility.DisplayDialog("완료", "모든 PlayerPrefs 데이터가 삭제되었습니다.", "확인");
        }
    }

    [MenuItem("RR95Tools/PlayerPrefs/Clear All Data (No Confirm)")]
    public static void ClearAllPlayerPrefsNoConfirm()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("<color=red>[PlayerPrefs]</color> 모든 데이터가 삭제되었습니다.");
    }

    [MenuItem("RR95Tools/PlayerPrefs/Save Current Data")]
    public static void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
        Debug.Log("<color=green>[PlayerPrefs]</color> 현재 데이터가 저장되었습니다.");
    }

    [MenuItem("RR95Tools/PlayerPrefs/Open Data Location")]
    public static void OpenPlayerPrefsLocation()
    {
        string path = "";

#if UNITY_EDITOR_WIN
        path = @"HKEY_CURRENT_USER\Software\Unity\UnityEditor\" + Application.companyName + @"\" + Application.productName;
        EditorUtility.DisplayDialog(
            "PlayerPrefs 위치",
            "Windows Registry 경로:\n" + path + "\n\nRegedit에서 확인할 수 있습니다.",
            "확인");
        Debug.Log($"<color=cyan>[PlayerPrefs Location]</color> {path}");
#elif UNITY_EDITOR_OSX
        path = "~/Library/Preferences/unity." + Application.companyName + "." + Application.productName + ".plist";
        EditorUtility.DisplayDialog(
            "PlayerPrefs 위치",
            "Mac 파일 경로:\n" + path,
            "확인");
        Debug.Log($"<color=cyan>[PlayerPrefs Location]</color> {path}");
#elif UNITY_EDITOR_LINUX
        path = "~/.config/unity3d/" + Application.companyName + "/" + Application.productName;
        EditorUtility.DisplayDialog(
            "PlayerPrefs 위치",
            "Linux 파일 경로:\n" + path,
            "확인");
        Debug.Log($"<color=cyan>[PlayerPrefs Location]</color> {path}");
#endif
    }

    [MenuItem("RR95Tools/PlayerPrefs/Log All Keys (Requires Custom Implementation)")]
    public static void LogAllKeys()
    {
        EditorUtility.DisplayDialog(
            "알림",
            "PlayerPrefs는 모든 키를 나열하는 기본 기능을 제공하지 않습니다.\n" +
            "LocalSaveMgr에서 사용하는 키 목록을 수동으로 추가해야 합니다.",
            "확인");

        Debug.Log("<color=yellow>[PlayerPrefs]</color> 저장된 키 목록을 확인하려면 코드에 키 목록을 추가하세요.");
    }
}
#endif

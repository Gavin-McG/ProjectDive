using UnityEngine;

public class SpawnManagerObjects
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void SpawnManagers()
    {
        var managerSOs = Resources.LoadAll<ManagerObject>("");

        foreach (var managerSO in managerSOs)
        {
            //Create and mark object as DoNotDestroy
            var manager = GameObject.Instantiate(managerSO.prefab);
            GameObject.DontDestroyOnLoad(manager);
        }
    }
}

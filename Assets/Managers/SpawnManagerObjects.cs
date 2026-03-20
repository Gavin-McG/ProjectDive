using UnityEngine;

public class SpawnManagerObjects
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void SpawnManagers()
    {
        var managerSOs = Resources.LoadAll<ManagerObject>("");

        foreach (var managerSO in managerSOs)
        {
            //Create and mark object as DoNotDestroy
            var managerObject = Object.Instantiate(managerSO.prefab);
            Object.DontDestroyOnLoad(managerObject);
            
            //Register Provided Managers with static Managers class
            var managerProvider = managerObject.GetComponent<ManagerProvider>();
            if (managerProvider != null)
            {
                foreach (var manager in managerProvider.managers)
                {
                    if (manager == null) continue;
                    Managers.Add(manager);
                }
            }
        }
    }
}

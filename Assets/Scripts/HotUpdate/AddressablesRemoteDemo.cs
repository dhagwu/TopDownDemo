using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesRemoteDemo : MonoBehaviour
{
    [SerializeField] private string remotePrefabKey = "remote_test_prefab";
    [SerializeField] private Transform spawnPoint;

    [Header("Debug")]
    [SerializeField] private bool forceRedownloadOnce = false;

    private GameObject spawnedInstance;

    public IEnumerator CheckForUpdatesAndSpawn()
    {
        // 1. 检查 catalog 更新
        var checkHandle = Addressables.CheckForCatalogUpdates();
        yield return checkHandle;

        if (checkHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("[AddressablesRemoteDemo] CheckForCatalogUpdates failed.");
            yield break;
        }

        if (checkHandle.Result != null && checkHandle.Result.Count > 0)
        {
            Debug.Log($"[AddressablesRemoteDemo] Found {checkHandle.Result.Count} catalog update(s).");

            var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result);
            yield return updateHandle;

            if (updateHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("[AddressablesRemoteDemo] Catalog updated.");

                // 清掉“已不再被当前 catalog 引用”的旧 bundle
                var cleanHandle = Addressables.CleanBundleCache();
                yield return cleanHandle;

                if (cleanHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("[AddressablesRemoteDemo] Old bundle cache cleaned.");
                }
                else
                {
                    Debug.LogWarning("[AddressablesRemoteDemo] CleanBundleCache failed.");
                }

                Addressables.Release(cleanHandle);
            }
            else
            {
                Debug.LogError("[AddressablesRemoteDemo] UpdateCatalogs failed.");
                Addressables.Release(updateHandle);
                Addressables.Release(checkHandle);
                yield break;
            }

            Addressables.Release(updateHandle);
        }
        else
        {
            Debug.Log("[AddressablesRemoteDemo] No catalog update found.");
        }

        Addressables.Release(checkHandle);

        // 2. 只有你主动勾选时，才强制清这个 key 的缓存
        if (forceRedownloadOnce)
        {
            var clearHandle = Addressables.ClearDependencyCacheAsync(remotePrefabKey, true);
            yield return clearHandle;

            if (clearHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"[AddressablesRemoteDemo] Cleared cache for key: {remotePrefabKey}");
            }
            else
            {
                Debug.LogWarning($"[AddressablesRemoteDemo] ClearDependencyCacheAsync failed for key: {remotePrefabKey}");
            }

            Addressables.Release(clearHandle);

            // 执行一次后自动关掉，避免每次启动都清缓存
            forceRedownloadOnce = false;
        }

        // 3. 加载远程 Prefab
        var loadHandle = Addressables.LoadAssetAsync<GameObject>(remotePrefabKey);
        yield return loadHandle;

        if (loadHandle.Status != AsyncOperationStatus.Succeeded || loadHandle.Result == null)
        {
            Debug.LogError($"[AddressablesRemoteDemo] Load failed: {remotePrefabKey}");
            yield break;
        }

        if (spawnedInstance != null)
        {
            Destroy(spawnedInstance);
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        spawnedInstance = Instantiate(loadHandle.Result, pos, Quaternion.identity);

        Debug.Log($"[AddressablesRemoteDemo] Spawned remote prefab: {remotePrefabKey}");
    }
}
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdateBootstrap : MonoBehaviour
{
    [Header("Server")]
    [SerializeField] private string manifestUrl = "http://127.0.0.1:8000/api/manifest";

    [Header("Runtime References")]
    [SerializeField] private PlayerConfigSO playerConfig;
    [SerializeField] private LuaHotfixRuntime luaRuntime;
    [SerializeField] private AddressablesRemoteDemo addressablesRemoteDemo;

    private ServerManifest manifest;

    private string CacheRoot => Path.Combine(Application.persistentDataPath, "HotUpdate");

    private IEnumerator Start()
    {
        Directory.CreateDirectory(CacheRoot);
        Directory.CreateDirectory(Path.Combine(CacheRoot, "config"));
        Directory.CreateDirectory(Path.Combine(CacheRoot, "lua"));

        yield return FetchManifest();

        if (manifest != null)
        {
            yield return DownloadFiles(manifest.configs);
            yield return DownloadFiles(manifest.luaFiles);
        }
        else
        {
            Debug.LogWarning("[HotUpdateBootstrap] Manifest fetch failed. Will use cached files if present.");
        }

        ApplyPlayerConfigFromCache();

        if (luaRuntime != null)
        {
            luaRuntime.Init();
            luaRuntime.ApplyPatch("patches.player_patch");
        }

        if (addressablesRemoteDemo != null)
        {
            yield return addressablesRemoteDemo.CheckForUpdatesAndSpawn();
        }

        Debug.Log("[HotUpdateBootstrap] All hot-update steps finished.");
    }

    private IEnumerator FetchManifest()
    {
        using var req = UnityWebRequest.Get(manifestUrl);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[HotUpdateBootstrap] FetchManifest failed: " + req.error);
            manifest = null;
            yield break;
        }

        manifest = JsonUtility.FromJson<ServerManifest>(req.downloadHandler.text);
        Debug.Log("[HotUpdateBootstrap] Manifest loaded.");
    }

    private IEnumerator DownloadFiles(string[] relativePaths)
    {
        if (relativePaths == null || manifest == null)
            yield break;

        foreach (var relativePath in relativePaths)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                Debug.LogError("[HotUpdateBootstrap] Empty relative path, skipped.");
                continue;
            }

            string baseUrl = manifest.baseUrl.TrimEnd('/');
            string url = $"{baseUrl}/{relativePath}";
            string localPath = Path.Combine(CacheRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

            string dir = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Debug.Log($"[HotUpdateBootstrap] Downloading: {url}");

            using var req = UnityWebRequest.Get(url);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[HotUpdateBootstrap] Download failed: {url} | {req.error} | code={req.responseCode}");
                continue;
            }

            string text = req.downloadHandler.text;
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogError($"[HotUpdateBootstrap] Downloaded text is empty: {url}");
                continue;
            }

            File.WriteAllText(localPath, text);
            Debug.Log($"[HotUpdateBootstrap] Saved text file: {localPath}");
        }
    }

    private void ApplyPlayerConfigFromCache()
    {
        if (playerConfig == null)
        {
            Debug.LogError("[HotUpdateBootstrap] playerConfig is null.");
            return;
        }

        string localPath = Path.Combine(CacheRoot, "config", "player_config.json");
        if (!File.Exists(localPath))
        {
            Debug.LogWarning("[HotUpdateBootstrap] No cached player_config.json found. Keep original ScriptableObject values.");
            return;
        }

        string json = File.ReadAllText(localPath);
        RemotePlayerConfig cfg = JsonUtility.FromJson<RemotePlayerConfig>(json);

        playerConfig.moveSpeed = cfg.moveSpeed;
        playerConfig.turnSpeed = cfg.turnSpeed;
        playerConfig.gravity = cfg.gravity;

        playerConfig.dashSpeed = cfg.dashSpeed;
        playerConfig.dashDuration = cfg.dashDuration;
        playerConfig.dashCooldown = cfg.dashCooldown;

        playerConfig.attackDamage = cfg.attackDamage;
        playerConfig.attackRadius = cfg.attackRadius;
        playerConfig.attackCooldown = cfg.attackCooldown;

        playerConfig.maxHp = cfg.maxHp;

        Debug.Log($"[HotUpdateBootstrap] Config applied. moveSpeed={playerConfig.moveSpeed}, attackDamage={playerConfig.attackDamage}, maxHp={playerConfig.maxHp}");
    }
}
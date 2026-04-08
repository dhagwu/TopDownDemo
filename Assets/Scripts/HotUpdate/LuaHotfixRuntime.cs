using System.IO;
using UnityEngine;
using XLua;

public class LuaHotfixRuntime : MonoBehaviour
{
    private LuaEnv env;

    public void Init()
    {
        if (env != null) return;

        env = new LuaEnv();
        env.AddLoader(CustomLoader);
        env.DoString("require 'main'");
        Debug.Log("[LuaHotfixRuntime] Lua initialized.");
    }

    public void ApplyPatch(string moduleName)
    {
        if (env == null)
        {
            Debug.LogError("[LuaHotfixRuntime] LuaEnv is null.");
            return;
        }

        env.DoString($"package.loaded['{moduleName}'] = nil");
        env.DoString($"require '{moduleName}'");
        Debug.Log($"[LuaHotfixRuntime] Patch applied: {moduleName}");
    }

    private byte[] CustomLoader(ref string filepath)
    {
        string relativePath = filepath.Replace('.', '/');
        string localPath = Path.Combine(
            Application.persistentDataPath,
            "HotUpdate",
            "lua",
            relativePath + ".lua.txt"
        );

        if (File.Exists(localPath))
        {
            filepath = localPath;
            return File.ReadAllBytes(localPath);
        }

        Debug.LogWarning($"[LuaHotfixRuntime] Lua file not found: {localPath}");
        return null;
    }

    private void Update()
    {
        env?.Tick();
    }

    private void OnDestroy()
    {
        if (env == null)
            return;

        try
        {
            env.DoString("xlua.hotfix(CS.PlayerAttack, 'GetDamage', nil)");
            env.Tick();
            env.Dispose();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[LuaHotfixRuntime] Dispose warning: {e.Message}");
        }
        finally
        {
            env = null;
        }
    }
}
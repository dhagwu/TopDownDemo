using System;

[Serializable]
public class ServerManifest
{
    public string baseUrl;
    public string[] configs;
    public string[] luaFiles;
}

[Serializable]
public class RemotePlayerConfig
{
    public float moveSpeed;
    public float turnSpeed;
    public float gravity;

    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;

    public int attackDamage;
    public float attackRadius;
    public float attackCooldown;

    public int maxHp;
}
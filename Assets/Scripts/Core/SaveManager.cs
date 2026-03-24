using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference saveAction;
    [SerializeField] private InputActionReference loadAction;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private HUDController hudController;

    [Header("Options")]
    [SerializeField] private bool autoLoadOnStart = false;

    private void Awake()
    {
        TryResolveReferences();
    }

    private void Start()
    {
        if (autoLoadOnStart && SaveSystem.HasSave())
        {
            LoadGame();
        }
    }

    private void OnEnable()
    {
        if (saveAction != null)
        {
            saveAction.action.Enable();
            saveAction.action.performed += OnSavePerformed;
        }

        if (loadAction != null)
        {
            loadAction.action.Enable();
            loadAction.action.performed += OnLoadPerformed;
        }
    }

    private void OnDisable()
    {
        if (saveAction != null)
        {
            saveAction.action.performed -= OnSavePerformed;
            saveAction.action.Disable();
        }

        if (loadAction != null)
        {
            loadAction.action.performed -= OnLoadPerformed;
            loadAction.action.Disable();
        }
    }

    private void TryResolveReferences()
    {
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (player == null && playerStats != null)
            player = playerStats.transform;

        if (playerController == null && player != null)
            playerController = player.GetComponent<CharacterController>();

        if (hudController == null)
            hudController = FindFirstObjectByType<HUDController>();
    }

    private void OnSavePerformed(InputAction.CallbackContext ctx)
    {
        SaveGame();
    }

    private void OnLoadPerformed(InputAction.CallbackContext ctx)
    {
        LoadGame();
    }

    public void SaveGame()
    {
        TryResolveReferences();

        if (player == null || playerStats == null || hudController == null)
        {
            Debug.LogWarning("Save failed: missing references.");
            return;
        }

        GameSaveData data = new GameSaveData
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            playerPosX = player.position.x,
            playerPosY = player.position.y,
            playerPosZ = player.position.z,
            playerCurrentHp = playerStats.CurrentHp,
            killCount = hudController.CurrentKillCount
        };

        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        TryResolveReferences();

        GameSaveData data = SaveSystem.Load();
        if (data == null)
            return;

        if (player == null || playerStats == null || hudController == null)
        {
            Debug.LogWarning("Load failed: missing references.");
            return;
        }

        Vector3 loadedPosition = new Vector3(
            data.playerPosX,
            data.playerPosY,
            data.playerPosZ
        );

        if (playerController != null)
            playerController.enabled = false;

        player.position = loadedPosition;

        if (playerController != null)
            playerController.enabled = true;

        playerStats.SetHpFromSave(data.playerCurrentHp);
        hudController.SetKillCount(data.killCount);
    }
}
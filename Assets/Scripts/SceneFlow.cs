using UnityEngine;
using UnityEngine.SceneManagement;

//moves the player between the ship (GameScene) and the CubeSat designer.
//the designer replaces the ship scene, so the player's spot is remembered here and put back when the ship loads again.
public static class SceneFlow
{
    public const string StartSceneName = "StartScene";
    public const string ShipSceneName = "GameScene";
    public const string DesignerSceneName = "cubesatdesigner";

    //where the player was standing when they left the ship, null = use the scene's spawn point
    private static Vector2? returnPosition;

    //where the player waws standing on the ship, or where they left it if they're in the designer.
    //setting it picks where the player starts the next time the ship loads
    public static Vector2? ShipPosition
    {
        get
        {
            GameObject player = GameObject.FindWithTag("Player");
            return player != null ? (Vector2)player.transform.position : returnPosition;
        }
        set { returnPosition = value; }
    }

    public static void OpenDesigner(Vector2 playerPosition)
    {
        returnPosition = playerPosition;
        SceneManager.LoadScene(DesignerSceneName);
    }

    public static void ReturnToShip()
    {
        SceneManager.LoadScene(ShipSceneName);
    }

    //clears the remembered spot so a new game or loaded save starts at the spawn point
    public static void ResetShipPosition()
    {
        returnPosition = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == ShipSceneName)
        {
            PutPlayerBack();
        }
    }

    private static void PutPlayerBack()
    {
        if (returnPosition == null)
        {
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            return;
        }

        Vector2 position = returnPosition.Value;
        player.transform.position = new Vector3(position.x, position.y, player.transform.position.z);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = position;
        }

        //snap the camera too
        Camera cam = Camera.main;
        if (cam != null)
        {
            CameraFollow follow = cam.GetComponent<CameraFollow>();
            float yOffset = follow != null ? follow.yOffset : 0f;
            cam.transform.position = new Vector3(position.x, position.y + yOffset, cam.transform.position.z);
        }
    }
}

using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float height = 30f;
    public bool rotateWithPlayer = false;

    [Header("Minimap UI")]
    public GameObject minimapUI;
    public KeyCode toggleKey = KeyCode.M;

    private bool isVisible = true;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        if (minimapUI != null)
        {
            minimapUI.SetActive(isVisible);
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleMinimap();
        }

        if (target == null)
        {
            return;
        }

        transform.position = new Vector3(
            target.position.x,
            target.position.y + height,
            target.position.z
        );

        if (rotateWithPlayer)
        {
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    public void ToggleMinimap()
    {
        isVisible = !isVisible;

        if (minimapUI != null)
        {
            minimapUI.SetActive(isVisible);
        }
    }
}
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AbilityEvents))]
public class TargetEnemy : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] renderers;
    [SerializeField] Material normalMat;
    [SerializeField] Material outlinedMat;

    [SerializeField] public Transform target;
    bool isSelected;
    bool mouseOver;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<SpriteRenderer>(true);

        // start normal
        SetOutlined(false);
    }

    void OnMouseEnter()
    {
        HighlightEnemy();
        mouseOver = true;
        GameEventManager.Instance.targetEvents.RaiseOnAimEnemy();
    }

    void HighlightEnemy()
    {
        if (!isSelected) SetOutlined(true);
        GameEventManager.Instance.targetEvents.RaiseOnRemoveAim();
    }

    void OnMouseExit()
    {
        RemoveHighlight();
        mouseOver = false;
        GameEventManager.Instance.targetEvents.RaiseOnRemoveAim();
    }

    void RemoveHighlight()
    {
        if (!isSelected) SetOutlined(false);
    }

    void Update()
    {
        // Only check clicks if mouse is over this enemy and not over UI
        if (mouseOver && (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject()))
        {
            if (Input.GetMouseButtonDown(1))
            {
                isSelected = true;
                SetOutlined(isSelected);
                GameEventManager.Instance.targetEvents.RaiseOnTargetEnemy(this);
            }
        }
    }

    public void DeselectTarget()
    {
        SetOutlined(false);
        isSelected = false;
    }

    void OnDisable()
    {
        SetOutlined(false);
        isSelected = false;
    }

    void SetOutlined(bool outlined)
    {
        if (renderers == null) return;
        var mat = outlined ? outlinedMat : normalMat;
        foreach (var sr in renderers)
            if (sr) sr.sharedMaterial = mat;
    }
}
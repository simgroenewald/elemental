using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AbilityEvents))]
public class TargetEnemy : MonoBehaviour, ITargetable
{
    [SerializeField] SpriteRenderer mainRenderer;
    [SerializeField] Material normalMat;
    [SerializeField] Material outlinedMat;
    public Enemy enemy;
    public Transform target;

    Material normalInst;
    Material outlinedInst;

    bool isHovered;
    bool isSelected;
    bool mouseOver;

    MaterialPropertyBlock _mpb;

    void Awake()
    { 
        normalInst = new Material(normalMat);
        outlinedInst = new Material(outlinedMat);
        _mpb = new MaterialPropertyBlock();

        // start normal
        enemy = GetComponent<Enemy>();
        SetOutlined(false);
    }

    void OnMouseEnter()
    {
        if (enemy.room.isEntered)
        {
            HighlightEnemy();
            mouseOver = true;
            GameEventManager.Instance.targetEvents.RaiseOnAimEnemy();
        }
    }

    void HighlightEnemy()
    {
        if (!isSelected && !isHovered) SetOutlined(true);
        GameEventManager.Instance.targetEvents.RaiseOnRemoveAim();
    }

    void OnMouseExit()
    {
        RemoveHighlight();
        mouseOver = false;
        isHovered = false;
        GameEventManager.Instance.targetEvents.RaiseOnRemoveAim();
    }

    void RemoveHighlight()
    {
        if (!isSelected) SetOutlined(false);
    }

    void Update()
    {
        // Only check clicks if mouse is over this enemy and not over UI
        if (enemy.room.isEntered && mouseOver && (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject()))
        {
            if (Input.GetMouseButtonDown(1))
            {
                isSelected = true;
                SetOutlined(isSelected);
                GameEventManager.Instance.targetEvents.RaiseOnTargetEnemy(this);
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (GameManager.Instance.player.activeAbility.GetStagedAbility() == null || !GameManager.Instance.player.activeAbility.GetStagedAbility().abilityDetails.isEnemyTargetable) return;
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
        var mat = outlined ? outlinedInst : normalInst;
        mainRenderer.material = mat;
        mainRenderer.GetPropertyBlock(_mpb);
        _mpb.Clear();
        mainRenderer.SetPropertyBlock(_mpb);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public Transform GetTargetTransform()
    {
        return target;
    }
}
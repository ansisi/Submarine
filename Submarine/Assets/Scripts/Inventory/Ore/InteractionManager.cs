using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    private List<InteractableBase> allInteractables = new();
    private InteractableBase currentTarget;

    private Transform player;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float closestDistance = float.MaxValue;
        InteractableBase closest = null;

        foreach (var interactable in allInteractables)
        {
            if (!interactable.IsInRange(player)) continue;

            float dist = interactable.DistanceToPlayer(player);
            if (closest == null || dist < closestDistance ||
                (Mathf.Approximately(dist, closestDistance) && interactable.Priority < closest.Priority))
            {
                closest = interactable;
                closestDistance = dist;
            }
        }

        if (closest != currentTarget)
        {
            currentTarget = closest;

            if (currentTarget != null)
                PickupUIManager.instance.ShowHint(true, currentTarget.GetHintText());
            else
                PickupUIManager.instance.ShowHint(false);
        }

        if (currentTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentTarget.Interact();
                PickupUIManager.instance.ShowHint(false);
                currentTarget = null;
            }
        }
    }

    public void Register(InteractableBase obj)
    {
        if (!allInteractables.Contains(obj))
            allInteractables.Add(obj);
    }

    public void Unregister(InteractableBase obj)
    {
        if (allInteractables.Contains(obj))
            allInteractables.Remove(obj);
    }
}

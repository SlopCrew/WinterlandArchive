using System.Collections;
using Reptile;
using UnityEngine;

namespace Winterland.Common;

class Portal : MonoBehaviour {
    public Collider[] InnerTriggers;
    public Collider[] OuterTriggers;

    public Transform Exit;

    public float fadeOutDuration = 0;

    private bool touchingInner = false;
    private bool touchingOuter = false;

    void FixedUpdate() {

    }

    void OnTriggerEnter(Collider other) {
        var player = PlayerCollisionUtility.GetPlayer(other);
        if(player != null) {
            StartCoroutine(FadeTeleport(player));
        }
    }

    IEnumerator FadeTeleport(Player player) {
        if(fadeOutDuration > 0) {
            Core.Instance.UIManager.effects.FadeToBlack(fadeOutDuration);
            yield return new WaitForSeconds(fadeOutDuration);
        }

        Vector3 MoveRelative(Vector3 moveThisPosition, Transform fromHere, Transform toHere) {
            var relativePosition = fromHere.InverseTransformPoint(moveThisPosition);
            return toHere.TransformPoint(relativePosition);
        }

        // Compute player's position relative to this end.  Teleport them to the other end at that relative position.
        player.transform.position = MoveRelative(player.transform.position, transform, Exit);

        var camera = GameplayCamera.instance;
        camera.cameraMode.position = MoveRelative(camera.cameraMode.position, transform, Exit);
        camera.cameraMode.positionFinal = MoveRelative(camera.cameraMode.positionFinal, transform, Exit);
        camera.cameraMode.lookAtPos = MoveRelative(camera.cameraMode.lookAtPos, transform, Exit);

        if(fadeOutDuration > 0) {
            Core.Instance.UIManager.effects.FadeOpen(fadeOutDuration);
        }
    }
}
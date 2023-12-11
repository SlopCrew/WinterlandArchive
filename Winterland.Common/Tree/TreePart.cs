using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace Winterland.Common;

/// <summary>
/// Tree construction timeline tells this component to activate and deactivate.
/// This component tells the timeline to pause till it's finished animating.
/// </summary>
public class TreePart : MonoBehaviour, ITreePauseReason {
    public ITreeState state;
    public PlayableDirector appearanceDirector = null;
    public PlayableDirector disappearanceDirector = null;
    public PlayableDirector idleAnimationDirector = null;
    public GameObject[] idleAnimationGameObjects = null;
    public Animator animator = null;

    bool waitingForAppearDirectorToFinish = false;
    bool waitingForDisappearDirectorToFinish = false;

    public void Appear() {
        BeforeAppear();
        if(!Application.isEditor && !state.IsFastForwarding && StartAppearAnimation()) {
            // Wait for appearance animation
            state.ReasonsToBePaused.Add(this);
        } else {
            // No appearance animation
            AfterAppear();
            StartIdleAnimation();
        }
    }

    public void Disappear() {
        StopIdleAnimation();
        BeforeDisappear();
        if(!Application.isEditor && !state.IsFastForwarding && StartDisappearAnimation()) {
            // Wait for disappearance animation
            state.ReasonsToBePaused.Add(this);
        } else {
            // No disappearance animation
            AfterDisappear();
        }
    }

    void Update() {
        if(waitingForAppearDirectorToFinish && appearanceDirector.state != PlayState.Playing) {
            waitingForAppearDirectorToFinish = false;
            state.ReasonsToBePaused.Remove(this);
            AfterAppear();
            StartIdleAnimation();
        }
        if(waitingForDisappearDirectorToFinish && disappearanceDirector.state != PlayState.Playing) {
            waitingForDisappearDirectorToFinish = false;
            state.ReasonsToBePaused.Remove(this);
            AfterDisappear();
        }
    }

    public void ResetPart() {
        appearanceDirector?.Stop();
        disappearanceDirector?.Stop();
        StopIdleAnimation();
        state.ReasonsToBePaused.Remove(this);
        waitingForAppearDirectorToFinish = false;
        waitingForDisappearDirectorToFinish = false;
        if(animator != null) {
            animator.SetBoolString("Appear", false);
            animator.SetBoolString("Idle", false);
            animator.SetBoolString("Disappear", false);
            animator.Rebind();
            animator.Update(0f);
        }
        gameObject.SetActive(false);
    }

    void BeforeAppear() {
        gameObject.SetActive(true);
        animator?.SetBoolString("Appear", true);
        animator?.Update(0f);
    }
    bool StartAppearAnimation() {
        var shouldWait = false;
        if(appearanceDirector != null) {
            shouldWait = true;
            waitingForAppearDirectorToFinish = true;
            appearanceDirector.Play();
        }
        return shouldWait;
    }
    void AfterAppear() {

    }
    void StartIdleAnimation() {
        animator?.SetBoolString("Idle", true);
        animator?.Update(0f);
        if(idleAnimationDirector != null) idleAnimationDirector.Play();
        foreach(var go in idleAnimationGameObjects) {
            go.SetActive(true);
        }
    }
    void StopIdleAnimation() {
        if(idleAnimationDirector != null) idleAnimationDirector.Stop();
        foreach(var go in idleAnimationGameObjects) {
            go.SetActive(false);
        }
    }
    void BeforeDisappear() {
        animator?.SetBoolString("Disappear", true);
        animator?.Update(0f);
    }
    bool StartDisappearAnimation() {
        var shouldWait = false;
        if(disappearanceDirector != null) {
            shouldWait = true;
            waitingForDisappearDirectorToFinish = true;
            disappearanceDirector.Play();
        }
        return shouldWait;
    }

    void AfterDisappear() {
        gameObject.SetActive(false);
    }
}

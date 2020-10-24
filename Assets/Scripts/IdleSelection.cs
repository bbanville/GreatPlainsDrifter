using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BrendonBanville.Tools.InformationAttribute;

public class IdleSelection : MonoBehaviour
{
    [Information("Use Either Animated Idle or Static Idle. No Idle will occur if both are on", InformationType.Warning, false)]
    [Header("Idle Selection")]
    public bool IsCutsceneCharacter = false;

    /// <summary>
    /// 0 = No idle
    /// 1 = Idle
    /// 2 = Idle2
    /// 3 = Idle3
    /// </summary>
    public bool AnimatedIdle = false;
    [Condition("AnimatedIdle", true)]
    public int idleSelection = 0;

    /// <summary>
    /// 0 = No idle
    /// 1 = FemaleStandingPose
    /// 2 = SittingPose
    /// 3 = SittingPose2
    /// </summary>
    public bool StaticIdle = false;
    [Condition("StaticIdle", true)]
    public int idlePose = 0;

    public bool CutsceneAnim = false;
    [Condition("CutsceneAnim", true)]
    public int cutsceneAnim = 0;

    private Animator _animator;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _animator = this.GetComponent<Animator>();

        if (AnimatedIdle && idlePose == 0)
        {
            _animator.SetInteger("IdleSelection", idleSelection);
        }

        if (StaticIdle && idleSelection == 0)
        {
            _animator.SetInteger("IdlePose", idlePose);
        }

        if (CutsceneAnim)
        {
            _animator.SetInteger("CutsceneAnim", cutsceneAnim);
        }
    }

    void Update()
    {
        if (IsCutsceneCharacter)
        {
            if (CutsceneAnim)
            {
                _animator.SetInteger("CutsceneAnim", cutsceneAnim);
            }

            if (AnimatedIdle && idlePose == 0)
            {
                _animator.SetInteger("IdleSelection", idleSelection);
            }
        }
    }
}

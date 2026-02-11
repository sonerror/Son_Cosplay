using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Satisgame;
using HoangHH;
using System;
using System.Collections;
public class StepManager : Singleton<StepManager>
{
    [SerializeField] protected int currentStep = 0;
    public void SetCurrentStep(int step)
    {
        currentStep = step;
    }
    public int CurrentStep
    {
        get => currentStep;
        set => currentStep = value;
    }
    public int CurrentStepManager()
    {
        return currentStep;
    }
}

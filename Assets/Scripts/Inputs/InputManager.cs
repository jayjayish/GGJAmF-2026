using System;
using System.Collections.Generic;
using Data;
using Scenes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class InputManager
{
    private static InputActionAsset _actionAsset;
    private const string MoveKey = "Move";
    private const string AttackKey = "Attack";
    private const string LeftRotateKey = "LeftRotate";
    private const string RightRotateKey = "RightRotate";
    private const string AimKey = "Aim";
    
    private static List<Action<Vector2>> MoveActions = new ();
    private static List<Action<Vector2>> AimActions = new ();
    private static List<Action> AttackActions = new ();
    private static List<Action> LeftRotateActions = new ();
    private static List<Action> RightRotateActions = new ();

    private static Vector3 _mousePosition;

    public enum ActionEnum : byte
    {
        Attack,
        Left,
        Right
    }
    
    public static void Initialize(Action callback = null)
    {
        Addressables.LoadAssetAsync<InputActionAsset>(GlobalAddresses.InputActionsAddr).Completed += asyncHandle =>
        {
            if (asyncHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _actionAsset = asyncHandle.Result;
                InitializeListeners();
                callback?.Invoke();
            }
            else
            {
                Debug.LogError("InputActionAsset couldn't be loaded");
            }
        };
    }

    private static void InitializeListeners()
    {
        _actionAsset.FindAction(MoveKey).performed += InvokeMove;
        _actionAsset.FindAction(MoveKey).canceled += InvokeMove;
        _actionAsset.FindAction(AimKey).performed += InvokeAim;
        _actionAsset.FindAction(AimKey).canceled += InvokeAim;
        _actionAsset.FindAction(AttackKey).started += InvokeAttack;
        _actionAsset.FindAction(LeftRotateKey).started += InvokeLeft;
        _actionAsset.FindAction(RightRotateKey).started += InvokeRight;
        
        AddAimAction(OnMouseMove);
    }

    public static void AddMoveAction(Action<Vector2> callback)
    {
        MoveActions.Add(callback);
    }
    
    public static void RemoveMoveAction(Action<Vector2> callback)
    {
        MoveActions.Remove(callback);
    }
    
    public static void InvokeMove(InputAction.CallbackContext context)
    {
        
        var direction = context.ReadValue<Vector2>();
        foreach(Action<Vector2> action in MoveActions)
        {
            action.Invoke(direction);
        }
    }
    
    public static void AddAimAction(Action<Vector2> callback)
    {
        AimActions.Add(callback);
    }
    
    public static void RemoveAimAction(Action<Vector2> callback)
    {
        AimActions.Remove(callback);
    }
    
    public static void InvokeAim(InputAction.CallbackContext context)
    {
        
        var direction = context.ReadValue<Vector2>();
        foreach(Action<Vector2> action in AimActions)
        {
            action.Invoke(direction);
        }
    }
    
    public static void AddAttackAction(Action callback)
    {
        AttackActions.Add(callback);
    }
    
    public static void RemoveAttackAction(Action callback)
    {
        AttackActions.Remove(callback);
    }
    
    public static void InvokeAttack(InputAction.CallbackContext context)
    {
        foreach(Action action in AttackActions)
        {
            action.Invoke();
        }
    }
    
    public static void AddLeftAction(Action callback)
    {
        LeftRotateActions.Add(callback);
    }
    
    public static void RemoveLeftAction(Action callback)
    {
        LeftRotateActions.Remove(callback);
    }
    
    public static void InvokeLeft(InputAction.CallbackContext context)
    {
        foreach(Action action in LeftRotateActions)
        {
            action.Invoke();
        }
    }
    
    public static void AddRightAction(Action callback)
    {
        RightRotateActions.Add(callback);
    }
    
    public static void RemoveRightAction(Action callback)
    {
        RightRotateActions.Remove(callback);
    }
    
    public static void InvokeRight(InputAction.CallbackContext context)
    {
        foreach(Action action in RightRotateActions)
        {
            action.Invoke();
        }
    }

    public static void OnMouseMove(Vector2 mousePos)
    {
        if (Camera.main == null)
        {
            return;
        }
        _mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
        Debug.Log($"{_mousePosition}");
    }

    public static Vector3 GetMousePosition()
    {
        return _mousePosition;
    }
}

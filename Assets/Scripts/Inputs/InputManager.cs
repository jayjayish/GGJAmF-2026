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
    
    private static List<Action<Vector2>> _moveActions = new ();
    private static List<Action<Vector2>> _aimActions = new ();
    private static List<Action> _attackActions = new ();
    private static List<Action> _leftRotateDownActions = new ();
    private static List<Action> _rightRotateDownActions = new ();
    private static List<Action> _leftRotateUpActions = new ();
    private static List<Action> _rightRotateUpActions = new ();

    private static Vector3 _mouseWorldPosition;
    private static Vector2 _mouseScreenPosition;

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
        _actionAsset.FindAction(LeftRotateKey).started += InvokeLeftDown;
        _actionAsset.FindAction(RightRotateKey).started += InvokeRightDown;
        _actionAsset.FindAction(LeftRotateKey).canceled += InvokeLeftUp;
        _actionAsset.FindAction(RightRotateKey).canceled += InvokeRightUp;
        
        AddAimAction(OnMouseMove);
    }

    public static void AddMoveAction(Action<Vector2> callback)
    {
        _moveActions.Add(callback);
    }
    
    public static void RemoveMoveAction(Action<Vector2> callback)
    {
        _moveActions.Remove(callback);
    }
    
    public static void InvokeMove(InputAction.CallbackContext context)
    {
        CalculateMouseWorldPosition();
        var direction = context.ReadValue<Vector2>();
        foreach(Action<Vector2> action in _moveActions)
        {
            action.Invoke(direction);
        }
    }
    
    public static void AddAimAction(Action<Vector2> callback)
    {
        _aimActions.Add(callback);
    }
    
    public static void RemoveAimAction(Action<Vector2> callback)
    {
        _aimActions.Remove(callback);
    }
    
    public static void InvokeAim(InputAction.CallbackContext context)
    {
        
        var direction = context.ReadValue<Vector2>();
        foreach(Action<Vector2> action in _aimActions)
        {
            action.Invoke(direction);
        }
    }
    
    public static void AddAttackAction(Action callback)
    {
        _attackActions.Add(callback);
    }
    
    public static void RemoveAttackAction(Action callback)
    {
        _attackActions.Remove(callback);
    }
    
    public static void InvokeAttack(InputAction.CallbackContext context)
    {
        foreach(Action action in _attackActions)
        {
            action.Invoke();
        }
    }
    
    public static void AddLeftDownAction(Action callback)
    {
        _leftRotateDownActions.Add(callback);
    }
    
    public static void RemoveLeftDownAction(Action callback)
    {
        _leftRotateDownActions.Remove(callback);
    }
    
    public static void InvokeLeftDown(InputAction.CallbackContext context)
    {
        foreach(Action action in _leftRotateDownActions)
        {
            action.Invoke();
        }
    }
    
    public static void AddRightDownAction(Action callback)
    {
        _rightRotateDownActions.Add(callback);
    }
    
    public static void RemoveRightDownAction(Action callback)
    {
        _rightRotateDownActions.Remove(callback);
    }
    
    public static void InvokeRightDown(InputAction.CallbackContext context)
    {
        foreach(Action action in _rightRotateDownActions)
        {
            action.Invoke();
        }
    }
    
    public static void AddLeftUpAction(Action callback)
    {
        _leftRotateUpActions.Add(callback);
    }
    
    public static void RemoveLeftUpAction(Action callback)
    {
        _leftRotateUpActions.Remove(callback);
    }
    
    public static void InvokeLeftUp(InputAction.CallbackContext context)
    {
        foreach(Action action in _leftRotateUpActions)
        {
            action.Invoke();
        }
    }
    
    public static void AddRightUpAction(Action callback)
    {
        _rightRotateUpActions.Add(callback);
    }
    
    public static void RemoveRightUpAction(Action callback)
    {
        _rightRotateUpActions.Remove(callback);
    }
    
    public static void InvokeRightUp(InputAction.CallbackContext context)
    {
        foreach(Action action in _rightRotateUpActions)
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

        _mouseScreenPosition = mousePos;
        CalculateMouseWorldPosition();
    }

    public static Vector3 GetMouseWorldPosition()
    {
        return _mouseWorldPosition;
    }

    public static void CalculateMouseWorldPosition()
    {
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(_mouseScreenPosition.x, _mouseScreenPosition.y, -Camera.main.transform.position.z));
    }
}

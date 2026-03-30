using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EntityState<T> where T : Entity<T>
{
    public UnityEvent onEnter;
    public UnityEvent onExit;

    /// <summary>
    /// 记录实体进入该状态后经过的时间，单位为秒。
    /// 外部只读，内部或继承类可写。
    /// </summary>
    public float timeSinceEntered { get; protected set; }

    /// <summary>
    /// 进入该状态时调用，重置计时，触发进入事件，并调用子类实现的 OnEnter。
    /// </summary>
    /// <param name="entity">当前状态所属的实体对象。</param>
    public void Enter(T entity)
    {
        // 重置计时
        timeSinceEntered = 0;
        // 触发外部注册的进入事件回调
        onEnter?.Invoke();
        // 调用子类自定义的进入逻辑
        OnEnter(entity);
    }

    /// <summary>
    /// 退出该状态时调用，触发退出事件，并调用子类实现的 OnExit。
    /// </summary>
    /// <param name="entity">当前状态所属的实体对象。</param>
    public void Exit(T entity)
    {
        // 触发外部注册的退出事件回调
        onExit?.Invoke();
        // 调用子类自定义的退出逻辑
        OnExit(entity);
    }

    /// <summary>
    /// 每帧调用一次，执行状态持续期间的逻辑，并更新状态持续时间。
    /// </summary>
    /// <param name="entity">当前状态所属的实体对象。</param>
    public void Step(T entity)
    {
        // 调用子类实现的持续运行逻辑
        OnStep(entity);
        // 累计该状态已持续的时间，单位秒
        timeSinceEntered += Time.deltaTime;
    }

    /// <summary>
    /// 抽象方法，子类必须实现。
    /// 当状态被激活时调用，用于初始化该状态相关逻辑。
    /// </summary>
    /// <param name="entity">当前状态所属的实体对象。</param>
    protected abstract void OnEnter(T entity);

    /// <summary>
    /// 抽象方法，子类必须实现。
    /// 当状态被切换出去时调用，用于清理该状态相关逻辑。
    /// </summary>
    /// <param name="entity">当前状态所属的实体对象。</param>
    protected abstract void OnExit(T entity);

    /// <summary>
    /// 抽象方法，子类必须实现。
    /// 每帧调用，用于处理该状态下的持续逻辑。
    /// </summary>
    /// <param name="entity">当前状态所属的实体对象。</param>
    protected abstract void OnStep(T entity);

    /// <summary>
    /// 抽象方法，子类必须实现。
    /// 当实体与其他碰撞体接触时调用，用于处理碰撞相关逻辑。
    /// </summary>
    /// <param name="entity">当前状态所属的实体对象。</param>
    /// <param name="other">碰撞到的其他碰撞体。</param>
    public abstract void OnContact(T entity, Collider other);



    public static EntityState<T> CreateFromString(string typeName)
    {
        return (EntityState<T>)System.Activator.CreateInstance(System.Type.GetType(typeName));
    }
    public static List<EntityState<T>> CreateListFromStringArray(string[] array)
    {
        var list = new List<EntityState<T>>();
        foreach (var typeName in array)
        {
            list.Add(CreateFromString(typeName));
        }
        return list;
    }
}

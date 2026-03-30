using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class EntityStateManager : MonoBehaviour
{
    /// <summary>
    /// 状态管理相关事件集合（进入状态、退出状态、状态切换等）。
    /// 具体定义在 EntityStateManagerEvents 中。
    /// </summary>
    public EntityStateManagerEvents events;
}
public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T>
{
    protected List<EntityState<T>> m_list = new List<EntityState<T>>();
    protected Dictionary<Type, EntityState<T>> m_states = new Dictionary<Type, EntityState<T>>();
    public EntityState<T> current {  get; protected set; }
    public EntityState<T> last {  get; protected set; }
    /// <summary>
    /// 当前状态在状态列表中的索引位置。
    /// </summary>
    public int index => m_list.IndexOf(current);

    /// <summary>
    /// 上一个状态在状态列表中的索引位置。
    /// </summary>
    public int lastIndex => m_list.IndexOf(last);
    public T entity { get; protected set; }//该状态机关联的实体实例

    protected virtual void InitializeEntity() => entity = GetComponent<T>();
    protected virtual void Start()
    {
        InitializeStates();
        InitializeEntity();
    }
    protected abstract List<EntityState<T>> GetStateList();
    protected virtual void InitializeStates()
    {
        m_list= GetStateList();

        foreach(var state in m_list)
        {
            var type = state.GetType();
            if (!m_states.ContainsKey(type))
            {
                m_states.Add(type, state);
            }
        }

        if(m_list.Count > 0)
        {
            current = m_list[0];
        }
    }

    /// <summary>
    /// 判断当前状态是否为指定类型。
    /// </summary>
    /// <param name="type">需要比较的状态类型。</param>
    /// <returns>如果当前状态类型等于参数类型返回 true，否则返回 false。</returns>
    public virtual bool IsCurrentOfType(Type type)
    {
        if (current == null)
        {
            return false;
        }

        return current.GetType() == type;
    }

    /// <summary>
    /// 根据状态列表索引切换当前状态。
    /// </summary>
    /// <param name="to">目标状态在状态列表中的索引。</param>
    public virtual void Change(int to)
    {
        if (to >= 0 && to < m_list.Count)
        {
            Change(m_list[to]);
        }
    }


    /// <summary>
    /// 判断状态管理器是否包含指定类型的状态。
    /// </summary>
    /// <param name="type">状态类型。</param>
    /// <returns>如果包含返回 true，否则 false。</returns>
    public virtual bool ContainsStateOfType(Type type) => m_states.ContainsKey(type);

    /// <summary>
    /// 每帧调用，用于更新当前状态的逻辑。
    /// </summary>
    public virtual void Step()
    {
        if(current != null && Time.timeScale > 0)
        {
            current.Step(entity);
        }
    }

    /// <summary>
    /// 根据状态类型泛型参数切换状态。
    /// </summary>
    /// <typeparam name="TState">目标状态类型，必须继承自 EntityState<T>。</typeparam>
    public virtual void Change<TState>() where TState : EntityState<T>
    {
        var type = typeof(TState);

        if (m_states.ContainsKey(type))
        {
            Change(m_states[type]);
        }
    }

    /// <summary>
    /// 根据状态实例切换当前状态。
    /// 执行状态的退出与进入回调，并触发相关事件。
    /// </summary>
    /// <param name="to">目标状态实例。</param>
    public virtual void Change(EntityState<T> to)
    {
        // 确保目标状态不为空且游戏未暂停（Time.timeScale > 0）
        if (to != null && Time.timeScale > 0)
        {
            // 如果有当前状态，调用退出逻辑并触发退出事件
            if (current != null)
            {
                current.Exit(entity);
                events.onExit.Invoke(current.GetType());
                last = current;
            }

            // 切换到目标状态，调用进入逻辑并触发进入事件和状态切换事件
            current = to;
            current.Enter(entity);
            events.onEnter.Invoke(current.GetType());
            events.onChange?.Invoke();
        }
    }

    /// <summary>
    /// 当实体与其他碰撞体接触时调用，将事件传递给当前状态。
    /// </summary>
    /// <param name="other">碰撞到的其他碰撞体。</param>
    public virtual void OnContact(Collider other)
    {
        if (current != null && Time.timeScale > 0)
        {
            current.OnContact(entity, other);
        }
    }

}
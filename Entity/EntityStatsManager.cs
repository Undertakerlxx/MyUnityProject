using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityStatsManager<T> : MonoBehaviour where T : EntityStats<T>
{
    // 存放所有可用的属性集（例如不同难度或不同状态的属性）
    public T[] stats;

    /// <summary>
    /// 当前激活的属性实例
    /// </summary>
    public T current { get; protected set; }

    /// <summary>
    /// 切换当前属性到指定索引的属性集
    /// </summary>
    /// <param name="to">想要切换到的属性索引</param>
    public virtual void Change(int to)
    {
        // 确保索引合法
        if (to >= 0 && to < stats.Length)
        {
            // 如果切换的不是当前属性，则进行切换
            if (current != stats[to])
            {
                current = stats[to];
            }
        }
    }

    // 启动时自动初始化 current 为 stats 数组中的第一个属性集（如果有）
    protected virtual void Start()
    {
        if (stats.Length > 0)
        {
            current = stats[0];
        }
    }
}

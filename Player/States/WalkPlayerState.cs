using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkPlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        
    }
    protected override void OnExit(Player player)
    {

    }
    protected override void OnStep(Player player)
    {
        //保持贴地 
        player.SnapToGround();
        //重力处理
        player.Gravity();
        //允许跳跃
        player.Jump();
        //检查是否进入下落状态
        player.Fall();
        //冲刺处理
        player.Dash();
        player.Spin();        // 旋转攻击
        player.PickAndThrow();

        // 获取玩家输入方向（相机方向）
        var inputDirection = player.inputs.GetMovementCameraDirection();

        if (inputDirection.sqrMagnitude > 0)
        {
            // 输入方向与当前水平速度的点乘，用于判断刹车阈值
            var dot = Vector3.Dot(inputDirection, player.lateralVelocity);

            if (dot >= player.stats.current.brakeThreshold)
            {
                // 超过刹车阈值 → 正常加速与面向方向
                player.Accelerate(inputDirection);
                player.FaceDirectionSmooth(player.lateralVelocity);
            }
            else
            {
                // 低于刹车阈值 → 进入刹车状态
                player.states.Change<BrakePlayerState>();
            }
        }
        else
        {
            // 没有输入 → 使用摩擦力减速
            player.Friction();

            // 当水平速度为零 → 切换到闲置状态
            if (player.lateralVelocity.sqrMagnitude <= 0)
            {
                player.states.Change<IdlePlayerState>();
            }
        }
        // 玩家按下蹲或爬行 → 切换到蹲伏状态
        if (player.inputs.GetCrouchAndCraw())
        {
            player.states.Change<CrouchPlayerState>();
        }

    }
    public override void OnContact(Player player, Collider other)
    {
        player.PushRigidbody(other);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePlayerState : PlayerState
{
    protected override void OnEnter(Player player)
    {
        
    }
    protected override void OnExit(Player player)
    {
        
    }
    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.SnapToGround();  //保持贴地   
        //允许跳跃
        player.Jump();
        //检查是否进入下落状态
        player.Fall();
        player.Spin();        // 旋转攻击
        player.PickAndThrow();

        var inputDirection = player.inputs.GetMovementDirection();
        if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude > 0)
        {
            player.states.Change<WalkPlayerState>();
        }
        // 如果按下下蹲/爬行 → 切换到 Crouch 状态
        else if (player.inputs.GetCrouchAndCraw())
        {
            player.states.Change<CrouchPlayerState>();
        }
    }
    public override void OnContact(Player player, Collider other)
    {

    }



}

using UnityEngine;

public class RobotController : MonoBehaviour 
{
    // 로봇의 5단계 상태 정의
    public enum RobotState { Standby, Approaching, YieldControl, Docked, E_Stop }
    public RobotState currentState = RobotState.Standby;

    public VehicleController vehicle;

    void Update() 
    {
        CheckFailsafe();
        ExecuteStateMachine();
    }

    void ExecuteStateMachine() 
    {
        switch (currentState) 
        {
            case RobotState.Standby:
                // 차량이 WakeUp 상태가 되면 접근 시작
                if (vehicle.currentState == VehicleController.VehicleState.WakeUp) 
                {
                    currentState = RobotState.Approaching;
                }
                break;

            case RobotState.Approaching:
                MoveTowardsVehicle();
                
                // 차량과의 거리가 30cm 이내가 되면 구동 모터를 멈추고 제어권 이관
                if (vehicle.distanceToRobot <= 0.3f) 
                {
                    currentState = RobotState.YieldControl;
                }
                break;

            case RobotState.YieldControl:
                Debug.Log("로봇: 30cm 이내 진입. 강제 푸시를 멈추고 차량 액추에이터에 제어권을 이관합니다.");
                // 차량이 인입을 완료하면 Docked 상태로 동기화
                currentState = RobotState.Docked;
                break;

            case RobotState.Docked:
                Debug.Log("로봇: 물리적 도킹 완료. 전력 공급 대기 중.");
                break;

            case RobotState.E_Stop:
                EmergencyStop();
                break;
        }
    }

    void CheckFailsafe() 
    {
        // V2I 동기화: 차량 측에서 E-Stop이 걸리면 로봇도 강제 중단
        if (vehicle.currentState == VehicleController.VehicleState.E_Stop) 
        {
            if (currentState != RobotState.E_Stop)
            {
                currentState = RobotState.E_Stop;
            }
        }
    }

    void MoveTowardsVehicle() 
    {
        Debug.Log("로봇: 수신된 UWB 좌표를 기반으로 차량 충전구를 향해 이동 중...");
    }

    void EmergencyStop() 
    {
        Debug.Log("로봇: [비상 정지] 차량의 E-Stop 신호 수신. 모든 구동계를 즉시 차단합니다.");
    }
}
// PowerUpController.cs
using UnityEngine;

// 앞으로 추가될 모든 파워업의 종류를 여기서 관리합니다.
public enum PowerUpType
{
    FireRateIncrease,
    BulletSpeedIncrease,
    // 여기에 다른 파워업 종류를 추가할 수 있습니다.
}

public class PowerUpController : MonoBehaviour
{
    // 이 아이템이 어떤 종류의 파워업인지 Inspector에서 설정할 수 있습니다.
    public PowerUpType type;
}
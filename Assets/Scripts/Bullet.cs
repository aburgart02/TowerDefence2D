using UnityEngine;

public enum BulletType
{
    Rock,
    Arrow,
    Fireball
};

public class Bullet : MonoBehaviour {
    [SerializeField]
    private int attackStrength;

    [SerializeField]
    private BulletType bulletType;

    public Enemy target;
    public Vector2 lastPosition;
    public int AttackStrength => attackStrength;

    public BulletType BulletType => bulletType;
}
